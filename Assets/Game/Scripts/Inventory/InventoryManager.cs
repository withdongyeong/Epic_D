using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Scripts.Building;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 인벤토리 시스템 전체 관리 클래스
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private InventoryGrid _inventoryGrid;
        [SerializeField] private ShopManager _shopManager;
        [SerializeField] private Button _startGameButton;
        
        [Header("게임 시작 설정")]
        [SerializeField] private string _gameSceneName = "GameplayScene";
        
        // 싱글톤 패턴
        private static InventoryManager _instance;
        
        /// <summary>
        /// 싱글톤 인스턴스
        /// </summary>
        public static InventoryManager Instance 
        { 
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<InventoryManager>();
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// 인벤토리 그리드 프로퍼티
        /// </summary>
        public InventoryGrid InventoryGrid { get => _inventoryGrid; }
        
        /// <summary>
        /// 상점 매니저 프로퍼티
        /// </summary>
        public ShopManager ShopManager { get => _shopManager; }
        
        private void Awake()
        {
            // 싱글톤 설정
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
        }
        
        private void Start()
        {
            // 게임 시작 버튼 이벤트 연결
            if (_startGameButton != null)
            {
                _startGameButton.onClick.AddListener(StartGame);
            }
        }
        
        /// <summary>
        /// 게임 시작 처리
        /// </summary>
        public void StartGame()
        {
            // 인벤토리에 배치된 모든 아이템을 TilePlacementData로 변환
            SaveInventoryToBuilding();
            
            // 게임 씬으로 전환
            UnityEngine.SceneManagement.SceneManager.LoadScene(_gameSceneName);
        }
        
        /// <summary>
        /// 인벤토리 데이터를 BuildingManager에 저장
        /// </summary>
        private void SaveInventoryToBuilding()
        {
            if (_inventoryGrid == null) return;
            
            // BuildingManager 초기화 (기존 데이터 제거)
            BuildingManager.PlacedTiles.Clear();
            
            // 인벤토리의 모든 아이템 가져오기
            List<InventoryItem> inventoryItems = _inventoryGrid.GetAllItems();
            
            foreach (InventoryItem item in inventoryItems)
            {
                if (item == null) continue;
                
                // 아이템의 TileData 생성
                TileData tileData = item.ConvertToTileData();
                
                if (tileData != null)
                {
                    // 아이템 형태 데이터에 따라 타일 배치 정보 생성
                    for (int y = 0; y < item.Height; y++)
                    {
                        for (int x = 0; x < item.Width; x++)
                        {
                            if (item.ShapeData[y, x])
                            {
                                // TilePlacementData 생성 및 추가
                                TilePlacementData placementData = new TilePlacementData
                                {
                                    tileData = tileData,
                                    x = item.GridX + x,
                                    y = item.GridY + y
                                };
                                
                                BuildingManager.PlacedTiles.Add(placementData);
                            }
                        }
                    }
                }
            }
            
            Debug.Log($"인벤토리에서 {BuildingManager.PlacedTiles.Count}개 타일 데이터 저장 완료");
        }
        
        /// <summary>
        /// 인벤토리 초기화 (새 게임)
        /// </summary>
        public void ResetInventory()
        {
            if (_inventoryGrid != null)
            {
                // 모든 아이템 제거
                List<InventoryItem> items = _inventoryGrid.GetAllItems();
                foreach (InventoryItem item in items)
                {
                    if (item != null)
                    {
                        _inventoryGrid.RemoveItem(item);
                        Destroy(item.gameObject);
                    }
                }
            }
            
            // 상점 리셋
            if (_shopManager != null)
            {
                _shopManager.ResetShop();
            }
        }
    }
}