using Game.Scripts.Characters.Enemies;
using Game.Scripts.Characters.Player;
using Game.Scripts.Tiles;
using Game.Scripts.Building;
using UnityEngine;
using System.Collections.Generic;

namespace Game.Scripts.Core
{
    /// <summary>
    /// 게임 전체 관리 클래스
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        public GameObject attackTilePrefab;
        public GameObject defenseTilePrefab;
        public GameObject buffTilePrefab;
        public GameObject enemyPrefab;
        
        private GridSystem _gridSystem;
        private BaseEnemy _enemy;
        private PlayerController _player;
        private PlayerHealth _playerHealth;
        private GameStateManager _gameStateManager;
        
        void Start()
        {
            _gridSystem = GetComponent<GridSystem>();
            _gameStateManager = GameStateManager.Instance;
            
            // 빌딩 씬에서 배치한 타일 생성
            if (BuildingManager.PlacedTiles.Count > 0)
            {
                CreateTilesFromBuildingData();
            }
            else
            {
                // 기본 타일 배치 (빌딩 데이터가 없을 경우)
                InitializeDefaultGrid();
            }
            
            SpawnPlayer();
            SpawnEnemy();
            
            // 게임 시작 상태로 설정
            _gameStateManager.StartGame();
        }
        
        /// <summary>
        /// 빌딩 씬 데이터 기반으로 타일 생성
        /// </summary>
        private void CreateTilesFromBuildingData()
        {
            foreach (TilePlacementData placementData in BuildingManager.PlacedTiles)
            {
                TileData tileData = placementData.tileData;
                GameObject tilePrefab = GetTilePrefabByType(tileData.type);
                
                if (tilePrefab != null)
                {
                    // 회전 상태에 따른 실제 크기 계산
                    int width = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.height : tileData.width;
                    int height = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.width : tileData.height;
                    
                    // 타일 원점(좌상단) 위치
                    int startX = placementData.x;
                    int startY = placementData.y;
                    
                    // 타일 중심 위치 계산
                    Vector3 centerPos = _gridSystem.GetWorldPosition(startX, startY);
                    
                    // 타일 생성
                    GameObject tileObj = Instantiate(tilePrefab, centerPos, Quaternion.Euler(0, 0, tileData.rotation));
                    BaseTile tile = tileObj.GetComponent<BaseTile>();
                    
                    // 타일 속성 설정
                    if (tile is AttackTile attackTile)
                    {
                        attackTile.Damage = tileData.damage;
                    }
                    
                    // 그리드에 타일 등록 (해당 타일이 차지하는 모든 셀)
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            _gridSystem.RegisterTile(tile, startX + x, startY + y);
                        }
                    }
                }
            }
            
            Debug.Log($"빌딩 데이터에서 {BuildingManager.PlacedTiles.Count}개 타일 생성 완료");
        }
        
        /// <summary>
        /// 타일 타입에 따른 프리팹 반환
        /// </summary>
        private GameObject GetTilePrefabByType(TileData.TileType type)
        {
            switch (type)
            {
                case TileData.TileType.Attack:
                    return attackTilePrefab;
                case TileData.TileType.Defense:
                    return defenseTilePrefab;
                case TileData.TileType.Buff:
                    return buffTilePrefab;
                default:
                    return attackTilePrefab;
            }
        }
    
        /// <summary>
        /// 기본 그리드 초기화 (빌딩 데이터가 없을 경우)
        /// </summary>
        private void InitializeDefaultGrid()
        {
            // 테스트용 타일 배치
            SpawnTile(attackTilePrefab, 2, 2);
            SpawnTile(attackTilePrefab, 4, 3);
            SpawnTile(attackTilePrefab, 1, 5);
        }
    
        /// <summary>
        /// 타일 생성 및 그리드 등록
        /// </summary>
        private void SpawnTile(GameObject tilePrefab, int x, int y)
        {
            Vector3 position = _gridSystem.GetWorldPosition(x, y);
            GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity);
            BaseTile tile = tileObj.GetComponent<BaseTile>();
            _gridSystem.RegisterTile(tile, x, y);
        }
        
        /// <summary>
        /// 플레이어 캐릭터 생성
        /// </summary>
        private void SpawnPlayer()
        {
            Vector3 position = _gridSystem.GetWorldPosition(0, 0);
            GameObject playerObj = Instantiate(playerPrefab, position, Quaternion.identity);
            _player = playerObj.GetComponent<PlayerController>();
            _playerHealth = playerObj.GetComponent<PlayerHealth>();
            
            // 플레이어 사망 이벤트 연결
            if (_playerHealth != null)
            {
                _playerHealth.OnPlayerDeath += HandlePlayerDeath;
            }
        }
        
        /// <summary>
        /// 적 캐릭터 생성
        /// </summary>
        private void SpawnEnemy()
        {
            Vector3 enemyPosition = new Vector3(15f, 4f, 0f); // 오른쪽 위치
            GameObject enemyObj = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
            _enemy = enemyObj.GetComponent<BaseEnemy>();
            
            // 적 사망 이벤트 연결
            if (_enemy != null)
            {
                _enemy.OnEnemyDeath += HandleEnemyDeath;
            }
        }
        
        /// <summary>
        /// 플레이어 사망 처리
        /// </summary>
        private void HandlePlayerDeath()
        {
            _gameStateManager.LoseGame();
        }
        
        /// <summary>
        /// 적 사망 처리
        /// </summary>
        private void HandleEnemyDeath()
        {
            _gameStateManager.WinGame();
        }
        
        private void OnDestroy()
        {
            // 이벤트 연결 해제
            if (_playerHealth != null)
            {
                _playerHealth.OnPlayerDeath -= HandlePlayerDeath;
            }
            
            if (_enemy != null)
            {
                _enemy.OnEnemyDeath -= HandleEnemyDeath;
            }
        }
    }   
}