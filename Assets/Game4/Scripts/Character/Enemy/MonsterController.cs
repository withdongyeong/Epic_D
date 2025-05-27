using System;
using Game4.Scripts.Character.Player;
using Game4.Scripts.Core;
using Game4.Scripts.VFX;

namespace Game4.Scripts.Character.Enemy
{
using UnityEngine;

public class MonsterController : MonoBehaviour, IMonster
{
    /// <summary>
    /// 몬스터 이동 속도
    /// </summary>
    private float moveSpeed = 4f;
    
    /// <summary>
    /// 몬스터 최대 체력
    /// </summary>
    private int maxHealth = 100;
    
    /// <summary>
    /// 현재 몬스터 체력
    /// </summary>
    private int currentHealth;
    
    /// <summary>
    /// 공격 데미지
    /// </summary>
    private int attackDamage = 10;
    
    /// <summary>
    /// 공격 사거리
    /// </summary>
    private float attackRange = 4f;
    
    /// <summary>
    /// 추적 사거리
    /// </summary>
    private float chaseRange = 1000f;
    
    /// <summary>
    /// 공격 쿨다운
    /// </summary>
    private float attackCooldown = 1f;
    
    /// <summary>
    /// 현재 공격 쿨다운 타이머
    /// </summary>
    private float attackCooldownTimer = 0f;
    
    /// <summary>
    /// 돌진 공격 속도
    /// </summary>
    private float chargeSpeed = 8f;
    
    /// <summary>
    /// 돌진 지속 시간
    /// </summary>
    private float chargeDuration = 1.5f;
    
    /// <summary>
    /// 현재 돌진 타이머
    /// </summary>
    private float chargeTimer = 0f;
    
    /// <summary>
    /// 돌진 방향
    /// </summary>
    private Vector2 chargeDirection;
    
    /// <summary>
    /// 돌진 중인지 여부
    /// </summary>
    private bool isCharging = false;
    
    /// <summary>
    /// 플레이어 참조
    /// </summary>
    private Transform player;
    
    /// <summary>
    /// 리지드바디 컴포넌트
    /// </summary>
    private Rigidbody2D rb;
    
    /// <summary>
    /// 피격 효과 컴포넌트
    /// </summary>
    private HitEffect hitEffect;
    
    /// <summary>
    /// 몬스터 상태
    /// </summary>
    private MonsterState state = MonsterState.Idle;

    /// <summary>
    /// 몬스터 사망 이벤트
    /// </summary>
    public event Action OnMonsterDeath;
    
    // Properties
    /// <summary>
    /// 몬스터 이동 속도 프로퍼티
    /// </summary>
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    
    /// <summary>
    /// 몬스터 최대 체력 프로퍼티
    /// </summary>
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    
    /// <summary>
    /// 현재 몬스터 체력 프로퍼티
    /// </summary>
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    
    /// <summary>
    /// 공격 데미지 프로퍼티
    /// </summary>
    public int AttackDamage { get => attackDamage; set => attackDamage = value; }
    
    /// <summary>
    /// 공격 사거리 프로퍼티
    /// </summary>
    public float AttackRange { get => attackRange; set => attackRange = value; }
    
    /// <summary>
    /// 추적 사거리 프로퍼티
    /// </summary>
    public float ChaseRange { get => chaseRange; set => chaseRange = value; }

    /// <summary>
    /// 몬스터 상태 열거형
    /// </summary>
    private enum MonsterState
    {
        Idle,
        Chase,
        Attack,
        Charging
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
        // 돌진 중이면 상태 변경하지 않음
        if (isCharging) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
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
    /// 공격 상태 처리
    /// </summary>
    private void HandleAttack()
    {
        rb.linearVelocity = Vector2.zero;
        
        if (attackCooldownTimer <= 0)
        {
            StartChargeAttack();
            attackCooldownTimer = attackCooldown;
        }
    }
    
    /// <summary>
    /// 돌진 공격 시작
    /// </summary>
    private void StartChargeAttack()
    {
        // 플레이어 방향으로 돌진 방향 설정
        chargeDirection = (player.position - transform.position).normalized;
        
        // 돌진 상태로 전환
        state = MonsterState.Charging;
        isCharging = true;
        chargeTimer = chargeDuration;
        
        Debug.Log("Monster started charge attack!");
    }
    
    /// <summary>
    /// 돌진 상태 처리
    /// </summary>
    private void HandleCharging()
    {
        // 돌진 이동
        rb.linearVelocity = chargeDirection * chargeSpeed;
        
        // 돌진 시간 감소
        chargeTimer -= Time.deltaTime;
        
        // 돌진 완료
        if (chargeTimer <= 0)
        {
            EndChargeAttack();
        }
    }
    
    /// <summary>
    /// 돌진 공격 종료
    /// </summary>
    private void EndChargeAttack()
    {
        isCharging = false;
        rb.linearVelocity = Vector2.zero;
        
        // 플레이어와의 거리에 따라 상태 재설정
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            state = MonsterState.Attack;
            // 돌진 후 즉시 데미지 적용
            Attack();
        }
        else if (distanceToPlayer <= chaseRange)
        {
            state = MonsterState.Chase;
        }
        else
        {
            state = MonsterState.Idle;
        }
        
        Debug.Log("Monster finished charge attack!");
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
    /// 플레이어 공격
    /// </summary>
    private void Attack()
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.TakeDamage(attackDamage);
            Debug.Log($"Monster attacked player for {attackDamage} damage!");
        }
    }

    /// <summary>
    /// 몬스터가 데미지를 받는 메서드
    /// </summary>
    /// <param name="damage">받는 데미지 양</param>
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        
        Debug.Log($"Monster took {damage} damage. Current Health: {currentHealth}");
        
        // 피격 효과 재생
        if (hitEffect != null)
        {
            hitEffect.PlayHitEffect();
        }
        
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
        Debug.Log("Monster Died!");
        
        // 사망 이벤트 호출
        OnMonsterDeath?.Invoke();
        
        // GameManager에 몬스터 사망 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MonsterDied();
        }
        
        // 사망 이펙트나 아이템 드롭 등은 나중에 추가
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
    }
}
}