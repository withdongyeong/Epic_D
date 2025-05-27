using Game4.Scripts.Character.Player;

namespace Game4.Scripts.Character.Enemy
{
#pragma warning disable CS0618, CS0612, CS0672
    using UnityEngine;
#pragma warning restore CS0618, CS0612, CS0672

    public class ProjectileController : MonoBehaviour
    {
        /// <summary>
        /// 투사체 데미지
        /// </summary>
        private int damage = 15;
    
        /// <summary>
        /// 투사체 수명 (초)
        /// </summary>
        private float lifetime = 4f;
    
        /// <summary>
        /// 생성 시간
        /// </summary>
        private float spawnTime;

        // Properties
        /// <summary>
        /// 투사체 데미지 프로퍼티
        /// </summary>
        public int Damage { get => damage; set => damage = value; }
    
        /// <summary>
        /// 투사체 수명 프로퍼티
        /// </summary>
        public float Lifetime { get => lifetime; set => lifetime = value; }

        /// <summary>
        /// 초기화
        /// </summary>
        private void Start()
        {
            spawnTime = Time.time;
        }

        /// <summary>
        /// 매 프레임 업데이트
        /// </summary>
        private void Update()
        {
            // 수명이 다하면 파괴
            if (Time.time - spawnTime >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// 충돌 처리
        /// </summary>
        /// <param name="other">충돌한 오브젝트</param>
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                // 플레이어에게 데미지
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.TakeDamage(damage);
                    Debug.Log($"Projectile hit player for {damage} damage!");
                }
            
                // 투사체 파괴
                Destroy(gameObject);
            }
        }
    }
}