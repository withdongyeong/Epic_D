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
        private GameStateManager _gameStateManager;
        
        private void Awake()
        {
            _gameStateManager = GameStateManager.Instance;
            _gameStateManager.OnGameStateChanged += HandleGameStateChanged;
        }
    
        private void Start()
        {
            _gridSystem = GetComponent<GridSystem>();
            InitializeGrid();
        }
    
        /// <summary>
        /// 게임 상태 변경 처리
        /// </summary>
        private void HandleGameStateChanged(GameStateManager.GameState newState)
        {
            switch (newState)
            {
                case GameStateManager.GameState.MainMenu:
                    // 게임 요소 비활성화
                    CleanupGameElements();
                    break;
                    
                case GameStateManager.GameState.Playing:
                    // 게임 요소 초기화 및 활성화
                    SetupGame();
                    break;
                    
                case GameStateManager.GameState.Victory:
                case GameStateManager.GameState.Defeat:
                    // 필요한 경우 게임 요소 정리
                    break;
            }
        }
        
        /// <summary>
        /// 게임 시작 시 필요한 설정
        /// </summary>
        private void SetupGame()
        {
            CleanupGameElements(); // 기존 요소 정리
            SpawnPlayer();
            SpawnEnemy();
        }
        
        /// <summary>
        /// 게임 요소 정리
        /// </summary>
        private void CleanupGameElements()
        {
            // 기존 플레이어 제거
            if (_player != null)
            {
                Destroy(_player.gameObject);
                _player = null;
            }
            
            // 기존 적 제거
            if (_enemy != null)
            {
                Destroy(_enemy.gameObject);
                _enemy = null;
            }
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
        }
        
        /// <summary>
        /// 적 캐릭터 생성
        /// </summary>
        private void SpawnEnemy()
        {
            Vector3 enemyPosition = new Vector3(15f, 4f, 0f); // 오른쪽 위치
            GameObject enemyObj = Instantiate(enemyPrefab, enemyPosition, Quaternion.identity);
            _enemy = enemyObj.GetComponent<BaseEnemy>();
        }
        
        private void OnDestroy()
        {
            if (_gameStateManager != null)
            {
                _gameStateManager.OnGameStateChanged -= HandleGameStateChanged;
            }
        }
    }   
}