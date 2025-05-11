using UnityEngine;
using System;
using Game.Scripts.Building;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 인벤토리 아이템 기본 클래스 - 아이템 데이터와 그리드 위치 관리
    /// </summary>
    public class InventoryItem : MonoBehaviour
    {
        private InventoryItemData _itemData;
        private int _gridX = -1;
        private int _gridY = -1;
        
        private InventoryItemDragHandler _dragHandler;
        private InventoryItemVisualizer _visualizer;
        
        /// <summary>
        /// 아이템 데이터 프로퍼티
        /// </summary>
        public InventoryItemData ItemData
        {
            get => _itemData;
            private set => _itemData = value;
        }

        /// <summary>
        /// 그리드 X 좌표 프로퍼티
        /// </summary>
        public int GridX
        {
            get => _gridX;
            private set => _gridX = value;
        }

        /// <summary>
        /// 그리드 Y 좌표 프로퍼티
        /// </summary>
        public int GridY
        {
            get => _gridY;
            private set => _gridY = value;
        }

        /// <summary>
        /// 아이템 너비 프로퍼티
        /// </summary>
        public int Width
        {
            get => _itemData != null ? _itemData.Width : 1;
        }

        /// <summary>
        /// 아이템 높이 프로퍼티
        /// </summary>
        public int Height
        {
            get => _itemData != null ? _itemData.Height : 1;
        }

        /// <summary>
        /// 아이템 형태 데이터 프로퍼티
        /// </summary>
        public bool[,] ShapeData
        {
            get => _itemData != null ? _itemData.ShapeData : new bool[1, 1] { { true } };
        }

        private void Awake()
        {
            // 컴포넌트 초기화 및 참조 설정
            _dragHandler = GetComponent<InventoryItemDragHandler>();
            _visualizer = GetComponent<InventoryItemVisualizer>();
            
            if (_dragHandler == null)
            {
                _dragHandler = gameObject.AddComponent<InventoryItemDragHandler>();
            }
            
            if (_visualizer == null)
            {
                _visualizer = gameObject.AddComponent<InventoryItemVisualizer>();
            }
        }

        void Start()
        {
            // 임시 아이템 데이터 생성 (L 모양) - 실제 구현에서는 제거하고 외부에서 설정
            InventoryItemData tempData = ScriptableObject.CreateInstance<InventoryItemData>();
            tempData.ItemName = "Test L-Shape";
            tempData.ItemSprite = GetComponentInChildren<UnityEngine.UI.Image>().sprite;
    
            // L 모양 설정 (3x2)
            bool[,] shapeData = new bool[2, 3] {
                { true, false, false },
                { true, true, true }
            };
            tempData.ShapeData = shapeData;
    
            // 아이템 초기화
            Initialize(tempData);
        }

        /// <summary>
        /// 아이템 데이터 설정
        /// </summary>
        /// <param name="itemData">설정할 아이템 데이터</param>
        public void Initialize(InventoryItemData itemData)
        {
            _itemData = itemData;
            
            // 시각화 컴포넌트에 데이터 전달
            if (_visualizer != null)
            {
                _visualizer.Initialize(itemData);
            }
            
            // 드래그 핸들러에 초기화 이벤트 전달
            if (_dragHandler != null)
            {
                _dragHandler.Initialize(this);
            }
        }

        /// <summary>
        /// 그리드 위치 설정
        /// </summary>
        /// <param name="x">그리드 X 좌표</param>
        /// <param name="y">그리드 Y 좌표</param>
        public void SetGridPosition(int x, int y)
        {
            _gridX = x;
            _gridY = y;
        }
        
        /// <summary>
        /// 아이템 회전
        /// </summary>
        public void RotateItem()
        {
            if (_itemData == null) return;
    
            // 아이템 데이터 회전 (ShapeData 회전)
            _itemData.Rotate90Degrees();
            
            // 시각화 컴포넌트에 회전 이벤트 전달
            _visualizer?.OnItemRotated();
            
            // 드래그 핸들러에 회전 이벤트 전달
            _dragHandler?.OnItemRotated();
        }
        
        /// <summary>
        /// 아이템 데이터를 TileData로 변환 (Building 시스템과 호환용)
        /// </summary>
        public TileData ConvertToTileData()
        {
            if (_itemData == null) return null;

            TileData tileData = ScriptableObject.CreateInstance<TileData>();
            tileData.type = _itemData.TileType;
            tileData.tileSprite = _itemData.ItemSprite;

            // 아이템 형태를 기반으로 너비와 높이 설정
            tileData.width = _itemData.Width;
            tileData.height = _itemData.Height;

            // 속성 설정
            tileData.damage = _itemData.Damage;

            return tileData;
        }
    }
}