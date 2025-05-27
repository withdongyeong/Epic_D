using UnityEngine;
using System.Collections;
using Game3.Scripts.Arrow;
using Game3.Scripts.Core;

namespace Game3.Scripts.Character.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        /// <summary>
        /// 몬스터 이동 속도
        /// </summary>
        private float moveSpeed = 2f;
        
        /// <summary>
        /// 몬스터 최대 체력
        /// </summary>
        private int maxHealth = 50;
        
        /// <summary>
        /// 현재 몬스터 체력
        /// </summary>
        private int currentHealth;
        
        /// <summary>
        /// 몬스터 공격력
        /// </summary>
        private int attackDamage = 10;
        
        /// <summary>
        /// 몬스터 공격 범위
        /// </summary>
        private float attackRange = 1.5f;
        
        /// <summary>
        /// 몬스터 감지 범위
        /// </summary>
        private float detectionRange = 10f;
        
        /// <summary>
        /// 돌진 속도
        /// </summary>
        private float dashSpeed = 8f;
        
        /// <summary>
        /// 공격 쿨다운
        /// </summary>
        private float attackCooldown = 2f;
        
        /// <summary>
        /// 현재 공격 쿨다운
        /// </summary>
        private float currentAttackCooldown = 0f;
        
        /// <summary>
        /// 몬스터가 사망 시 부여하는 점수
        /// </summary>
        private int scoreValue = 10;
        
        /// <summary>
        /// 현재 공격 중인지 여부
        /// </summary>
        private bool isAttacking = false;
        
        /// <summary>
        /// 화살에 꿰뚫렸는지 여부
        /// </summary>
        private bool isImpaled = false;
        
        /// <summary>
        /// 몬스터를 꿰뚫은 화살
        /// </summary>
        private GameObject impalingArrow;
        
        /// <summary>
        /// 꿰뚫린 상태에서 회복 중인지 여부
        /// </summary>
        private bool isRecoveringFromImpale = false;
        
        /// <summary>
        /// 꿰뚫림 회복 시간
        /// </summary>
        private float impaleRecoveryTime = 1.5f;
        
        /// <summary>
        /// 꿰뚫림 회복 타이머
        /// </summary>
        private float recoveryTimer = 0f;
        
        /// <summary>
        /// 무적 상태 여부
        /// </summary>
        private bool isInvulnerable = false;
        
        /// <summary>
        /// 무적 시간
        /// </summary>
        private float invulnerabilityDuration = 0.5f;
        
        /// <summary>
        /// 무적 타이머
        /// </summary>
        private float invulnerabilityTimer = 0f;
        
        /// <summary>
        /// 플레이어 트랜스폼 참조
        /// </summary>
        private Transform playerTransform;
        
        /// <summary>
        /// 리지드바디 컴포넌트
        /// </summary>
        private Rigidbody2D rb;
        
        /// <summary>
        /// 애니메이터 컴포넌트 (있을 경우)
        /// </summary>
        private Animator animator;
        
        /// <summary>
        /// 스프라이트 렌더러
        /// </summary>
        private SpriteRenderer spriteRenderer;
        
        /// <summary>
        /// 게임 매니저 참조
        /// </summary>
        private GameManager gameManager;

        /// <summary>
        /// 화살 상태 확인 타이머
        /// </summary>
        private float arrowCheckTimer = 0f;
        
        /// <summary>
        /// 화살 상태 확인 간격
        /// </summary>
        private float arrowCheckInterval = 0.2f;
        
        /// <summary>
        /// 몬스터 최대 체력 프로퍼티
        /// </summary>
        public int MaxHealth { get => maxHealth; set => maxHealth = value; }
        
        /// <summary>
        /// 현재 몬스터 체력 프로퍼티
        /// </summary>
        public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
        
        /// <summary>
        /// 몬스터 공격력 프로퍼티
        /// </summary>
        public int AttackDamage { get => attackDamage; set => attackDamage = value; }
        
        /// <summary>
        /// 몬스터 점수 프로퍼티
        /// </summary>
        public int ScoreValue { get => scoreValue; set => scoreValue = value; }
        
        /// <summary>
        /// 화살에 꿰뚫렸는지 여부 프로퍼티
        /// </summary>
        public bool IsImpaled { get => isImpaled; }
        
        /// <summary>
        /// 몬스터 이동 속도 프로퍼티
        /// </summary>
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        
        /// <summary>
        /// 몬스터 공격 범위 프로퍼티
        /// </summary>
        public float AttackRange { get => attackRange; set => attackRange = value; }
        
        /// <summary>
        /// 몬스터 감지 범위 프로퍼티
        /// </summary>
        public float DetectionRange { get => detectionRange; set => detectionRange = value; }
        
        /// <summary>
        /// 돌진 속도 프로퍼티
        /// </summary>
        public float DashSpeed { get => dashSpeed; set => dashSpeed = value; }
        
        /// <summary>
        /// 공격 쿨다운 프로퍼티
        /// </summary>
        public float AttackCooldown { get => attackCooldown; set => attackCooldown = value; }
        
        /// <summary>
        /// 꿰뚫림 회복 시간 프로퍼티
        /// </summary>
        public float ImpaleRecoveryTime { get => impaleRecoveryTime; set => impaleRecoveryTime = value; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            currentHealth = maxHealth;
            gameManager = GameManager.Instance;
            
            // 플레이어 찾기
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
            }
        }

        private void Update()
        {
            // 무적 시간 업데이트
            UpdateInvulnerabilityState();
            
            // 화살이 사라졌는지 확인 (꿰뚫려 있을 때만)
            if (isImpaled)
            {
                // 화살이 존재하지 않거나 비활성화된 경우 꿰뚫림 해제
                if (impalingArrow == null || !impalingArrow.activeInHierarchy)
                {
                    Debug.Log("화살이 없어졌거나 비활성화됨 감지: 몬스터 상태 해제");
                    ReleaseFromArrow();
                }
                
                // 꿰뚫린 상태면 이동 처리 안함
                return;
            }
            
            // 꿰뚫림 회복 중이면 타이머 업데이트
            if (isRecoveringFromImpale)
            {
                recoveryTimer -= Time.deltaTime;
                if (recoveryTimer <= 0)
                {
                    FinishRecovery();
                }
                return; // 회복 중에는 움직이지 않음
            }
            
            // 플레이어 감지 및 추적
            TrackPlayer();
        }
        
        /// <summary>
        /// 무적 상태 업데이트
        /// </summary>
        private void UpdateInvulnerabilityState()
        {
            if (isInvulnerable)
            {
                invulnerabilityTimer -= Time.deltaTime;
                if (invulnerabilityTimer <= 0f)
                {
                    isInvulnerable = false;
                    // 깜빡임 효과 종료
                    if (spriteRenderer != null && !isImpaled && !isRecoveringFromImpale)
                    {
                        spriteRenderer.color = Color.white;
                    }
                }
            }
        }
        
        /// <summary>
        /// 화살에서 분리 및 회복 상태 전환
        /// </summary>
        private void ReleaseFromArrow()
        {
            if (!isImpaled) return;
            
            Debug.Log($"몬스터 {gameObject.name}가 화살에서 분리됨");
            
            // 혹시 부모가 설정되어 있다면 해제
            if (transform.parent != null)
            {
                transform.SetParent(null);
            }
            
            // 상태 변경
            isImpaled = false;
            impalingArrow = null;
            
            // 회복 상태로 전환
            StartRecovery();
        }
        
        /// <summary>
        /// 회복 상태 시작
        /// </summary>
        private void StartRecovery()
        {
            // 이미 회복 중이면 중복 실행 방지
            if (isRecoveringFromImpale) return;
            
            // 시각적 효과
            if (spriteRenderer != null)
            {
                spriteRenderer.color = new Color(0.7f, 0.7f, 1f); // 약간 푸른 색 (회복 중)
            }
            
            // 무적 상태 해제
            isInvulnerable = false;
            
            // 회복 시간 설정
            isRecoveringFromImpale = true;
            recoveryTimer = impaleRecoveryTime;
            
            // 약간 뒤로 밀려나는 효과 (선택적)
            if (playerTransform != null)
            {
                Vector2 knockbackDir = ((Vector2)transform.position - (Vector2)playerTransform.position).normalized;
                rb.AddForce(knockbackDir * 2f, ForceMode2D.Impulse);
            }
            
            // 디버그 로그
            Debug.Log($"몬스터 {gameObject.name}가 회복 상태 시작 ({impaleRecoveryTime}초)");
        }
        
        /// <summary>
        /// 회복 완료 처리
        /// </summary>
        private void FinishRecovery()
        {
            isRecoveringFromImpale = false;
            
            // 시각적 효과 원래대로
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.white;
            }
            
            // 추적 재개
            ResumeTrackingPlayer();
            
            // 디버그 로그
            Debug.Log($"몬스터 {gameObject.name}가 회복 완료, 일반 상태 복귀");
        }
        
        /// <summary>
        /// 플레이어 감지 및 추적
        /// </summary>
        private void TrackPlayer()
        {
            if (playerTransform == null)
            {
                // 플레이어를 못 찾으면 다시 검색
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
                else
                {
                    return; // 플레이어가 없으면 종료
                }
            }
            
            if (isAttacking) return; // 공격 중이면 처리하지 않음
            
            // 플레이어와의 거리 계산
            float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
            
            // 쿨다운 감소
            if (currentAttackCooldown > 0)
            {
                currentAttackCooldown -= Time.deltaTime;
            }
            
            // 플레이어가 감지 범위 내에 있을 때
            if (distanceToPlayer <= detectionRange)
            {
                // 공격 범위 안에 있고 쿨다운이 끝났을 때 공격
                if (distanceToPlayer <= attackRange && currentAttackCooldown <= 0)
                {
                    StartCoroutine(AttackPattern());
                }
                // 그렇지 않으면 플레이어를 향해 이동
                else
                {
                    MoveTowardsPlayer();
                }
            }
            else
            {
                // 감지 범위 밖이면 멈춤
                StopMoving();
            }
        }

        /// <summary>
        /// 플레이어를 향해 이동하는 메서드
        /// </summary>
        private void MoveTowardsPlayer()
        {
            if (playerTransform != null)
            {
                Vector2 direction = (playerTransform.position - transform.position).normalized;
                rb.linearVelocity = direction * moveSpeed;
                
                // 이동 방향으로 회전
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
                
                // 애니메이션 설정 (있을 경우)
                if (animator != null)
                {
                    animator.SetBool("IsMoving", true);
                }
            }
        }
        
        /// <summary>
        /// 몬스터 이동 중지
        /// </summary>
        private void StopMoving()
        {
            rb.linearVelocity = Vector2.zero;
            
            // 애니메이션 설정 (있을 경우)
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
        }
        
        /// <summary>
        /// 회복 후 플레이어 추적 재개
        /// </summary>
        private void ResumeTrackingPlayer()
        {
            // 플레이어 다시 찾기 (필요한 경우)
            if (playerTransform == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                }
            }
            
            if (playerTransform != null)
            {
                // 플레이어와의 거리 계산
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                
                // 플레이어가 감지 범위 내에 있으면 즉시 이동 시작
                if (distanceToPlayer <= detectionRange)
                {
                    // 플레이어 방향으로 회전
                    Vector2 direction = (playerTransform.position - transform.position).normalized;
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle);
                    
                    // 즉시 이동 시작 (약간 더 빠르게 시작하여 반응성 높임)
                    rb.linearVelocity = direction * moveSpeed;
                    
                    // 애니메이션 설정 (있을 경우)
                    if (animator != null)
                    {
                        animator.SetBool("IsMoving", true);
                    }
                    
                    Debug.Log($"몬스터 {gameObject.name}가 플레이어 추적 재개 - 거리: {distanceToPlayer}");
                }
            }
        }

        /// <summary>
        /// 몬스터 공격 패턴 - 잠시 멈췄다가 플레이어 방향으로 돌진
        /// </summary>
        private IEnumerator AttackPattern()
        {
            isAttacking = true;
            
            // 현재 속도 저장
            Vector2 originalVelocity = rb.linearVelocity;
            
            // 멈추기
            rb.linearVelocity = Vector2.zero;
            
            // 애니메이션 설정 (있을 경우)
            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
                animator.SetTrigger("Attack");
            }
            
            // 잠시 대기 (준비 모션)
            yield return new WaitForSeconds(0.5f);
            
            if (playerTransform != null)
            {
                // 플레이어 방향으로 돌진
                Vector2 dashDirection = (playerTransform.position - transform.position).normalized;
                rb.linearVelocity = dashDirection * dashSpeed;
                
                // 돌진 지속 시간
                yield return new WaitForSeconds(0.3f);
                
                // 속도 복원
                rb.linearVelocity = originalVelocity;
            }
            
            // 쿨다운 설정
            currentAttackCooldown = attackCooldown;
            isAttacking = false;
            
            // 애니메이션 설정 (있을 경우)
            if (animator != null)
            {
                animator.SetBool("IsMoving", rb.linearVelocity.magnitude > 0.1f);
            }
        }

        /// <summary>
        /// 화살에 꿰뚫린 상태 설정
        /// </summary>
        /// <param name="impaled">꿰뚫렸는지 여부</param>
        /// <param name="arrow">몬스터를 꿰뚫은 화살</param>
        public void SetImpaled(bool impaled, GameObject arrow)
        {
            // 상태 변화가 없으면 무시
            if (isImpaled == impaled) return;
            
            isImpaled = impaled;
            
            if (impaled)
            {
                // 꿰뚫린 상태 시작
                impalingArrow = arrow;
                
                // 모든 물리 효과 및 이동 중지
                rb.linearVelocity = Vector2.zero;
                
                // 애니메이션 설정 (있을 경우)
                if (animator != null)
                {
                    animator.SetBool("IsMoving", false);
                    animator.SetTrigger("Hit");
                }
                
                // 시각적 효과 (선택적)
                if (spriteRenderer != null)
                {
                    spriteRenderer.color = new Color(1f, 0.7f, 0.7f); // 약간 붉은 색
                }
                
                // 꿰뚫린 동안 무적 상태 설정
                isInvulnerable = true;
                invulnerabilityTimer = float.MaxValue; // 꿰뚫려 있는 동안 계속 무적
                
                // 회복 상태 리셋 (만약 도중에 다시 꿰뚫렸다면)
                isRecoveringFromImpale = false;
                
                Debug.Log($"몬스터 {gameObject.name}가 화살에 꿰뚫림");
            }
            else
            {
                // 꿰뚫린 상태 종료
                impalingArrow = null;
                
                // 회복 상태로 전환
                StartRecovery();
            }
        }

        /// <summary>
        /// 플레이어와 충돌했을 때 데미지 처리
        /// </summary>
        private void OnCollisionEnter2D(Collision2D collision)
        {
            // 꿰뚫렸거나 회복 중이면 충돌 무시
            if (isImpaled || isRecoveringFromImpale) return;
            
            if (collision.gameObject.CompareTag("Player"))
            {
                Player.PlayerController player = collision.gameObject.GetComponent<Player.PlayerController>();
                if (player != null)
                {
                    player.TakeDamage(attackDamage);
                }
            }
        }

        /// <summary>
        /// 몬스터가 데미지를 입는 메서드
        /// </summary>
        /// <param name="damage">입는 데미지 양</param>
        public void TakeDamage(int damage)
        {
            // 무적 상태면 데미지를 입지 않음
            if (isInvulnerable) return;
            
            // 체력 감소
            currentHealth -= damage;
            
            // 일시적 무적 상태 설정
            isInvulnerable = true;
            invulnerabilityTimer = invulnerabilityDuration;
            
            // 애니메이션 설정 (있을 경우)
            if (animator != null && !isImpaled)
            {
                animator.SetTrigger("Hit");
            }
            
            // 시각적 효과 (선택적)
            StartCoroutine(FlashEffect());
            
            // 디버그 로그
            Debug.Log($"몬스터 체력: {currentHealth}/{maxHealth} (데미지: {damage})");
            
            // 체력이 0 이하면 사망 처리
            if (currentHealth <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// 히트 시 깜빡임 효과
        /// </summary>
        private IEnumerator FlashEffect()
        {
            if (spriteRenderer == null || isImpaled) yield break;
            
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            
            // 원래 상태로 복구 (꿰뚫린 상태가 아닐 때만)
            if (!isImpaled && !isRecoveringFromImpale)
            {
                spriteRenderer.color = originalColor;
            }
        }

        /// <summary>
        /// 몬스터 사망 처리
        /// </summary>
        private void Die()
        {
            // 꿰뚫린 상태라면 화살에서 분리
            if (isImpaled && impalingArrow != null)
            {
                // 부모 관계 해제
                transform.SetParent(null);
                
                // 화살 컨트롤러에서 이 몬스터 참조 제거
                ArrowController arrowCtrl = impalingArrow.GetComponent<ArrowController>();
                if (arrowCtrl != null)
                {
                    // RemoveImpaledEnemy 메서드를 호출하여 이 몬스터 제거
                    arrowCtrl.RemoveImpaledEnemy(gameObject);
                }
                
                // 상태 초기화
                isImpaled = false;
                impalingArrow = null;
            }
            
            // 애니메이션 설정 (있을 경우)
            if (animator != null)
            {
                animator.SetTrigger("Die");
                // 애니메이션이 끝난 후 파괴하려면 이벤트를 사용하거나 
                // 여기서는 간단히 지연 후 파괴
                Destroy(gameObject, 1f);
            }
            else
            {
                // 애니메이터가 없으면 바로 파괴
                OnDeathEffects();
            }
        }
        
        /// <summary>
        /// 사망 시 효과 처리
        /// </summary>
        private void OnDeathEffects()
        {
            // 게임 매니저에 점수 추가
            if (gameManager != null)
            {
                gameManager.AddScore(scoreValue);
            }
            
            // 몬스터 오브젝트 파괴
            Destroy(gameObject);
        }
        
        /// <summary>
        /// 화살 상태를 강제로 확인하는 메서드
        /// </summary>
        private void OnDestroy()
        {
            // 화살에 꿰뚫린 상태로 파괴된다면, 화살에서 참조 제거
            if (isImpaled && impalingArrow != null)
            {
                ArrowController arrowCtrl = impalingArrow.GetComponent<ArrowController>();
                if (arrowCtrl != null)
                {
                    arrowCtrl.RemoveImpaledEnemy(gameObject);
                }
            }
        }
        
        /// <summary>
        /// 비활성화될 때 화살 참조 정리
        /// </summary>
        private void OnDisable()
        {
            // 화살에 꿰뚫린 상태로 비활성화된다면, 화살에서 참조 제거
            if (isImpaled && impalingArrow != null)
            {
                ArrowController arrowCtrl = impalingArrow.GetComponent<ArrowController>();
                if (arrowCtrl != null)
                {
                    arrowCtrl.RemoveImpaledEnemy(gameObject);
                }
            }
        }
    }
}