namespace Game.Scripts.Characters.Enemies
{
    using UnityEngine;

    /// <summary>
    /// 모든 적 캐릭터의 기본 클래스
    /// </summary>
    public class BaseEnemy : MonoBehaviour
    {
        private int _health = 100;
        private bool _isDead;
    
        public int Health { get => _health; set => _health = value; }
        public bool IsDead { get => _isDead; }
    
        /// <summary>
        /// 데미지 적용 및 사망 처리
        /// </summary>
        public virtual void TakeDamage(int damage)
        {
            if (_isDead) return;
        
            _health -= damage;
            Debug.Log($"적 피격: {damage} 데미지, 남은 체력: {_health}");
        
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
            Debug.Log("적 사망");
        }
    }
}