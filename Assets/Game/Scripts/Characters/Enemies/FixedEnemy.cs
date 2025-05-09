using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Scripts.Characters.Player;
using Game.Scripts.Core;
using Game.Scripts.Projectiles;

namespace Game.Scripts.Characters.Enemies
{
    /// <summary>
    /// 고정 위치 적 캐릭터 구현
    /// </summary>
    public class FixedEnemy : BaseEnemy
    {
        private float _attackCooldown = 2f;
        private int _currentPattern;
        private GridSystem _gridSystem;
        private PlayerController _player;
        private PlayerHealth _playerHealth;
        private float _patternCooldown = 0.5f; // 패턴 간 딜레이
        
        public GameObject projectilePrefab;
        public GameObject warningTilePrefab;
        
        public float AttackCooldown { get => _attackCooldown; set => _attackCooldown = value; }
        
        void Start()
        {
            _gridSystem = FindAnyObjectByType<GridSystem>();
            _player = FindAnyObjectByType<PlayerController>();
            _playerHealth = _player.GetComponent<PlayerHealth>();
            
            StartCoroutine(AttackRoutine());
        }
        
        /// <summary>
        /// 공격 패턴 실행 루틴
        /// </summary>
        private IEnumerator AttackRoutine()
        {
            yield return new WaitForSeconds(1f); // 초반 딜레이 감소
        
            while (!IsDead)
            {
                ExecutePattern();
                yield return new WaitForSeconds(_patternCooldown); // 패턴 간 딜레이 감소
            }
        }
        
        /// <summary>
        /// 현재 패턴 실행
        /// </summary>
        private void ExecutePattern()
        {
            switch (Random.Range(0, 8)) // 8개 패턴
            {
                case 0:
                    StartCoroutine(ShootProjectile());
                    break;
                case 1:
                    StartCoroutine(AreaAttack());
                    break;
                case 2:
                    StartCoroutine(HalfGridAttack());
                    break;
                case 3:
                    StartCoroutine(RapidFirePattern());
                    break;
                case 4:
                    StartCoroutine(CrossAttackPattern());
                    break;
                case 5:
                    StartCoroutine(MultiAreaAttack());
                    break;
                case 6:
                    StartCoroutine(DiagonalAttackPattern());
                    break;
                case 7:
                    StartCoroutine(DiagonalCrossPattern());
                    break;
            }
        }
        
        /// <summary>
        /// 패턴 1: 투사체 발사
        /// </summary>
        private IEnumerator ShootProjectile()
        {
            Debug.Log("적: 투사체 발사");
            
            if (projectilePrefab != null && _player != null)
            {
                Vector3 direction = (_player.transform.position - transform.position).normalized;
                GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Projectile projectile = projectileObj.GetComponent<Projectile>();
                projectile.Initialize(direction, Projectile.ProjectileTeam.Enemy);
            }
            
            yield return null;
        }
        
        /// <summary>
        /// 패턴 2: 플레이어 위치 범위 공격
        /// </summary>
        private IEnumerator AreaAttack()
        {
            Debug.Log("적: 영역 공격 준비");
            
            // 플레이어 위치 가져오기
            int playerX, playerY;
            _gridSystem.GetXY(_player.transform.position, out playerX, out playerY);
            
            // 경고 타일 표시 (3x3 영역)
            GameObject[] warningTiles = new GameObject[9]; // 3x3 = 9
            int index = 0;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int tileX = playerX + x;
                    int tileY = playerY + y;
            
                    if (_gridSystem.IsValidPosition(tileX, tileY))
                    {
                        Vector3 tilePos = _gridSystem.GetWorldPosition(tileX, tileY);
                        warningTiles[index++] = Instantiate(warningTilePrefab, tilePos, Quaternion.identity);
                    }
                }
            }
            
            // 경고 대기
            yield return new WaitForSeconds(1.5f);
            
            // 공격 실행
            Debug.Log("적: 영역 공격 발동");
            
            // 플레이어가 영역 내에 있는지 확인
            _gridSystem.GetXY(_player.transform.position, out int currentX, out int currentY);
            if (Mathf.Abs(currentX - playerX) <= 1 && Mathf.Abs(currentY - playerY) <= 1)
            {
                _playerHealth.TakeDamage(15);
            }
            
            // 경고 타일 제거
            foreach (GameObject tile in warningTiles)
            {
                if (tile != null)
                {
                    Destroy(tile);
                }
            }
        }
        
        /// <summary>
        /// 패턴 3: 그리드 절반 공격
        /// </summary>
        private IEnumerator HalfGridAttack()
        {
            Debug.Log("적: 광역 공격 준비");
            
            // 공격할 절반 결정 (가로 또는 세로)
            bool isHorizontal = Random.value > 0.5f;
            int splitValue = _gridSystem.Height / 2; 
            
            // 경고 타일 생성
            GameObject[] warningTiles = new GameObject[_gridSystem.Width * _gridSystem.Height / 2];
            int index = 0;
            
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                for (int y = 0; y < _gridSystem.Height; y++)
                {
                    bool shouldAttack = isHorizontal ? y < splitValue : x < splitValue;
                    
                    if (shouldAttack)
                    {
                        Vector3 tilePos = _gridSystem.GetWorldPosition(x, y);
                        warningTiles[index] = Instantiate(warningTilePrefab, tilePos, Quaternion.identity);
                        index++;
                    }
                }
            }
            
            // 경고 대기
            yield return new WaitForSeconds(2f);
            
            // 공격 실행
            Debug.Log("적: 광역 공격 발동");
            
            // 플레이어 위치 확인
            _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
            bool isPlayerInArea = isHorizontal ? playerY < splitValue : playerX < splitValue;
            
            if (isPlayerInArea)
            {
                _playerHealth.TakeDamage(20);
            }
            
            // 경고 타일 제거
            foreach (GameObject tile in warningTiles)
            {
                if (tile != null)
                {
                    Destroy(tile);
                }
            }
        }
        
        /// <summary>
        /// 패턴 4 : 연속 투사체 발사
        /// </summary>
        private IEnumerator RapidFirePattern()
        {
            Debug.Log("적: 연속 발사");
        
            for (int i = 0; i < 5; i++)
            {
                Vector3 direction = (_player.transform.position - transform.position).normalized;
                GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Projectile projectile = projectileObj.GetComponent<Projectile>();
                projectile.Initialize(direction, Projectile.ProjectileTeam.Enemy);
            
                yield return new WaitForSeconds(0.2f);
            }
        }
    
        /// <summary>
        /// 패턴 5 : 십자 공격
        /// </summary>
        private IEnumerator CrossAttackPattern()
        {
            Debug.Log("적: 십자 공격 준비");
        
            // 플레이어 위치
            _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
        
            // 십자 경고 생성
            List<GameObject> warningTiles = new List<GameObject>();
        
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                Vector3 pos = _gridSystem.GetWorldPosition(x, playerY);
                warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
            }
        
            for (int y = 0; y < _gridSystem.Height; y++)
            {
                if (y != playerY) // 중복 방지
                {
                    Vector3 pos = _gridSystem.GetWorldPosition(playerX, y);
                    warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
                }
            }
        
            yield return new WaitForSeconds(0.8f);
        
            // 공격 실행
            _gridSystem.GetXY(_player.transform.position, out int currentX, out int currentY);
            if (currentX == playerX || currentY == playerY)
            {
                _playerHealth.TakeDamage(15);
            }
        
            // 제거
            foreach (GameObject tile in warningTiles)
            {
                Destroy(tile);
            }
        }
        
        /// <summary>
        /// 패턴 6 : 연속 영역 공격
        /// </summary>
        private IEnumerator MultiAreaAttack()
        {
            Debug.Log("적: 연속 영역 공격");
    
            for (int i = 0; i < 3; i++)
            {
                // 랜덤 위치 선택
                int targetX = Random.Range(0, _gridSystem.Width);
                int targetY = Random.Range(0, _gridSystem.Height);
        
                // 경고 타일 생성 (2x2 영역)
                List<GameObject> warningTiles = new List<GameObject>();
        
                for (int x = 0; x < 2; x++)
                {
                    for (int y = 0; y < 2; y++)
                    {
                        int tileX = targetX + x;
                        int tileY = targetY + y;
                
                        if (_gridSystem.IsValidPosition(tileX, tileY))
                        {
                            Vector3 tilePos = _gridSystem.GetWorldPosition(tileX, tileY);
                            warningTiles.Add(Instantiate(warningTilePrefab, tilePos, Quaternion.identity));
                        }
                    }
                }
        
                yield return new WaitForSeconds(0.6f);
        
                // 공격 실행
                _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
                bool isHit = playerX >= targetX && playerX < targetX + 2 && 
                             playerY >= targetY && playerY < targetY + 2;
        
                if (isHit)
                {
                    _playerHealth.TakeDamage(10);
                }
        
                // 경고 타일 제거
                foreach (GameObject tile in warningTiles)
                {
                    Destroy(tile);
                }
        
                yield return new WaitForSeconds(0.3f);
            }
        }
        
        /// <summary>
        /// 패턴 7 : 대각선 공격
        /// </summary>
        private IEnumerator DiagonalAttackPattern()
        {
            Debug.Log("적: 대각선 공격 준비");
            
            // 플레이어 위치
            _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
            
            // 대각선 경고 생성
            List<GameObject> warningTiles = new List<GameObject>();
            
            // 우상단-좌하단 대각선
            int offset = playerY - playerX;
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                int y = x + offset;
                if (_gridSystem.IsValidPosition(x, y))
                {
                    Vector3 pos = _gridSystem.GetWorldPosition(x, y);
                    warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
                }
            }
            
            // 좌상단-우하단 대각선
            offset = playerY + playerX;
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                int y = offset - x;
                if (_gridSystem.IsValidPosition(x, y))
                {
                    Vector3 pos = _gridSystem.GetWorldPosition(x, y);
                    warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
                }
            }
            
            yield return new WaitForSeconds(0.8f);
            
            // 공격 실행
            _gridSystem.GetXY(_player.transform.position, out int currentX, out int currentY);
            
            // 우상단-좌하단 대각선 검사
            bool isOnDiagonal1 = (currentY - currentX) == (playerY - playerX);
            
            // 좌상단-우하단 대각선 검사
            bool isOnDiagonal2 = (currentY + currentX) == (playerY + playerX);
            
            if (isOnDiagonal1 || isOnDiagonal2)
            {
                _playerHealth.TakeDamage(15);
            }
            
            // 제거
            foreach (GameObject tile in warningTiles)
            {
                Destroy(tile);
            }
        }
        
        /// <summary>
        /// 패턴 8 : 대각선 후 십자 공격 패턴
        /// </summary>
        private IEnumerator DiagonalCrossPattern()
        {
            Debug.Log("적: 대각선-십자 연속 공격 준비");
            
            // 플레이어 위치
            _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
            
            // 1단계: 대각선 경고 생성
            List<GameObject> warningTiles = new List<GameObject>();
            
            // 우상단-좌하단 대각선
            int offset = playerY - playerX;
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                int y = x + offset;
                if (_gridSystem.IsValidPosition(x, y))
                {
                    Vector3 pos = _gridSystem.GetWorldPosition(x, y);
                    warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
                }
            }
            
            yield return new WaitForSeconds(0.8f);
            
            // 대각선 공격 실행
            _gridSystem.GetXY(_player.transform.position, out int currentX, out int currentY);
            bool isOnDiagonal = (currentY - currentX) == (playerY - playerX);
            
            if (isOnDiagonal)
            {
                _playerHealth.TakeDamage(10);
            }
            
            // 경고 타일 제거
            foreach (GameObject tile in warningTiles)
            {
                Destroy(tile);
            }
            
            // 새로운 플레이어 위치 가져오기
            _gridSystem.GetXY(_player.transform.position, out playerX, out playerY);
            
            // 2단계: 십자 경고 생성
            warningTiles = new List<GameObject>();
            
            yield return new WaitForSeconds(0.3f);
            
            // 가로 방향
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                Vector3 pos = _gridSystem.GetWorldPosition(x, playerY);
                warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
            }
            
            yield return new WaitForSeconds(0.8f);
            
            // 십자 공격 실행
            _gridSystem.GetXY(_player.transform.position, out currentX, out currentY);
            if (currentY == playerY)
            {
                _playerHealth.TakeDamage(15);
            }
            
            // 타일 제거
            foreach (GameObject tile in warningTiles)
            {
                Destroy(tile);
            }
        }
    }
}