using Game.Scripts.Characters.Enemies;
using Game.Scripts.Tiles;
using UnityEngine;

namespace Game.Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        public GameObject playerPrefab;
        public GameObject attackTilePrefab;
        private GridSystem _gridSystem;
        public GameObject enemyPrefab;
        private BaseEnemy _enemy;
    
        private void Start()
        {
            _gridSystem = GetComponent<GridSystem>();
            InitializeGrid();
            SpawnPlayer();
            SpawnEnemy();
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
            Instantiate(playerPrefab, position, Quaternion.identity);
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
    }   
}