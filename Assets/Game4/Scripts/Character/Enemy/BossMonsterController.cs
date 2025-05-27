using Game4.Scripts.Character.Player;
using Game4.Scripts.Core;
using Game4.Scripts.VFX;

namespace Game4.Scripts.Character.Enemy
{
#pragma warning disable CS0618, CS0612, CS0672
using UnityEngine;
using System;
#pragma warning restore CS0618, CS0612, CS0672

public class BossMonsterController : MonoBehaviour, IMonster
{
    /// <summary>
    /// 몬스터 이동 속도
    /// </summary>
    private float moveSpeed = 8f;
    
    /// <summary>
    /// 몬스터 최대 체력 (매우 높음)
    /// </summary>
    private int maxHealth = 500;
    
    /// <summary>
    /// 현재 몬스터 체력
    /// </summary>
    private int currentHealth;
    
    /// <summary>
    /// 기본 공격 데미지
    /// </summary>
    private int attackDamage = 20;
    
    /// <summary>
    /// 공격 사거리
    /// </summary>
    private float attackRange = 2f;
    
    /// <summary>
    /// 추적 사거리
    /// </summary>
    private float chaseRange = 1200000f;
    
    /// <summary>
    /// 공격 쿨다운
    /// </summary>
    private float attackCooldown = 1.5f;
    
    /// <summary>
    /// 현재 공격 쿨다운 타이머
    /// </summary>
    private float attackCooldownTimer = 0f;
    
    /// <summary>
    /// 특수 공격 쿨다운
    /// </summary>
    private float specialAttackCooldown = 8f;
    
    /// <summary>
    /// 특수 공격 쿨다운 타이머
    /// </summary>
    private float specialAttackTimer = 0f;
    
    /// <summary>
    /// 투사체 프리팹
    /// </summary>
    public GameObject projectilePrefab;
    
    /// <summary>
    /// 투사체 속도
    /// </summary>
    private float projectileSpeed = 9f;
    
    /// <summary>
    /// 돌진 속도
    /// </summary>
    private float chargeSpeed = 10f;
    
    /// <summary>
    /// 돌진 지속 시간
    /// </summary>
    private float chargeDuration = 1f;
    
    /// <summary>
    /// 돌진 타이머
    /// </summary>
    private float chargeTimer = 0f;
    
    /// <summary>
    /// 돌진 방향
    /// </summary>
    private Vector2 chargeDirection;
    
    /// <summary>
    /// 현재 공격 패턴
    /// </summary>
    private AttackPattern currentPattern = AttackPattern.Basic;
    
    /// <summary>
    /// 플레이어 참조
    /// </summary>
    private Transform player;
    
    /// <summary>
    /// 리지드바디 컴포넌트
    /// </summary>
    private Rigidbody2D rb;
    
    /// <summary>
    /// 몬스터 상태
    /// </summary>
    private MonsterState state = MonsterState.Idle;
    
    /// <summary>
    /// 피격 효과 컴포넌트
    /// </summary>
    private HitEffect hitEffect;

    // Events
    /// <summary>
    /// 몬스터 사망 이벤트
    /// </summary>
    public event Action OnMonsterDeath;

    // Properties
    /// <summary>
    /// 공격 데미지 프로퍼티
    /// </summary>
    public int AttackDamage { get => attackDamage; set => attackDamage = value; }
    
    /// <summary>
    /// 투사체 프리팹 프로퍼티
    /// </summary>
    public GameObject ProjectilePrefab { get => projectilePrefab; set => projectilePrefab = value; }
    
    /// <summary>
    /// 체력 비율 프로퍼티
    /// </summary>
    public float HealthRatio => (float)currentHealth / maxHealth;

    /// <summary>
    /// 몬스터 상태 열거형
    /// </summary>
    private enum MonsterState
    {
        Idle,
        Chase,
        Attack,
        SpecialAttack,
        Charging
    }

    /// <summary>
    /// 공격 패턴 열거형
    /// </summary>
    private enum AttackPattern
    {
        Basic,      // 기본 근접 공격
        Ranged,     // 원거리 공격
        Charge,     // 돌진 공격
        MultiShot   // 다중 발사
    }

    /// <summary>
    /// 컴포넌트 초기화
    /// </summary>
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        hitEffect = GetComponent<HitEffect>();
        
        if (hitEffect == null)
        {
            hitEffect = gameObject.AddComponent<HitEffect>();
        }
    }

    /// <summary>
    /// 게임 시작 시 초기화
    /// </summary>
    private void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        specialAttackTimer = specialAttackCooldown;
        
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure Player has 'Player' tag.");
        }
        
        Debug.Log("Boss Monster spawned with " + maxHealth + " HP!");
    }

    /// <summary>
    /// 매 프레임 업데이트
    /// </summary>
    private void Update()
    {
        if (player == null) return;
        
        UpdateState();
        HandleBehavior();
        UpdateCooldowns();
    }

    /// <summary>
    /// 플레이어와의 거리에 따라 상태 업데이트
    /// </summary>
    private void UpdateState()
    {
        if (state == MonsterState.Charging || state == MonsterState.SpecialAttack) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        // 체력에 따른 공격성 증가
        float aggressionMultiplier = 1f + (1f - HealthRatio); // 체력이 낮을수록 더 공격적
        
        // 특수 공격 빈도 증가 (체력이 낮을수록)
        float adjustedSpecialCooldown = specialAttackCooldown / aggressionMultiplier;
        
        if (specialAttackTimer <= 0 && UnityEngine.Random.Range(0f, 1f) < 0.3f * aggressionMultiplier)
        {
            state = MonsterState.SpecialAttack;
            return;
        }
        
        if (distanceToPlayer <= attackRange)
        {
            state = MonsterState.Attack;
        }
        else if (distanceToPlayer <= chaseRange)
        {
            state = MonsterState.Chase;
        }
        else
        {
            state = MonsterState.Idle;
        }
    }
    /// <summary>
    /// 상태에 따른 행동 처리
    /// </summary>
    private void HandleBehavior()
    {
        switch (state)
        {
            case MonsterState.Idle:
                HandleIdle();
                break;
            case MonsterState.Chase:
                HandleChase();
                break;
            case MonsterState.Attack:
                HandleAttack();
                break;
            case MonsterState.SpecialAttack:
                HandleSpecialAttack();
                break;
            case MonsterState.Charging:
                HandleCharging();
                break;
        }
    }

    /// <summary>
    /// 대기 상태 처리
    /// </summary>
    private void HandleIdle()
    {
        rb.linearVelocity = Vector2.zero;
    }

    /// <summary>
    /// 추적 상태 처리
    /// </summary>
    private void HandleChase()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * moveSpeed;
        
        // 플레이어 방향으로 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// 기본 공격 상태 처리
    /// </summary>
    private void HandleAttack()
    {
        rb.linearVelocity = Vector2.zero;
        
        if (attackCooldownTimer <= 0)
        {
            BasicAttack();
            attackCooldownTimer = attackCooldown;
        }
    }

    /// <summary>
    /// 특수 공격 상태 처리
    /// </summary>
    private void HandleSpecialAttack()
    {
        rb.linearVelocity = Vector2.zero;
        
        // 체력에 따라 공격 패턴 선택
        if (HealthRatio > 0.7f)
        {
            currentPattern = AttackPattern.Ranged;
        }
        else if (HealthRatio > 0.4f)
        {
            currentPattern = AttackPattern.Charge;
        }
        else
        {
            currentPattern = AttackPattern.MultiShot;
        }
        
        ExecuteSpecialAttack();
        specialAttackTimer = specialAttackCooldown;
        
        // 특수 공격 후 일반 상태로 복귀
        state = MonsterState.Chase;
    }

    /// <summary>
    /// 돌진 상태 처리
    /// </summary>
    private void HandleCharging()
    {
        rb.linearVelocity = chargeDirection * chargeSpeed;
        chargeTimer -= Time.deltaTime;
        
        if (chargeTimer <= 0)
        {
            state = MonsterState.Chase;
        }
    }

    /// <summary>
    /// 기본 공격 실행
    /// </summary>
    private void BasicAttack()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage);
            Debug.Log($"Boss attacked player for {attackDamage} damage!");
        }
    }

    /// <summary>
    /// 특수 공격 실행
    /// </summary>
    private void ExecuteSpecialAttack()
    {
        switch (currentPattern)
        {
            case AttackPattern.Ranged:
                RangedAttack();
                break;
            case AttackPattern.Charge:
                ChargeAttack();
                break;
            case AttackPattern.MultiShot:
                MultiShotAttack();
                break;
            default:
                RangedAttack();
                break;
        }
    }

    /// <summary>
    /// 원거리 공격 (단일 투사체)
    /// </summary>
    private void RangedAttack()
    {
        if (projectilePrefab == null) return;
        
        Vector2 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb != null)
        {
            projectileRb.linearVelocity = direction * projectileSpeed;
        }
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        Destroy(projectile, 4f);
        Debug.Log("Boss used ranged attack!");
    }

    /// <summary>
    /// 돌진 공격
    /// </summary>
    private void ChargeAttack()
    {
        chargeDirection = (player.position - transform.position).normalized;
        chargeTimer = chargeDuration;
        state = MonsterState.Charging;
        
        Debug.Log("Boss started charge attack!");
    }

    /// <summary>
    /// 다중 발사 공격 (3방향)
    /// </summary>
    private void MultiShotAttack()
    {
        if (projectilePrefab == null) return;
        
        Vector2 baseDirection = (player.position - transform.position).normalized;
        
        // 3방향으로 발사 (-30도, 0도, +30도)
        for (int i = -1; i <= 1; i++)
        {
            float angleOffset = i * 30f;
            float radians = Mathf.Atan2(baseDirection.y, baseDirection.x) + (angleOffset * Mathf.Deg2Rad);
            Vector2 direction = new Vector2(Mathf.Cos(radians), Mathf.Sin(radians));
            
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            
            Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
            if (projectileRb != null)
            {
                projectileRb.linearVelocity = direction * projectileSpeed;
            }
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
            
            Destroy(projectile, 4f);
        }
        
        Debug.Log("Boss used multi-shot attack!");
    }

    /// <summary>
    /// 쿨다운 업데이트
    /// </summary>
    private void UpdateCooldowns()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
        }
        
        if (specialAttackTimer > 0)
        {
            specialAttackTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 몬스터가 데미지를 받는 메서드
    /// </summary>
    /// <param name="damage">받는 데미지 양</param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        // 피격 효과 재생
        if (hitEffect != null)
        {
            hitEffect.PlayHitEffect();
        }
        
        Debug.Log($"Boss took {damage} damage. Current Health: {currentHealth}/{maxHealth} ({HealthRatio:P0})");
        
        // 체력이 0 이하면 사망 처리
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// 몬스터 사망 처리
    /// </summary>
    private void Die()
    {
        Debug.Log("BOSS DEFEATED!");
        
        // 사망 이벤트 호출
        OnMonsterDeath?.Invoke();
        
        // GameManager에 몬스터 사망 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.BossDied();
        }
        
        Destroy(gameObject); // 1초 후 파괴 (연출용)
    }

    /// <summary>
    /// 충돌 처리
    /// </summary>
    /// <param name="other">충돌한 오브젝트</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController = other.GetComponent<PlayerController>();
            if (playerController != null)
            {
                // 돌진 중이면 더 큰 데미지
                int damage = state == MonsterState.Charging ? attackDamage * 2 : attackDamage;
                playerController.TakeDamage(damage);
            }
        }
    }

    /// <summary>
    /// 기즈모 그리기 (에디터에서 사거리 확인용)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // 공격 사거리
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        // 추적 사거리
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        // 체력 표시 (에디터에서만)
        #if UNITY_EDITOR
        if (Application.isPlaying)
        {
            Gizmos.color = Color.white;
            UnityEditor.Handles.Label(transform.position + Vector3.up * 2f, $"HP: {currentHealth}/{maxHealth}");
        }
        #endif
    }
}
}