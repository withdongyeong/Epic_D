using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 타일 배치 데이터 클래스
    /// </summary>
    [System.Serializable]
    public class TilePlacementData
    {
        public InventoryItemData itemData;
        public int x;
        public int y;
    }
    
    /// <summary>
    /// 인벤토리 데이터 관리 싱글톤 클래스
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        private static InventoryManager _instance;
        
        private List<TilePlacementData> _placedTiles = new List<TilePlacementData>();
        
        /// <summary>
        /// 싱글톤 인스턴스 프로퍼티
        /// </summary>
        public static InventoryManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("InventoryManager");
                    _instance = go.AddComponent<InventoryManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        /// <summary>
        /// 배치된 타일 목록 프로퍼티
        /// </summary>
        public List<TilePlacementData> PlacedTiles
        {
            get => _placedTiles;
            private set => _placedTiles = value;
        }
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        /// <summary>
        /// 인벤토리 그리드에서 타일 데이터 수집
        /// </summary>
        /// <param name="grid">인벤토리 그리드</param>
        public void CollectTilesFromGrid()
        {
            InventoryGrid grid = FindAnyObjectByType<InventoryGrid>();
            
            if (grid == null) return;
            
            // 기존 데이터 클리어
            _placedTiles.Clear();
            
            // 그리드에서 모든 아이템 가져오기
            List<InventoryItem> items = grid.GetAllItems();
            
            foreach (InventoryItem item in items)
            {
                // 아이템 데이터 가져오기
                InventoryItemData itemData = item.ItemData;
                
                if (itemData != null)
                {
                    TilePlacementData placementData = new TilePlacementData
                    {
                        itemData = itemData,
                        x = item.GridX,
                        y = item.GridY
                    };
                    
                    _placedTiles.Add(placementData);
                }
            }
            
            Debug.Log($"인벤토리에서 {_placedTiles.Count}개 타일 수집 완료");
        }
        
        /// <summary>
        /// 현재 배치된 타일 목록 초기화
        /// </summary>
        public void ClearPlacedTiles()
        {
            _placedTiles.Clear();
        }
    }
}