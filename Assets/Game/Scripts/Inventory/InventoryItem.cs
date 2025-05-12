using UnityEngine;
using System;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 테스트용 아이템 형상 타입
    /// </summary>
    public enum ItemShapeType
    {
        Single,     // 1칸짜리
        Double,     // 2칸짜리 (가로)
        LShape,     // 3칸짜리 ㄱ 형태
        TetrisL,    // 4칸짜리 (기존 L 형태)
        Horizontal4 // 4칸짜리 (가로 4칸)
    }

    /// <summary>
    /// 인벤토리 아이템 기본 클래스 - 아이템 데이터와 그리드 위치 관리
    /// </summary>
    public class InventoryItem : MonoBehaviour
    {
        [SerializeField] private InventoryItemData _itemDataTemplate;
        
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
        /// 아이템 데이터 템플릿 프로퍼티
        /// </summary>
        public InventoryItemData ItemDataTemplate
        {
            get => _itemDataTemplate;
            set 
            { 
                _itemDataTemplate = value;
                if (_itemDataTemplate != null)
                {
                    // 템플릿이 변경되면 새로운 인스턴스 생성
                    InitializeFromTemplate();
                }
            }
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
            if (_itemDataTemplate != null)
            {
                // 템플릿에서 아이템 데이터 초기화
                InitializeFromTemplate();
            }
            else
            {
                // 템플릿이 없으면 기본 아이템 데이터 생성
                Debug.LogWarning("ItemDataTemplate이 설정되지 않았습니다. 기본값으로 초기화합니다.");
                CreateDefaultItemData();
            }
        }

        /// <summary>
        /// 템플릿에서 아이템 데이터 초기화
        /// </summary>
        private void InitializeFromTemplate()
        {
            // 템플릿 복제
            _itemData = _itemDataTemplate.Clone();
            _itemData.UpdateShapeFromType();
            
            // 시각화 및 드래그 핸들러 초기화
            _visualizer?.Initialize(_itemData);
            _dragHandler?.Initialize(this);
        }

        /// <summary>
        /// 기본 아이템 데이터 생성
        /// </summary>
        private void CreateDefaultItemData()
        {
            // 임시 아이템 데이터 생성
            InventoryItemData tempData = ScriptableObject.CreateInstance<InventoryItemData>();
            tempData.ItemName = "Default Item";
            tempData.ItemSprite = GetComponentInChildren<UnityEngine.UI.Image>()?.sprite;
            
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
    }
}