using System;
using UnityEngine;

namespace Game.Scripts.Characters.Enemies
{
    /// <summary>
    /// 모든 적 캐릭터의 기본 클래스
    /// </summary>
    public abstract class BaseEnemy : MonoBehaviour
    {
        [Header("기본 속성")]
        public int maxHealth = 300;
        public float attackDelay = 0.5f;
        
        public event Action OnEnemyDeath;

        private int _currentHealth;
        public bool IsDead { get; protected set; }
        
        
        public int CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
        
        protected virtual void Awake()
        {
            _currentHealth = maxHealth;
        }
        
        /// <summary>
        /// 데미지 받기
        /// </summary>
        public virtual void TakeDamage(int damage)
        {
            if (IsDead) return;
            
            _currentHealth -= damage;
            
            if (_currentHealth <= 0)
            {
                Die();
            }
        }
        
        /// <summary>
        /// 사망 처리
        /// </summary>
        protected virtual void Die()
        {
            IsDead = true;
            OnEnemyDeath?.Invoke();
        }
        
        /// <summary>
        /// 메테오 공격을 트리거해야 하는지 확인
        /// </summary>
        public virtual bool ShouldTriggerMeteorAttack()
        {
            // 기본적으로 체력이 50% 이하일 때 메테오 공격 활성화
            return _currentHealth <= maxHealth * 0.5f;
        }
    }
}