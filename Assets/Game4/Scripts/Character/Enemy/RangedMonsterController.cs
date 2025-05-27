using Game4.Scripts.Core;
using Game4.Scripts.VFX;

namespace Game4.Scripts.Character.Enemy
{
#pragma warning disable CS0618, CS0612, CS0672
using UnityEngine;
using System;
#pragma warning restore CS0618, CS0612, CS0672

public class RangedMonsterController : MonoBehaviour, IMonster
{
    /// <summary>
    /// 몬스터 이동 속도
    /// </summary>
    private float moveSpeed = 3f;
    
    /// <summary>
    /// 몬스터 최대 체력
    /// </summary>
    private int maxHealth = 40;
    
    /// <summary>
    /// 현재 몬스터 체력
    /// </summary>
    private int currentHealth;
    
    /// <summary>
    /// 공격 데미지
    /// </summary>
    private int attackDamage = 8;
    
    /// <summary>
    /// 공격 사거리
    /// </summary>
    private float attackRange = 8f;
    
    /// <summary>
    /// 추적 사거리
    /// </summary>
    private float chaseRange = 8000f;
    
    /// <summary>
    /// 유지하려는 거리 (너무 가까우면 후퇴)
    /// </summary>
    private float preferredDistance = 6f;
    
    /// <summary>
    /// 공격 쿨다운
    /// </summary>
    private float attackCooldown = 2f;
    
    /// <summary>
    /// 현재 공격 쿨다운 타이머
    /// </summary>
    private float attackCooldownTimer = 0f;
    
    /// <summary>
    /// 투사체 프리팹
    /// </summary>
    public GameObject projectilePrefab;
    
    /// <summary>
    /// 투사체 속도
    /// </summary>
    private float projectileSpeed = 9f;
    
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
    /// 몬스터 상태 열거형
    /// </summary>
    private enum MonsterState
    {
        Idle,
        Chase,
        Attack,
        Retreat
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
        
        if (player == null)
        {
            Debug.LogError("Player not found! Make sure Player has 'Player' tag.");
        }
    }

    /// <summary>
    /// 매 프레임 업데이트
    /// </summary>
    private void Update()
    {
        if (player == null) return;
        
        UpdateState();
        HandleBehavior();
        UpdateCooldown();
    }

    /// <summary>
    /// 플레이어와의 거리에 따라 상태 업데이트
    /// </summary>
    private void UpdateState()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer > chaseRange)
        {
            state = MonsterState.Idle;
        }
        else if (distanceToPlayer < preferredDistance)
        {
            state = MonsterState.Retreat;
        }
        else if (distanceToPlayer <= attackRange && attackCooldownTimer <= 0)
        {
            state = MonsterState.Attack;
        }
        else
        {
            state = MonsterState.Chase;
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
            case MonsterState.Retreat:
                HandleRetreat();
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
    /// 공격 상태 처리
    /// </summary>
    private void HandleAttack()
    {
        rb.linearVelocity = Vector2.zero;
        
        if (attackCooldownTimer <= 0)
        {
            FireProjectile();
            attackCooldownTimer = attackCooldown;
        }
    }

    /// <summary>
    /// 후퇴 상태 처리
    /// </summary>
    private void HandleRetreat()
    {
        Vector2 direction = (transform.position - player.position).normalized;
        rb.linearVelocity = direction * moveSpeed * 0.8f; // 후퇴 시 약간 느리게
        
        // 플레이어 방향으로 회전 (공격 가능하도록)
        Vector2 lookDirection = (player.position - transform.position).normalized;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    /// <summary>
    /// 투사체 발사
    /// </summary>
    private void FireProjectile()
    {
        if (projectilePrefab == null) return;
        
        Vector2 direction = (player.position - transform.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        
        // 투사체에 방향과 속도 설정
        Rigidbody2D projectileRb = projectile.GetComponent<Rigidbody2D>();
        if (projectileRb != null)
        {
            projectileRb.linearVelocity = direction * projectileSpeed;
        }
        
        // 투사체 회전
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // 3초 후 투사체 파괴
        Destroy(projectile, 3f);
        
        Debug.Log("Ranged monster fired projectile!");
    }

    /// <summary>
    /// 공격 쿨다운 업데이트
    /// </summary>
    private void UpdateCooldown()
    {
        if (attackCooldownTimer > 0)
        {
            attackCooldownTimer -= Time.deltaTime;
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
        
        Debug.Log($"Ranged monster took {damage} damage. Current Health: {currentHealth}");
        
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
        Debug.Log("Ranged Monster Died!");
        
        // 사망 이벤트 호출
        OnMonsterDeath?.Invoke();
        
        // GameManager에 몬스터 사망 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MonsterDied();
        }
        
        Destroy(gameObject);
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
        
        // 선호 거리
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, preferredDistance);
    }
}
}