using UnityEngine;
using System;

namespace Game.Scripts.Characters.Player
{
    /// <summary>
    /// 플레이어 체력 관리 시스템
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        private int _maxHealth = 100;
        private int _currentHealth;
        
        public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
        
        public event Action<int> OnHealthChanged;
        public event Action OnPlayerDeath;
        
        /// <summary>
        /// 초기화
        /// </summary>
        private void Start()
        {
            _currentHealth = _maxHealth;
        }
        
        /// <summary>
        /// 데미지 처리
        /// </summary>
        public void TakeDamage(int damage)
        {
            _currentHealth -= damage;
            _currentHealth = Mathf.Max(0, _currentHealth);
            
            OnHealthChanged?.Invoke(_currentHealth);
            
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// 회복 처리
        /// </summary>
        public void Heal(int amount)
        {
            _currentHealth += amount;
            _currentHealth = Mathf.Min(_maxHealth, _currentHealth);
            
            OnHealthChanged?.Invoke(_currentHealth);
        }
        
        /// <summary>
        /// 사망 처리
        /// </summary>
        private void Die()
        {
            Debug.Log("플레이어 사망");
            OnPlayerDeath?.Invoke();
        }
    }
}