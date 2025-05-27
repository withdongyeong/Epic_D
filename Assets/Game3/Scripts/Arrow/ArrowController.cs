using UnityEngine;
using System.Collections.Generic;
using Game3.Scripts.Character.Enemy;

namespace Game3.Scripts.Arrow
{
    public class ArrowController : MonoBehaviour
    {
        /// <summary>
        /// 화살이 입히는 데미지
        /// </summary>
        [SerializeField] private int damage = 20;
        
        /// <summary>
        /// 화살 크기 변화 최대 배율 (파워가 최대일 때)
        /// </summary>
        [SerializeField] private float maxSizeMultiplier = 1.5f;
        
        /// <summary>
        /// 꿰뚫기 힘 (화살이 몇 마리까지 꿰뚫을 수 있는지)
        /// </summary>
        [SerializeField] private int piercingPower = 3;
        
        /// <summary>
        /// 꿰뚫기 감소량 (몬스터 하나를 꿰뚫을 때마다 감소하는 꿰뚫기 힘)
        /// </summary>
        [SerializeField] private int piercingReduction = 1;
        
        /// <summary>
        /// 현재 꿰뚫기 힘
        /// </summary>
        private int currentPiercingPower;
        
        /// <summary>
        /// 꿰뚫린 몬스터들 목록
        /// </summary>
        private List<GameObject> impaledEnemies = new List<GameObject>();
        
        /// <summary>
        /// 이미 충돌한 몬스터들 (중복 충돌 방지)
        /// </summary>
        private List<Collider2D> hitColliders = new List<Collider2D>();
        
        /// <summary>
        /// 화살이 목적지에 도달했는지 여부
        /// </summary>
        private bool reachedDestination = false;
        
        /// <summary>
        /// 화살이 사라지기까지 남은 시간
        /// </summary>
        private float destroyTimer = 3f;
        
        /// <summary>
        /// 목적지 도달 후 몬스터 제어 해제까지 대기 시간
        /// </summary>
        private float releaseEnemyDelay = 1f;
        
        /// <summary>
        /// 원래 화살 크기
        /// </summary>
        private Vector3 originalScale;
        
        /// <summary>
        /// 곡선 이동 컴포넌트
        /// </summary>
        private ArrowCurvedMovement curvedMovement;
        
        /// <summary>
        /// 리지드바디 컴포넌트
        /// </summary>
        private Rigidbody2D rb;
        
        /// <summary>
        /// 콜라이더 컴포넌트
        /// </summary>
        private Collider2D col;
        
        /// <summary>
        /// 화살 데미지 프로퍼티
        /// </summary>
        public int Damage { get => damage; set => damage = value; }
        
        /// <summary>
        /// 꿰뚫기 힘 프로퍼티
        /// </summary>
        public int PiercingPower { get => piercingPower; set => piercingPower = value; }

        private void Awake()
        {
            originalScale = transform.localScale;
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            curvedMovement = GetComponent<ArrowCurvedMovement>();
            currentPiercingPower = piercingPower;
        }
        
        private void Update()
        {
            // 목적지에 도달한 후 타이머 카운트다운
            if (reachedDestination)
            {
                destroyTimer -= Time.deltaTime;
                if (destroyTimer <= 0)
                {
                    // 화살 파괴 전 몬스터 해제
                    ReleaseAllImpaledEnemies();
                    
                    // 지연 시간을 두고 화살 파괴 (몬스터가 분리될 시간 제공)
                    Destroy(gameObject, 0.1f);
                }
            }
        }

        /// <summary>
        /// 화살 속도와 방향 초기화
        /// </summary>
        /// <param name="direction">화살 발사 방향</param>
        /// <param name="speed">화살 속도</param>
        /// <param name="power">화살 파워 (0-1, 기본값 1)</param>
        /// <param name="target">목표 위치 (선택 사항)</param>
        public void Initialize(Vector2 direction, float speed, float power = 1f, Vector2? target = null)
        {
            // 파워에 따라 꿰뚫기 힘 보너스 (선택적)
            currentPiercingPower = piercingPower + Mathf.FloorToInt(power * 2); // 최대 파워에서 +2 보너스
            
            // 화살 속도 설정
            if (rb != null && curvedMovement == null)
            {
                rb.linearVelocity = direction.normalized * speed;
                
                // 화살이 속도 방향으로 회전
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            
            // 파워에 따라 화살 크기 변경
            if (power > 0)
            {
                // 파워에 비례하여 화살 크기 증가 (1.0-1.5배)
                float sizeMultiplier = 1f + (power * (maxSizeMultiplier - 1f));
                transform.localScale = originalScale * sizeMultiplier;
                
                // 선택적: 파워에 따라 화살 색상 변경
                SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    // 파워가 높을수록 더 밝은 색상
                    float intensity = 0.5f + (power * 0.5f);
                    spriteRenderer.color = new Color(intensity, intensity, intensity);
                }
            }
            
            // 데미지도 파워에 따라 조정 (이미 기존 로직에서 처리됨)
        }
        
        /// <summary>
        /// 목적지 도달 이벤트 처리
        /// </summary>
        public void OnReachDestination()
        {
            reachedDestination = true;
            
            // 속도 멈추기
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.isKinematic = true; // 물리 효과 비활성화
            }
            
            // 곡선 이동 중지
            if (curvedMovement != null)
            {
                curvedMovement.StopMovement();
            }
            
            // 콜라이더 비활성화로 추가 충돌 방지
            if (col != null)
            {
                col.enabled = false;
            }
            
            // 꿰뚫린 모든 몬스터에게 데미지 입히기
            ApplyDamageToImpaledEnemies();
            
            // 몬스터가 꿰뚫려 있으면 몬스터 고정
            if (impaledEnemies.Count > 0)
            {
                // 몬스터 제어권 해제 예약
                Invoke("ReleaseAllImpaledEnemies", releaseEnemyDelay);
            }
        }
        
        /// <summary>
        /// 꿰뚫린 모든 몬스터에게 데미지 적용
        /// </summary>
        private void ApplyDamageToImpaledEnemies()
        {
            foreach (GameObject enemyObj in impaledEnemies)
            {
                if (enemyObj == null) continue;
                
                EnemyController enemy = enemyObj.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    // 몬스터에게 데미지 입히기
                    int reducedDamage = Mathf.Max(1, damage / 3);
                    enemy.TakeDamage(reducedDamage);
                    
                    // 디버그 로그
                    Debug.Log($"목적지 도달 시 몬스터 {enemyObj.name}에게 {reducedDamage} 데미지");
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // 목적지에 도달했거나 꿰뚫기 힘이 없으면 추가 충돌 무시
            if (reachedDestination || currentPiercingPower <= 0) return;
            
            // 이미 충돌한 콜라이더면 무시
            if (hitColliders.Contains(other)) return;
            
            // 몬스터에 닿았을 때
            if (other.CompareTag("Enemy"))
            {
                hitColliders.Add(other); // 중복 충돌 방지
                
                EnemyController enemy = other.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    // 중복 꿰뚫기 방지 위해 현재 상태 확인
                    if (!enemy.IsImpaled)
                    {
                        // 몬스터 꿰뚫기
                        ImpaleEnemy(other.gameObject, enemy);
                        
                        // 꿰뚫기 힘 감소
                        currentPiercingPower -= piercingReduction;
                    }
                }
            }
            
            // 장애물에 닿았을 때 (맵 경계나 벽 등)
            if (other.CompareTag("Obstacle"))
            {
                OnReachDestination();
            }
        }
        
        /// <summary>
        /// 몬스터를 화살에 꿰뚫는 처리
        /// </summary>
        /// <param name="enemyObj">몬스터 게임오브젝트</param>
        /// <param name="enemyController">몬스터 컨트롤러</param>
        private void ImpaleEnemy(GameObject enemyObj, EnemyController enemyController)
        {
            impaledEnemies.Add(enemyObj);
            
            // 몬스터의 제어권 빼앗기
            enemyController.SetImpaled(true, this.gameObject);
            
            // 몬스터를 화살의 자식으로 설정하여 함께 이동하게 함
            enemyObj.transform.SetParent(this.transform);
            
            // 몬스터 위치 조정 (화살 앞쪽에 배치)
            // 여러 몬스터일 경우 순서대로 배치
            float offsetX = 0.5f + (impaledEnemies.Count - 1) * 0.3f;
            enemyObj.transform.localPosition = new Vector3(offsetX, 0, 0);
            
            // 몬스터 회전 고정 (화살과 같은 방향 유지)
            enemyObj.transform.localRotation = Quaternion.identity;
            
            // 몬스터 콜라이더 일시적으로 비활성화하여 추가 충돌 방지
            Collider2D[] enemyColliders = enemyObj.GetComponents<Collider2D>();
            foreach (Collider2D collider in enemyColliders)
            {
                collider.enabled = false;
            }
            
            // 몬스터 리지드바디 비활성화
            Rigidbody2D enemyRb = enemyObj.GetComponent<Rigidbody2D>();
            if (enemyRb != null)
            {
                enemyRb.linearVelocity = Vector2.zero;
                enemyRb.isKinematic = true;
            }
            
            // 시각적 효과: 몬스터를 조금 회전시켜 꿰뚫린 느낌 주기
            enemyObj.transform.Rotate(0, 0, Random.Range(-20f, 20f));
        }
        
        /// <summary>
        /// 모든 꿰뚫린 몬스터 해제
        /// </summary>
        private void ReleaseAllImpaledEnemies()
        {
            // 임시 리스트에 몬스터들을 복사 (원본 리스트가 수정되는 문제 방지)
            List<GameObject> enemiesToRelease = new List<GameObject>(impaledEnemies);
            
            foreach (GameObject enemyObj in enemiesToRelease)
            {
                if (enemyObj == null) continue;
                
                // 몬스터의 부모 관계 해제
                enemyObj.transform.SetParent(null);
                
                // 몬스터 컨트롤러에 제어권 반환
                EnemyController enemyController = enemyObj.GetComponent<EnemyController>();
                if (enemyController != null)
                {
                    enemyController.SetImpaled(false, null);
                }
                
                // 몬스터 콜라이더 다시 활성화
                Collider2D[] enemyColliders = enemyObj.GetComponents<Collider2D>();
                foreach (Collider2D collider in enemyColliders)
                {
                    collider.enabled = true;
                }
                
                // 몬스터 리지드바디 다시 활성화
                Rigidbody2D enemyRb = enemyObj.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    enemyRb.isKinematic = false;
                }
                
                // 몬스터 회전 원래대로 (선택적)
                enemyObj.transform.rotation = Quaternion.identity;
                
                // 디버그 로그
                Debug.Log($"몬스터 해제: {enemyObj.name}");
            }
            
            // 꿰뚫기 상태 초기화
            impaledEnemies.Clear();
        }
        
        /// <summary>
        /// 꿰뚫린 몬스터 하나를 목록에서 제거
        /// </summary>
        /// <param name="enemyObj">제거할 몬스터 게임오브젝트</param>
        public void RemoveImpaledEnemy(GameObject enemyObj)
        {
            if (impaledEnemies.Contains(enemyObj))
            {
                impaledEnemies.Remove(enemyObj);
                Debug.Log($"몬스터 꿰뚫기 목록에서 제거: {enemyObj.name}, 남은 몬스터: {impaledEnemies.Count}");
            }
        }        /// <summary>
        /// 온 디스트로이 이벤트 - 화살이 파괴될 때 호출됨
        /// </summary>
        private void OnDestroy()
        {
            // 아직 꿰뚫린 몬스터가 있다면 모두 해제
            if (impaledEnemies.Count > 0)
            {
                ReleaseAllImpaledEnemies();
            }
        }
    }
}