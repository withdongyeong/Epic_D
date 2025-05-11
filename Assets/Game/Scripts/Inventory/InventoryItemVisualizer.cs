using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 인벤토리 아이템 시각화 담당 클래스
    /// </summary>
    public class InventoryItemVisualizer : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private Image _itemImage;
        [SerializeField] private GameObject _rotationHintObject;
        [SerializeField] private GameObject _shapeTilePrefab; // 아이템 모양 표시용 타일 프리팹
        
        private InventoryItemData _itemData;
        private InventoryGrid _inventoryGrid;
        private GameObject[] _shapeTiles; // 아이템 모양 표시용 타일들
        
        /// <summary>
        /// 아이템 이미지 프로퍼티
        /// </summary>
        public Image ItemImage 
        { 
            get => _itemImage; 
            private set => _itemImage = value; 
        }

        private void Awake()
        {
            // 이미지 참조가 없으면 찾기
            if (_itemImage == null)
            {
                _itemImage = GetComponentInChildren<Image>();
            }
            
            // 인벤토리 그리드 찾기
            _inventoryGrid = FindAnyObjectByType<InventoryGrid>();
            
            // 회전 힌트 초기화
            if (_rotationHintObject != null)
            {
                _rotationHintObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 시각화 컴포넌트 초기화
        /// </summary>
        /// <param name="itemData">아이템 데이터</param>
        public void Initialize(InventoryItemData itemData)
        {
            _itemData = itemData;

            if (_itemImage != null && itemData.ItemSprite != null)
            {
                _itemImage.sprite = itemData.ItemSprite;

                // 피벗 포인트 설정 (중심점 조정)
                _itemImage.rectTransform.pivot = itemData.SpritePivot;
            }

            // 형태 타일 생성
            CreateShapeTiles();

            // 형태 타일 초기 상태는 비활성화
            SetShapeTilesActive(false);
        }
        
        /// <summary>
        /// 드래그 시작 시 호출
        /// </summary>
        public void OnBeginDrag()
        {
            // 회전 힌트 표시
            if (_rotationHintObject != null)
            {
                _rotationHintObject.SetActive(true);
            }
    
            // 형태 타일 표시
            SetShapeTilesActive(true);
        }
        
        /// <summary>
        /// 드래그 종료 시 호출
        /// </summary>
        public void OnEndDrag()
        {
            // 회전 힌트 숨기기
            if (_rotationHintObject != null)
            {
                _rotationHintObject.SetActive(false);
            }

            // 형태 타일 숨기기
            SetShapeTilesActive(false);
        }
        
        /// <summary>
        /// 아이템 회전 시 호출
        /// </summary>
        public void OnItemRotated()
        {
            if (_itemData == null) return;
            
            // 이미지 회전
            if (_itemImage != null)
            {
                // 이미지가 자식 오브젝트인 경우
                if (_itemImage.transform != transform)
                {
                    Vector3 currentRotation = _itemImage.rectTransform.eulerAngles;
                    _itemImage.rectTransform.eulerAngles = new Vector3(
                        currentRotation.x,
                        currentRotation.y,
                        currentRotation.z + 90f
                    );
                }
                // 이미지가 동일 오브젝트인 경우 (별도의 자식 이미지 생성 필요)
                else
                {
                    // 새 이미지 생성 또는 기존 회전 유지
                    // (상황에 따라 다르게 처리)
                }
            }
    
            // 형태 타일 다시 생성
            CreateShapeTiles();
    
            // 드래그 중이면 형태 타일 표시
            if (GetComponent<InventoryItemDragHandler>()?.IsDragging == true)
            {
                SetShapeTilesActive(true);
            }
        }
        
        /// <summary>
        /// 아이템 모양을 표시할 타일들 생성
        /// </summary>
        private void CreateShapeTiles()
        {
            if (_shapeTilePrefab == null || _itemData == null) return;

            // 기존 타일 제거
            DestroyShapeTiles();

            int width = _itemData.Width;
            int height = _itemData.Height;
            bool[,] shapeData = _itemData.ShapeData;
            
            int count = 0;
            // 전체 셀 개수 계산
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (shapeData[y, x])
                    {
                        count++;
                    }
                }
            }

            _shapeTiles = new GameObject[count];

            // 타일 생성
            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (shapeData[y, x])
                    {
                        GameObject tile = Instantiate(_shapeTilePrefab, transform);
                        RectTransform tileRect = tile.GetComponent<RectTransform>();

                        if (tileRect != null && _inventoryGrid != null)
                        {
                            float cellSize = _inventoryGrid.CellSize;
                            tileRect.sizeDelta = new Vector2(cellSize, cellSize);

                            // 로컬 위치 설정 (원점은 아이템의 왼쪽 상단)
                            float posX = x * cellSize + cellSize / 2;
                            float posY = -y * cellSize - cellSize / 2;
                            tileRect.anchoredPosition = new Vector2(posX, posY);
                        }

                        _shapeTiles[index] = tile;
                        index++;
                    }
                }
            }
        }

        /// <summary>
        /// 모양 타일 활성화/비활성화
        /// </summary>
        private void SetShapeTilesActive(bool active)
        {
            if (_shapeTiles == null) return;

            foreach (GameObject tile in _shapeTiles)
            {
                if (tile != null)
                {
                    tile.SetActive(active);
                }
            }
        }

        /// <summary>
        /// 모양 타일 제거
        /// </summary>
        private void DestroyShapeTiles()
        {
            if (_shapeTiles != null)
            {
                foreach (GameObject tile in _shapeTiles)
                {
                    if (tile != null)
                    {
                        Destroy(tile);
                    }
                }
            }

            _shapeTiles = null;
        }
        
        /// <summary>
        /// 파괴 시 리소스 정리
        /// </summary>
        private void OnDestroy()
        {
            DestroyShapeTiles();
        }
    }
}