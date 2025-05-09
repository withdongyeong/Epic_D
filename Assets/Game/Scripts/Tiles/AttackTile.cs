using Game.Scripts.Characters.Enemies;
using Game.Scripts.Projectiles;
using UnityEngine;

namespace Game.Scripts.Tiles  
{
    public class AttackTile : BaseTile
    {
        private int _damage = 10;
        public GameObject projectilePrefab;
        private BaseEnemy targetEnemy;
        
        public int Damage { get => _damage; set => _damage = value; }
        
        private void Start()
        {
            targetEnemy = FindAnyObjectByType<BaseEnemy>();
        }
        
        /// <summary>
        /// 타일 발동 - 투사체 발사
        /// </summary>
        public override void Activate()
        {
            base.Activate();
            if (GetState() == TileState.Activated && targetEnemy != null)
            {
                FireProjectile();
            }
        }
        /// <summary>
        /// 투사체 생성 및 발사
        /// </summary>
        private void FireProjectile()
        {
            if (projectilePrefab != null)
            {
                Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
                GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Projectile projectile = projectileObj.GetComponent<Projectile>();
                projectile.Initialize(direction);
            }
        }
    }   
}