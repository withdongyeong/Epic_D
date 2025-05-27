namespace Game4.Scripts.Core
{
    using UnityEngine;
using System;

public class HealthSystem : MonoBehaviour
{
    /// <summary>
    /// 최대 체력
    /// </summary>
    private int maxHealth = 100;
    
    /// <summary>
    /// 현재 체력
    /// </summary>
    private int currentHealth;
    
    /// <summary>
    /// 체력 변화 이벤트
    /// </summary>
    public event Action<int, int> OnHealthChanged; // (currentHealth, maxHealth)
    
    /// <summary>
    /// 사망 이벤트
    /// </summary>
    public event Action OnDeath;

    // Properties
    /// <summary>
    /// 최대 체력 프로퍼티
    /// </summary>
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    
    /// <summary>
    /// 현재 체력 프로퍼티
    /// </summary>
    public int CurrentHealth { get => currentHealth; private set => currentHealth = value; }
    
    /// <summary>
    /// 체력 비율 프로퍼티 (0.0 ~ 1.0)
    /// </summary>
    public float HealthRatio => maxHealth > 0 ? (float)currentHealth / maxHealth : 0f;
    
    /// <summary>
    /// 생존 상태 프로퍼티
    /// </summary>
    public bool IsAlive => currentHealth > 0;

    /// <summary>
    /// 게임 시작 시 체력 초기화
    /// </summary>
    private void Start()
    {
        InitializeHealth();
    }

    /// <summary>
    /// 체력 시스템 초기화
    /// </summary>
    public void InitializeHealth()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// 체력 초기화 (특정 값으로)
    /// </summary>
    /// <param name="health">설정할 체력 값</param>
    public void InitializeHealth(int health)
    {
        maxHealth = health;
        currentHealth = health;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// 데미지를 받는 메서드
    /// </summary>
    /// <param name="damage">받을 데미지 양</param>
    /// <returns>실제로 받은 데미지 양</returns>
    public int TakeDamage(int damage)
    {
        if (!IsAlive || damage <= 0) return 0;
        
        int actualDamage = Mathf.Min(damage, currentHealth);
        currentHealth -= actualDamage;
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
        
        return actualDamage;
    }

    /// <summary>
    /// 체력을 회복하는 메서드
    /// </summary>
    /// <param name="healAmount">회복할 체력 양</param>
    /// <returns>실제로 회복된 체력 양</returns>
    public int Heal(int healAmount)
    {
        if (!IsAlive || healAmount <= 0) return 0;
        
        int actualHeal = Mathf.Min(healAmount, maxHealth - currentHealth);
        currentHealth += actualHeal;
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        return actualHeal;
    }

    /// <summary>
    /// 체력을 특정 값으로 설정
    /// </summary>
    /// <param name="health">설정할 체력 값</param>
    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    /// <summary>
    /// 최대 체력 설정 및 현재 체력 조정
    /// </summary>
    /// <param name="newMaxHealth">새로운 최대 체력</param>
    /// <param name="adjustCurrentHealth">현재 체력을 비율에 맞게 조정할지 여부</param>
    public void SetMaxHealth(int newMaxHealth, bool adjustCurrentHealth = false)
    {
        if (newMaxHealth <= 0) return;
        
        if (adjustCurrentHealth && maxHealth > 0)
        {
            float ratio = HealthRatio;
            maxHealth = newMaxHealth;
            currentHealth = Mathf.RoundToInt(maxHealth * ratio);
        }
        else
        {
            maxHealth = newMaxHealth;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
        }
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            OnDeath?.Invoke();
        }
    }

    /// <summary>
    /// 체력을 완전히 회복
    /// </summary>
    public void FullHeal()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    /// <summary>
    /// 체력 정보를 문자열로 반환
    /// </summary>
    /// <returns>체력 정보 문자열</returns>
    public override string ToString()
    {
        return $"Health: {currentHealth}/{maxHealth} ({HealthRatio:P0})";
    }
}
}