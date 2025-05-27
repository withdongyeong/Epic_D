using Game4.Scripts.Character.Enemy;
using Game4.Scripts.Core;
using Game4.Scripts.VFX;

namespace Game4.Scripts.Character.Player
{
#pragma warning disable CS0618, CS0612, CS0672
using UnityEngine;
#pragma warning restore CS0618, CS0612, CS0672

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// 대시 거리
    /// </summary>
    private float dashDistance = 5f;

    /// <summary>
    /// 대시 타이머
    /// </summary>
    private float dashTimer = 0f;

    /// <summary>
    /// 대시 방향
    /// </summary>
    private Vector2 dashDirection;
    
    /// <summary>
    /// 플레이어 이동 속도
    /// </summary>
    private float moveSpeed = 10f;
    
    /// <summary>
    /// 플레이어 최대 체력
    /// </summary>
    private int maxHealth = 100;
    
    /// <summary>
    /// 현재 플레이어 체력
    /// </summary>
    private int currentHealth;
    
    /// <summary>
    /// 리지드바디 컴포넌트
    /// </summary>
    private Rigidbody2D rb;
    
    /// <summary>
    /// 플레이어 제어 가능 여부
    /// </summary>
    private bool canControl = true;
    
    /// <summary>
    /// 무적 상태 여부
    /// </summary>
    private bool isInvincible = false;
    
    /// <summary>
    /// 무적 지속 시간
    /// </summary>
    private float invincibilityDuration = 1f;
    
    /// <summary>
    /// 무적 타이머
    /// </summary>
    private float invincibilityTimer = 0f;

    // Properties
    /// <summary>
    /// 플레이어 이동 속도 프로퍼티
    /// </summary>
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    
    /// <summary>
    /// 플레이어 최대 체력 프로퍼티
    /// </summary>
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    
    /// <summary>
    /// 현재 플레이어 체력 프로퍼티
    /// </summary>
    public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    
    /// <summary>
    /// 플레이어 제어 가능 여부 프로퍼티
    /// </summary>
    public bool CanControl { get => canControl; set => canControl = value; }
    
    /// <summary>
    /// 무적 상태 여부 프로퍼티
    /// </summary>
    public bool IsInvincible { get => isInvincible; set => isInvincible = value; }

    /// <summary>
    /// 피격 효과 컴포넌트
    /// </summary>
    private HitEffect hitEffect;
    
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
    }

    /// <summary>
    /// 매 프레임 업데이트
    /// </summary>
    private void Update()
    {
        if (!canControl) return;
    
        HandleMovement(); // 대시 처리 제거
        HandleInvincibility();
    }

    /// <summary>
    /// 대시 시작 (순간이동으로 변경)
    /// </summary>
    /// <param name="direction">대시 방향</param>
    public void StartDash(Vector2 direction)
    {
        // 순간이동
        Vector3 dashTarget = transform.position + (Vector3)(direction.normalized * dashDistance);
        transform.position = dashTarget;
    
        Debug.Log($"Player dashed to {dashTarget}");
    }

    /// <summary>
    /// WASD 및 방향키를 사용하여 플레이어 이동 처리
    /// </summary>
    private void HandleMovement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
        rb.linearVelocity = movement * moveSpeed;
    }

    /// <summary>
    /// 무적 상태 처리
    /// </summary>
    private void HandleInvincibility()
    {
        if (isInvincible)
        {
            invincibilityTimer -= Time.deltaTime;
            if (invincibilityTimer <= 0)
            {
                isInvincible = false;
            }
        }
    }

    /// <summary>
    /// 플레이어가 데미지를 받는 메서드
    /// </summary>
    /// <param name="damage">받는 데미지 양</param>
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        
        currentHealth -= damage;
        
        // 무적 상태 적용
        isInvincible = true;
        invincibilityTimer = invincibilityDuration;
        
        Debug.Log($"Player took {damage} damage. Current Health: {currentHealth}");
        
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
    /// 플레이어 체력 완전 회복
    /// </summary>
    public void FullHeal()
    {
        currentHealth = maxHealth;
        isInvincible = false;
        invincibilityTimer = 0f;
        Debug.Log("Player fully healed!");
    }

    /// <summary>
    /// 플레이어 상태 초기화 (재시작용)
    /// </summary>
    public void ResetPlayer()
    {
        // 오브젝트 먼저 활성화
        gameObject.SetActive(true);
        
        // 상태 초기화
        currentHealth = maxHealth;
        canControl = true;
        isInvincible = false;
        invincibilityTimer = 0f;
        
        // 물리 상태 초기화
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // 투명도 복구
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f); // 완전 불투명
        }
        
        // 위치 초기화 (스폰 지점으로)
        transform.position = Vector3.zero; // 또는 원하는 스폰 위치
        
        Debug.Log($"Player reset! Health: {currentHealth}/{maxHealth}");
    }

    /// <summary>
    /// 플레이어 사망 처리
    /// </summary>
    private void Die()
    {
        canControl = false;
        rb.linearVelocity = Vector2.zero;
        
        Debug.Log("Player Died!");
        
        // GameManager에 플레이어 사망 알림
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayerDied();
        }
        
        // 플레이어 오브젝트는 비활성화하지 않고 투명하게 만들기
        // 검들이 플레이어 참조를 잃지 않도록 함
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0.3f); // 30% 투명도
        }
    }

    /// <summary>
    /// 충돌 처리
    /// </summary>
    /// <param name="other">충돌한 오브젝트</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            MonsterController monster = other.GetComponent<MonsterController>();
            if (monster != null)
            {
                TakeDamage(monster.AttackDamage);
            }
        }
    }
}
}