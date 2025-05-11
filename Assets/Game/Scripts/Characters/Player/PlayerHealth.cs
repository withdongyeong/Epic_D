using UnityEngine;
using System;
using System.Collections;

namespace Game.Scripts.Characters.Player
{
    /// <summary>
    /// 플레이어 체력 관리 시스템
    /// </summary>
    public class PlayerHealth : MonoBehaviour
    {
        private int _maxHealth = 100;
        private int _currentHealth;
        private bool _isInvincible = false;
        private Coroutine _invincibilityCoroutine;
        
        public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
        
        public event Action<int> OnHealthChanged;
        public event Action OnPlayerDeath;
        public event Action<bool> OnInvincibilityChanged;
        
        /// <summary>
        /// 초기화
        /// </summary>
        private void Start()
        {
            _currentHealth = _maxHealth;
            
            // 초기 체력 상태 이벤트 발생
            OnHealthChanged?.Invoke(_currentHealth);
        }
        
        /// <summary>
        /// 데미지 처리
        /// </summary>
        public void TakeDamage(int damage)
        {
            // 무적 상태면 데미지 무시
            if (_isInvincible)
            {
                Debug.Log("무적 상태: 데미지 무시");
                return;
            }
            
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
        /// 무적 상태 설정
        /// </summary>
        /// <param name="invincible">무적 상태 여부</param>
        public void SetInvincible(bool invincible)
        {
            _isInvincible = invincible;
            
            // 무적 상태 변경 이벤트 발생
            OnInvincibilityChanged?.Invoke(_isInvincible);
            
            Debug.Log($"플레이어 무적 상태 변경: {_isInvincible}");
        }
        
        /// <summary>
        /// 지속시간이 있는 무적 상태 설정
        /// </summary>
        /// <param name="invincible">무적 상태 여부</param>
        /// <param name="duration">지속 시간(초)</param>
        public void SetInvincible(bool invincible, float duration)
        {
            // 이미 실행 중인 코루틴이 있다면 중지
            if (_invincibilityCoroutine != null)
            {
                StopCoroutine(_invincibilityCoroutine);
                _invincibilityCoroutine = null;
            }
            
            // 무적 상태 설정
            SetInvincible(invincible);
            
            // 지속 시간 설정
            if (invincible && duration > 0)
            {
                _invincibilityCoroutine = StartCoroutine(InvincibilityTimer(duration));
            }
        }
        
        /// <summary>
        /// 무적 상태 타이머
        /// </summary>
        private IEnumerator InvincibilityTimer(float duration)
        {
            Debug.Log($"무적 상태 시작: {duration}초 동안 지속");
            
            yield return new WaitForSeconds(duration);
            
            SetInvincible(false);
            _invincibilityCoroutine = null;
            
            Debug.Log("무적 상태 종료");
        }
        
        /// <summary>
        /// 사망 처리
        /// </summary>
        private void Die()
        {
            Debug.Log("플레이어 사망");
            OnPlayerDeath?.Invoke();
        }
        
        private void OnDestroy()
        {
            // 실행 중인 코루틴이 있다면 중지
            if (_invincibilityCoroutine != null)
            {
                StopCoroutine(_invincibilityCoroutine);
                _invincibilityCoroutine = null;
            }
        }
    }
}