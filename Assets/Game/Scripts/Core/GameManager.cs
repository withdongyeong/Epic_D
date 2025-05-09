using Game.Scripts.Characters.Enemies;
using Game.Scripts.Characters.Player;
using Game.Scripts.Tiles;
using UnityEngine;

namespace Game.Scripts.Core
{
    /// <summary>
    /// 게임 전체 관리 클래스
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        public GameObject attackTilePrefab;
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
            
            InitializeGrid();
            SpawnPlayer();
            SpawnEnemy();
            
            // 게임 시작 상태로 설정
            _gameStateManager.StartGame();
        }
    
        private void InitializeGrid()
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