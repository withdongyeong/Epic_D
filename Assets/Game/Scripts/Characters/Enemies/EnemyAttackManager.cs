using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Characters.Player;
using Game.Scripts.Core;
using Game.Scripts.Projectiles;

namespace Game.Scripts.Characters.Enemies
{
    /// <summary>
    /// 적 공격 관련 기능 관리 클래스
    /// </summary>
    public class EnemyAttackManager : MonoBehaviour
    {
        private GridSystem _gridSystem;
        private PlayerController _player;
        private PlayerHealth _playerHealth;
        private GameObject _warningTilePrefab;
        private GameObject _explosionEffectPrefab;
        private GameObject _projectilePrefab;
        private GameObject _meteorPrefab;
        private Transform _enemyTransform;
        
        /// <summary>
        /// 초기화
        /// </summary>
        public void Initialize(
            GridSystem gridSystem,
            PlayerController player,
            PlayerHealth playerHealth,
            GameObject warningTilePrefab,
            GameObject explosionEffectPrefab,
            GameObject projectilePrefab,
            GameObject meteorPrefab,
            Transform enemyTransform)
        {
            _gridSystem = gridSystem;
            _player = player;
            _playerHealth = playerHealth;
            _warningTilePrefab = warningTilePrefab;
            _explosionEffectPrefab = explosionEffectPrefab;
            _projectilePrefab = projectilePrefab;
            _meteorPrefab = meteorPrefab;
            _enemyTransform = enemyTransform;
        }
        
        /// <summary>
        /// 데미지 효과 생성
        /// </summary>
        public void CreateDamageEffect(Vector3 position)
        {
            if (_explosionEffectPrefab != null)
            {
                GameObject effect = Instantiate(_explosionEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 0.7f);
            }
        }
        
        /// <summary>
        /// 플레이어에게 데미지 적용 및 이펙트 표시
        /// </summary>
        public void ApplyDamageWithEffect(int damage)
        {
            if (_playerHealth != null)
            {
                _playerHealth.TakeDamage(damage);
                CreateDamageEffect(_player.transform.position);
            }
        }
        
        /// <summary>
        /// 경고 타일 생성
        /// </summary>
        public GameObject CreateWarningTile(Vector3 position)
        {
            return Instantiate(_warningTilePrefab, position, Quaternion.identity);
        }
        
        /// <summary>
        /// 투사체 발사
        /// </summary>
        public void FireProjectile(Vector3 direction)
        {
            GameObject projectileObj = Instantiate(_projectilePrefab, _enemyTransform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction.normalized, Projectile.ProjectileTeam.Enemy);
        }
        
        /// <summary>
        /// 메테오 공격 실행
        /// </summary>
        public IEnumerator ExecuteMeteorAttack(Vector3 targetPosition)
        {
            if (_meteorPrefab == null) yield break;
            
            // 메테오 생성 (적 위치 상단에서 시작)
            Vector3 spawnPosition = _enemyTransform.position + new Vector3(0, 10, 0);
            GameObject meteor = Instantiate(_meteorPrefab, spawnPosition, Quaternion.identity);
            
            // 경고 타일 생성 (맵 절반에 대해)
            List<GameObject> warningTiles = new List<GameObject>();
            List<Vector3> impactPositions = new List<Vector3>();
            
            for (int x = _gridSystem.Width / 2; x < _gridSystem.Width; x++)
            {
                for (int y = 0; y < _gridSystem.Height; y++)
                {
                    Vector3 pos = _gridSystem.GetWorldPosition(x, y);
                    impactPositions.Add(pos);
                    warningTiles.Add(CreateWarningTile(pos));
                }
            }
            
            // 메테오 움직임 애니메이션
            float duration = 2.0f;
            float elapsedTime = 0f;
            Vector3 startPos = meteor.transform.position;
            
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                // 포물선 움직임
                meteor.transform.position = Vector3.Lerp(startPos, targetPosition, t) + 
                                           Vector3.up * (1 - 4 * (t - 0.5f) * (t - 0.5f)) * 5f;
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // 메테오 충돌 효과
            meteor.transform.position = targetPosition;
            
            // 충격파 효과
            foreach (Vector3 pos in impactPositions)
            {
                CreateDamageEffect(pos);
            }
            
            // 메테오 파괴
            Destroy(meteor, 0.5f);
            
            // 플레이어가 영향 범위에 있는지 확인
            _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
            if (playerX >= _gridSystem.Width / 2)
            {
                ApplyDamageWithEffect(30);
            }
            
            // 경고 타일 제거
            foreach (GameObject tile in warningTiles)
            {
                Destroy(tile);
            }
        }
    }
}