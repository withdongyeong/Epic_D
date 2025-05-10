using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using Game.Scripts.Building;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 인벤토리 아이템 클래스
    /// </summary>
    public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("참조")] [SerializeField] private Image _itemImage;
        [SerializeField] private GameObject _rotationHintObject;
        [SerializeField] private GameObject _shapeTilePrefab; // 아이템 모양 표시용 타일 프리팹

        private InventoryItemData _itemData;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Vector2 _originalPosition;
        private Transform _originalParent;
        private InventoryGrid _inventoryGrid;
        private Canvas _canvas;
        private Camera _camera;

        private int _gridX = -1;
        private int _gridY = -1;
        private bool _isDragging = false;
        private GameObject[] _shapeTiles; // 아이템 모양 표시용 타일들
        
        
        /// <summary>
        /// 아이템 데이터 프로퍼티
        /// </summary>
        public InventoryItemData ItemData
        {
            get => _itemData;
        }

        /// <summary>
        /// 그리드 X 좌표 프로퍼티
        /// </summary>
        public int GridX
        {
            get => _gridX;
        }

        /// <summary>
        /// 그리드 Y 좌표 프로퍼티
        /// </summary>
        public int GridY
        {
            get => _gridY;
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
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();

            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            _canvas = GetComponentInParent<Canvas>();
            if (_canvas == null)
            {
                _canvas = FindFirstObjectByType<Canvas>();
            }

            _camera = _canvas?.worldCamera;

            // 인벤토리 그리드 찾기
            _inventoryGrid = FindAnyObjectByType<InventoryGrid>();

            // 회전 힌트 초기화
            if (_rotationHintObject != null)
            {
                _rotationHintObject.SetActive(false);
            }
        }
        private void Update()
        {
            // 회전 키 처리 (R 키) - 드래그 중일 때만 작동
            if (Input.GetKeyDown(KeyCode.R) && _isDragging)
            {
                Debug.Log("Update에서 R키 감지: 회전 시작");
                RotateItem();
        
                // 중요: 회전 후 명시적으로 그리드 하이라이트 업데이트
                UpdateGridHighlight();
                Debug.Log("Update에서 회전 후 하이라이트 업데이트 호출");
            }
        }

        void Start()
        {
            // 임시 아이템 데이터 생성 (L 모양)
            InventoryItemData tempData = ScriptableObject.CreateInstance<InventoryItemData>();
            tempData.ItemName = "Test L-Shape";
            tempData.ItemSprite = GetComponentInChildren<Image>().sprite;
    
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
        /// 아이템 모양을 표시할 타일들 생성
        /// </summary>
        private void CreateShapeTiles()
        {
            if (_shapeTilePrefab == null || _itemData == null) return;

            // 기존 타일 제거
            DestroyShapeTiles();

            int count = 0;
            // 전체 셀 개수 계산
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (ShapeData[y, x])
                    {
                        count++;
                    }
                }
            }

            _shapeTiles = new GameObject[count];

            // 타일 생성
            int index = 0;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (ShapeData[y, x])
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
        /// 그리드 위치 설정
        /// </summary>
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
    
            // 이미지만 회전 (전체 GameObject는 회전하지 않음)
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
            if (_isDragging)
            {
                SetShapeTilesActive(true);
            }
            
    
            // 그리드 하이라이트 강제 업데이트
            if (_isDragging)
            {
                UpdateGridHighlight();
            }
        }

        /// <summary>
        /// 드래그 시작 처리
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalPosition = _rectTransform.anchoredPosition;
            _originalParent = transform.parent;
            _isDragging = true;
    
            // 아직 그리드에 배치된 아이템이라면 그리드에서 제거
            if (_inventoryGrid != null && _gridX != -1 && _gridY != -1)
            {
                _inventoryGrid.RemoveItem(this);
            }
    
            // 드래그 중 UI 설정
            _canvasGroup.alpha = 0.8f;
            _canvasGroup.blocksRaycasts = false;
    
            // 최상위 캔버스로 이동
            if (_canvas != null)
            {
                transform.SetParent(_canvas.transform);
            }
    
            // 회전 힌트 표시
            if (_rotationHintObject != null)
            {
                _rotationHintObject.SetActive(true);
            }
    
            // 형태 타일 표시
            SetShapeTilesActive(true);
    
            // 드래그 시작 시 명시적으로 하이라이트 업데이트
            UpdateGridHighlight();
    
            // 디버그 로그
            Debug.Log("드래그 시작: 위치 = " + eventData.position);
        }

        /// <summary>
        /// 드래그 중 처리
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            // 드래그 중 상태 확인
            if (!_isDragging)
            {
                _isDragging = true; // 드래그 중 상태 보장
                Debug.Log("OnDrag: 드래그 상태 복원");
            }
    
            // 마우스 위치로 이동
            _rectTransform.position = eventData.position;
    
            // 회전 키 처리는 Update 메서드에서 처리
    
            // 인벤토리 그리드 위치 하이라이트 - 강제 업데이트
            if (_inventoryGrid != null)
            {
                UpdateGridHighlight();
            }
            else
            {
                Debug.LogWarning("_inventoryGrid가 null입니다");
            }
        }

        /// <summary>
        /// 드래그 종료 처리
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;

            // UI 설정 복원
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            // 회전 힌트 숨기기
            if (_rotationHintObject != null)
            {
                _rotationHintObject.SetActive(false);
            }

            // 형태 타일 숨기기
            SetShapeTilesActive(false);

            // 그리드에 배치 시도
            bool placed = TryPlaceOnGrid();

            // 배치 실패 시 원래 위치로 복귀
            if (!placed)
            {
                transform.SetParent(_originalParent);
                _rectTransform.anchoredPosition = _originalPosition;

                // 아이템이 원래 그리드에 있었다면 다시 배치
                if (_inventoryGrid != null && _gridX != -1 && _gridY != -1)
                {
                    _inventoryGrid.PlaceItem(this, _gridX, _gridY);
                }
            }

            // 그리드 하이라이트 초기화
            if (_inventoryGrid != null)
            {
                _inventoryGrid.ResetHighlights();
            }
        }

        /// <summary>
        /// 그리드 하이라이트 업데이트
        /// </summary>
        private void UpdateGridHighlight()
        {
            if (_inventoryGrid == null || _itemData == null || _camera == null) return;

            // 마우스 위치를 그리드 좌표로 변환 시도
            if (_inventoryGrid.TryGetGridCoordinates(Input.mousePosition, _camera, out int gridX, out int gridY))
            {
                // 배치 가능 여부 확인 및 하이라이트
                bool canPlace = _inventoryGrid.CanPlaceItem(this, gridX, gridY);
                _inventoryGrid.HighlightCells(gridX, gridY, _itemData.ShapeData, canPlace);
            }
            else
            {
                // 그리드 밖에 있을 경우 하이라이트 제거
                _inventoryGrid.ResetHighlights();
            }
        }

        /// <summary>
        /// 그리드에 아이템 배치 시도
        /// </summary>
        private bool TryPlaceOnGrid()
        {
            if (_inventoryGrid == null)
            {
                Debug.LogError("인벤토리 그리드 참조가 없습니다");
                return false;
            }
    
            if (_itemData == null)
            {
                Debug.LogError("아이템 데이터가 없습니다");
                return false;
            }
    
            if (_camera == null)
            {
                Debug.LogWarning("카메라 참조가 없습니다. 메인 카메라를 사용합니다.");
                _camera = Camera.main;
            }
    
            Debug.Log("TryPlaceOnGrid 호출됨: 마우스 위치 " + Input.mousePosition);
    
            // 마우스 위치를 그리드 좌표로 변환 시도
            if (_inventoryGrid.TryGetGridCoordinates(Input.mousePosition, _camera, out int gridX, out int gridY))
            {
                Debug.Log($"그리드 좌표 변환 성공: ({gridX}, {gridY})");
        
                // 배치 가능 여부 확인
                bool canPlace = _inventoryGrid.CanPlaceItem(this, gridX, gridY);
                Debug.Log("배치 가능: " + canPlace);
        
                // 배치 시도
                if (_inventoryGrid.PlaceItem(this, gridX, gridY))
                {
                    // 아이템 위치 설정
                    transform.SetParent(_inventoryGrid.GridContainer);
                    _rectTransform.anchoredPosition = _inventoryGrid.GetCellPosition(gridX, gridY);
    
                    // 정렬 순서 조정 (다른 UI 요소보다 위에 표시)
                    Canvas.ForceUpdateCanvases(); // 캔버스 업데이트
                    if (GetComponent<Canvas>() != null)
                    {
                        GetComponent<Canvas>().sortingOrder = 10; // 높은 정렬 순서 설정
                    }
                    else
                    {
                        Canvas itemCanvas = gameObject.AddComponent<Canvas>();
                        itemCanvas.overrideSorting = true;
                        itemCanvas.sortingOrder = 10;
                    }
    
                    return true;
                }
                else
                {
                    Debug.Log("아이템 배치 실패: PlaceItem 반환값 false");
                }
            }
            else
            {
                Debug.Log("그리드 좌표 변환 실패: 마우스가 그리드 영역 밖에 있습니다");
            }
    
            return false;
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

        /// <summary>
        /// 파괴 시 리소스 정리
        /// </summary>
        private void OnDestroy()
        {
            DestroyShapeTiles();
        }
    }
}