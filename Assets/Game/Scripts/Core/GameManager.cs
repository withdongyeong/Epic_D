using Game.Scripts.Characters.Enemies;
using Game.Scripts.Characters.Player;
using Game.Scripts.Tiles;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Game.Scripts.Inventory;

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
        public GameObject healTilePrefab;
        public GameObject enemyPrefab;
        public TextMeshProUGUI countdownText;
        public float countdownDuration = 3f;
        
        private GridSystem _gridSystem;
        private BaseEnemy _enemy;
        private PlayerController _player;
        private PlayerHealth _playerHealth;
        private GameStateManager _gameStateManager;
        public GameObject highlightTilePrefab;
        private bool _gameStarted = false;
        
        void Start()
        {
            _gridSystem = GetComponent<GridSystem>();
            _gameStateManager = GameStateManager.Instance;
            
            // 빌딩 씬에서 배치한 타일 생성
            if (InventoryManager.Instance.PlacedTiles.Count > 0)
            {
                CreateTilesFromBuildingData();
            }
            else
            {
                Debug.LogWarning("빈 인벤토리로 게임에 왔어요!! 죽으셔야해요!");
            }
            
            SpawnPlayer();
            SpawnEnemy();
            
            // 카운트다운 텍스트 확인
            if (countdownText == null)
            {
                GameObject textObj = GameObject.Find("CountdownText");
                if (textObj != null)
                {
                    countdownText = textObj.GetComponent<TextMeshProUGUI>();
                }
            }
            
            // 카운트다운 시작
            StartCoroutine(StartCountdown());
        }
        
        private IEnumerator StartCountdown()
        {
            // 게임 시간은 멈추되 UI는 업데이트되도록 설정
            TimeScaleManager.Instance.StopTimeScale();
    
            // 카운트다운 시작
            float timeLeft = countdownDuration;
    
            while (timeLeft > 0)
            {
                // 카운트다운 텍스트 업데이트
                if (countdownText != null)
                {
                    countdownText.text = Mathf.CeilToInt(timeLeft).ToString();
                    countdownText.gameObject.SetActive(true);
                }
        
                // Time.timeScale에 영향받지 않는 WaitForSecondsRealtime 사용
                yield return new WaitForSecondsRealtime(0.1f);
                timeLeft -= 0.1f;
            }
    
            // 카운트다운 완료
            if (countdownText != null)
            {
                countdownText.text = "Start!";
                yield return new WaitForSecondsRealtime(0.5f);
                countdownText.gameObject.SetActive(false);
            }
    
            TimeScaleManager.Instance.ResetTimeScale();
    
            _gameStarted = true;
    
            // 게임 시작 상태로 설정
            _gameStateManager.StartGame();
        }
        
        private void CreateTilesFromBuildingData()
        {
            // 그리드 총 행 수 정의
            int totalRows = 8;
            
            // 선명한 색상 팔레트 정의 (20가지)
            Color[] colorPalette = new Color[]
            {
                new Color(1.0f, 0.0f, 0.0f, 0.7f),       // 빨강
                new Color(0.0f, 1.0f, 0.0f, 0.7f),       // 녹색
                new Color(0.0f, 0.0f, 1.0f, 0.7f),       // 파랑
                new Color(1.0f, 1.0f, 0.0f, 0.7f),       // 노랑
                new Color(1.0f, 0.0f, 1.0f, 0.7f),       // 마젠타
                new Color(0.0f, 1.0f, 1.0f, 0.7f),       // 시안
                new Color(1.0f, 0.5f, 0.0f, 0.7f),       // 주황
                new Color(0.5f, 0.0f, 1.0f, 0.7f),       // 보라
                new Color(0.0f, 0.5f, 1.0f, 0.7f),       // 하늘색
                new Color(0.5f, 1.0f, 0.0f, 0.7f),       // 라임
                new Color(1.0f, 0.0f, 0.5f, 0.7f),       // 핑크
                new Color(0.0f, 1.0f, 0.5f, 0.7f),       // 민트
                new Color(0.5f, 0.5f, 1.0f, 0.7f),       // 라벤더
                new Color(1.0f, 0.5f, 0.5f, 0.7f),       // 살구색
                new Color(0.5f, 1.0f, 0.5f, 0.7f),       // 연두
                new Color(0.7f, 0.3f, 0.0f, 0.7f),       // 갈색
                new Color(0.0f, 0.7f, 0.3f, 0.7f),       // 청록
                new Color(0.3f, 0.0f, 0.7f, 0.7f),       // 남색
                new Color(0.7f, 0.0f, 0.3f, 0.7f),       // 자주
                new Color(0.3f, 0.7f, 0.0f, 0.7f)        // 올리브
            };
            
            // 색상 인덱스 (순차적으로 색상 할당)
            int colorIndex = 0;
            
            foreach (TilePlacementData placementData in InventoryManager.Instance.PlacedTiles)
            {
                // InventoryItemData 사용
                InventoryItemData itemData = placementData.itemData;
                
                if (itemData != null)
                {
                    GameObject tilePrefab = GetTilePrefabByType(itemData.TileType);
                    
                    if (tilePrefab != null)
                    {
                        // 타일 원점(좌상단) 위치
                        int startX = placementData.x;
                        int startY = totalRows - 1 - placementData.y;
                        
                        // 타일 중심 위치 계산
                        Vector3 centerPos = _gridSystem.GetWorldPosition(startX, startY);
                        
                        // 타일 생성 (회전값은 0으로 설정 - ShapeData에 이미 회전이 반영됨)
                        GameObject tileObj = Instantiate(tilePrefab, centerPos, Quaternion.identity);
                        BaseTile tile = tileObj.GetComponent<BaseTile>();
                        
                        // 공통 타이밍 속성 설정
                        if (tile != null)
                        {
                            tile.ChargeTime = itemData.ChargeTime;
                        }
                        
                        // 타일 속성 설정 (InventoryItemData 기반)
                        if (tile is AttackTile attackTile)
                        {
                            attackTile.Damage = itemData.Damage;
                        }
                        else if (tile is DefenseTile defenseTile)
                        {
                            defenseTile.InvincibilityDuration = itemData.InvincibilityDuration;
                        }
                        else if (tile is HealTile healTile)
                        {
                            healTile.HealAmount = itemData.HealAmount;
                        }
                        else if (tile is ObstacleTile obstacleTile)
                        {
                            obstacleTile.Duration = itemData.ObstacleDuration;
                        }
                        
                        // 팔레트에서 순차적으로 색상 선택
                        Color tileColor = colorPalette[colorIndex % colorPalette.Length];
                        // 다음 타일은 다음 색상 사용
                        colorIndex++;
                        
                        // ShapeData에서 실제로 차지하는 셀 정보 사용 (Y축 반전)
                        bool[,] shapeData = itemData.ShapeData;
                        for (int y = 0; y < itemData.Height; y++)
                        {
                            for (int x = 0; x < itemData.Width; x++)
                            {
                                // Y축 반전하여 배치 (아래에서 위로)
                                int invertedY = itemData.Height - 1 - y;
                                
                                // 해당 위치에 타일이 존재하는 경우에만 처리
                                if (shapeData[invertedY, x])
                                {
                                    int gridX = startX + x;
                                    int gridY = startY + y - itemData.Height + 1; // Height만큼 아래로 이동
                                    
                                    _gridSystem.RegisterTile(tile, gridX, gridY);
                                    
                                    // 하이라이트 타일 추가 (실제 차지하는 셀만)
                                    if (highlightTilePrefab != null)
                                    {
                                        Vector3 worldPos = _gridSystem.GetWorldPosition(gridX, gridY);
                                        GameObject highlight = Instantiate(highlightTilePrefab, worldPos, Quaternion.identity);
                                        
                                        // 하이라이트에 색상 적용
                                        SpriteRenderer renderer = highlight.GetComponentInChildren<SpriteRenderer>();
                                        if (renderer != null)
                                        {
                                            renderer.color = tileColor;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            Debug.Log($"빌딩 데이터에서 {InventoryManager.Instance.PlacedTiles.Count}개 타일 생성 완료");
        }

        // TileType으로 프리팹 가져오기
        private GameObject GetTilePrefabByType(TileType type)
        {
            switch (type)
            {
                case TileType.Attack:
                    return attackTilePrefab;
                case TileType.Defense:
                    return defenseTilePrefab;
                case TileType.Heal:
                    return healTilePrefab;
                default:
                    return null;
            }
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