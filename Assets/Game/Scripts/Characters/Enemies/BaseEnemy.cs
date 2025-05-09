using System;
using UnityEngine;

namespace Game.Scripts.Characters.Enemies
{
    /// <summary>
    /// 모든 적 캐릭터의 기본 클래스
    /// </summary>
    public class BaseEnemy : MonoBehaviour
    {
        private int _health = 100;
        private bool _isDead;
    
        public int Health { get => _health; set => _health = value; }
        public bool IsDead { get => _isDead; }
        
        // 이벤트 정의
        public event Action OnEnemyDeath;
    
        /// <summary>
        /// 데미지 적용 및 사망 처리
        /// </summary>
        public virtual void TakeDamage(int damage)
        {
            if (_isDead) return;
        
            _health -= damage;
        
            if (_health <= 0)
            {
                Die();
            }
        }
    
        /// <summary>
        /// 사망 처리
        /// </summary>
        protected virtual void Die()
        {
            _isDead = true;
            OnEnemyDeath?.Invoke();
        }
    }
}