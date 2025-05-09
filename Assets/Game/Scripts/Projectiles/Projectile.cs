using Game.Scripts.Characters.Enemies;
using UnityEngine;

namespace Game.Scripts.Projectiles
{
    /// <summary>
    /// 기본 투사체 클래스
    /// </summary>
    public class Projectile : MonoBehaviour
    {
        private int damage = 10;
        private float speed = 12f;
        private Vector3 direction;
    
        public int Damage { get => damage; set => damage = value; }
        public float Speed { get => speed; set => speed = value; }
    
        /// <summary>
        /// 투사체 초기화
        /// </summary>
        public void Initialize(Vector3 dir)
        {
            direction = dir.normalized;
        }
    
        void Update()
        {
            transform.position += direction * speed * Time.deltaTime;
        
            // 화면 밖으로 나가면 제거
            if (Mathf.Abs(transform.position.x) > 20 || Mathf.Abs(transform.position.y) > 20)
            {
                Destroy(gameObject);
            }
        }
    
        /// <summary>
        /// 충돌 처리
        /// </summary>
        void OnTriggerEnter(Collider other)
        {
            Debug.Log($"충돌 감지: {other.gameObject.name}");
            BaseEnemy enemy = other.GetComponent<BaseEnemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
    }
}