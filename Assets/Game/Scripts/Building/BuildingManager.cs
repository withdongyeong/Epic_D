using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Game.Scripts.Building.Game.Scripts.Building;

namespace Game.Scripts.Building
{
    /// <summary>
    /// 빌딩 씬 관리 클래스
    /// </summary>
    public class BuildingManager : MonoBehaviour
    {
        [Header("UI 요소")]
        public Button startGameButton;
        private string _gameplaySceneName = "GameplayScene";
        
        [Header("타일 프리팹")]
        public GameObject attackTilePrefab;
        
        private BuildingGridSystem _gridSystem;
        private ShopManager _shopManager;
        
        // 타일 배치 데이터를 게임 씬으로 전달하기 위한 정적 변수
        public static List<TilePlacementData> PlacedTiles = new List<TilePlacementData>();

        private void Awake()
        {
            InitializeShopItems();
        }

        private void Start()
        {
            // 컴포넌트 참조 가져오기
            _gridSystem = FindAnyObjectByType<BuildingGridSystem>();
            _shopManager = FindAnyObjectByType<ShopManager>();
            
            InitializeUI();
            
        }
        
        private void OnEnable()
        {
            InitializeUI();
        }
        
        /// <summary>
        /// UI 초기화 - 버튼 이벤트 연결
        /// </summary>
        private void InitializeUI()
        {
            // 타임스케일 초기화
            Time.timeScale = 1.0f;
            
            if (startGameButton != null)
            {
                // 기존 리스너 제거 후 새로 할당
                startGameButton.onClick.RemoveAllListeners();
                startGameButton.onClick.AddListener(OnStartGameClicked);
            }
            else
            {
                Debug.LogError("시작 버튼이 할당되지 않았습니다.");
            }
        }
        
        /// <summary>
        /// 상점 아이템 초기화
        /// </summary>
        private void InitializeShopItems()
        {
            if (_shopManager != null)
            {
                // 사용 가능한 타일 목록 생성
                _shopManager.availableTiles.Clear();
                
                // 1x1 공격 타일 추가
                _shopManager.availableTiles.Add(new TileData(
                    TileData.TileType.Attack, 
                    "Basic Attack", 
                    1, 1, 10) 
                    { tilePrefab = attackTilePrefab });
                
                // 2x1 공격 타일 추가
                _shopManager.availableTiles.Add(new TileData(
                    TileData.TileType.Attack, 
                    "Double Attack", 
                    2, 1, 15) 
                    { tilePrefab = attackTilePrefab });
                
                // 1x2 공격 타일 추가
                _shopManager.availableTiles.Add(new TileData(
                    TileData.TileType.Attack, 
                    "Tall Attack", 
                    1, 2, 15) 
                    { tilePrefab = attackTilePrefab });
                
                // 2x2 공격 타일 추가
                _shopManager.availableTiles.Add(new TileData(
                    TileData.TileType.Attack, 
                    "Quad Attack", 
                    2, 2, 20) 
                    { tilePrefab = attackTilePrefab });
            }
        }

        /// <summary>
        /// 게임 시작 버튼 클릭 핸들러
        /// </summary>
        private void OnStartGameClicked()
        {
            Debug.Log("게임 시작 버튼 클릭: 씬 전환 시도");
            
            // 배치된 타일 정보 저장
            SavePlacedTiles();
            
            // 직접 타임스케일 설정
            Time.timeScale = 1.0f;
            
            // 직접 씬 전환
            SceneManager.LoadScene(_gameplaySceneName);
        }
        
        /// <summary>
        /// 배치된 타일 정보 저장
        /// </summary>
        private void SavePlacedTiles()
        {
            PlacedTiles.Clear();
            
            if (_gridSystem == null)
                return;
                
            TileData[,] gridData = _gridSystem.GetGridData();
            
            // 그리드를 순회하며 타일 정보 저장
            for (int y = 0; y < _gridSystem.height; y++)
            {
                for (int x = 0; x < _gridSystem.width; x++)
                {
                    TileData tileData = gridData[x, y];
                    
                    if (tileData != null)
                    {
                        // 해당 타일의 원점(좌상단) 확인
                        bool isOrigin = IsOriginTile(gridData, x, y);
                        
                        if (isOrigin)
                        {
                            // 타일 배치 데이터 생성 및 저장
                            TilePlacementData placementData = new TilePlacementData
                            {
                                x = x,
                                y = y,
                                tileData = tileData
                            };
                            
                            PlacedTiles.Add(placementData);
                        }
                    }
                }
            }
            
            Debug.Log($"저장된 타일 개수: {PlacedTiles.Count}");
        }
        
        /// <summary>
        /// 원점 타일 여부 확인 (여러 칸 타일의 경우 좌상단 셀만 체크)
        /// </summary>
        private bool IsOriginTile(TileData[,] grid, int x, int y)
        {
            TileData currentTile = grid[x, y];
            
            // 왼쪽 확인
            if (x > 0 && grid[x - 1, y] == currentTile)
                return false;
                
            // 위쪽 확인
            if (y > 0 && grid[x, y - 1] == currentTile)
                return false;
                
            return true;
        }
    }
    
    /// <summary>
    /// 타일 배치 데이터 클래스
    /// </summary>
    [System.Serializable]
    public class TilePlacementData
    {
        public int x;
        public int y;
        public TileData tileData;
    }
}