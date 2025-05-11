using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 인벤토리 아이템 드래그 처리 담당 클래스
    /// </summary>
    public class InventoryItemDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private InventoryItem _itemReference;
        private RectTransform _rectTransform;
        private CanvasGroup _canvasGroup;
        private Canvas _canvas;
        private Camera _camera;
        private InventoryGrid _inventoryGrid;
        
        private Vector2 _originalPosition;
        private Transform _originalParent;
        private bool _isDragging = false;
        private bool _isInitialized = false;
        
        /// <summary>
        /// 드래그 중인지 여부를 나타내는 프로퍼티
        /// </summary>
        public bool IsDragging 
        { 
            get => _isDragging; 
            private set => _isDragging = value; 
        }

        private void Awake()
        {
            // 컴포넌트 초기화
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
            if (_camera == null)
            {
                _camera = Camera.main;
            }

            // 인벤토리 그리드 찾기
            _inventoryGrid = FindAnyObjectByType<InventoryGrid>();
        }
        
        private void Start()
        {
            // Start에서 InventoryItem 참조 가져오기
            if (_itemReference == null)
            {
                _itemReference = GetComponent<InventoryItem>();
                if (_itemReference != null && _itemReference.ItemData != null)
                {
                    _isInitialized = true;
                }
            }
        }
        
        private void Update()
        {
            // 회전 키 처리 (R 키) - 드래그 중일 때만 작동
            if (Input.GetKeyDown(KeyCode.R) && _isDragging && _itemReference != null)
            {
                _itemReference.RotateItem();
                UpdateGridHighlight();
            }
        }

        /// <summary>
        /// 핸들러 초기화
        /// </summary>
        /// <param name="item">연결할 InventoryItem 참조</param>
        public void Initialize(InventoryItem item)
        {
            _itemReference = item;
            _isInitialized = (_itemReference != null && _itemReference.ItemData != null);
            
            // 그리드 참조 확인 및 업데이트
            if (_inventoryGrid == null)
            {
                _inventoryGrid = FindAnyObjectByType<InventoryGrid>();
            }
            
            // 카메라 참조 확인 및 업데이트
            if (_camera == null)
            {
                _camera = _canvas?.worldCamera;
                if (_camera == null)
                {
                    _camera = Camera.main;
                }
            }
        }
        
        /// <summary>
        /// 아이템 회전 후 호출되는 메서드
        /// </summary>
        public void OnItemRotated()
        {
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
            // 초기화 상태 확인
            if (!_isInitialized && _itemReference == null)
            {
                _itemReference = GetComponent<InventoryItem>();
                _isInitialized = (_itemReference != null && _itemReference.ItemData != null);
            }
            
            if (_itemReference == null || _itemReference.ItemData == null) 
            {
                Debug.LogWarning("아이템 참조가 없어 드래그를 시작할 수 없습니다");
                return;
            }
            
            _originalPosition = _rectTransform.anchoredPosition;
            _originalParent = transform.parent;
            _isDragging = true;
    
            // 아직 그리드에 배치된 아이템이라면 그리드에서 제거
            if (_inventoryGrid != null && _itemReference.GridX != -1 && _itemReference.GridY != -1)
            {
                _inventoryGrid.RemoveItem(_itemReference);
            }
    
            // 드래그 중 UI 설정
            _canvasGroup.alpha = 0.8f;
            _canvasGroup.blocksRaycasts = false;
    
            // 최상위 캔버스로 이동
            if (_canvas != null)
            {
                transform.SetParent(_canvas.transform);
            }
    
            // 시각적 표시를 위한 이벤트 발생 - InventoryItemVisualizer에서 처리
            GetComponent<InventoryItemVisualizer>()?.OnBeginDrag();
            
            // 그리드 참조 재확인
            if (_inventoryGrid == null)
            {
                _inventoryGrid = FindAnyObjectByType<InventoryGrid>();
            }
    
            // 카메라 참조 재확인
            if (_camera == null)
            {
                _camera = _canvas?.worldCamera;
                if (_camera == null)
                {
                    _camera = Camera.main;
                }
            }
    
            // 드래그 시작 시 명시적으로 하이라이트 업데이트
            UpdateGridHighlight();
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
    
            // 인벤토리 그리드 위치 하이라이트 - 강제 업데이트
            if (_inventoryGrid != null)
            {
                UpdateGridHighlight();
            }
            else
            {
                // 그리드 참조가 없는 경우 찾기 시도
                _inventoryGrid = FindAnyObjectByType<InventoryGrid>();
                if (_inventoryGrid != null)
                {
                    UpdateGridHighlight();
                }
                else
                {
                    Debug.LogWarning("_inventoryGrid를 찾을 수 없습니다");
                }
            }
        }

        /// <summary>
        /// 드래그 종료
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            if (_itemReference == null || !_isInitialized) return;
            
            _isDragging = false;

            // UI 설정 복원
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;

            // 시각적 표시를 위한 이벤트 발생 - InventoryItemVisualizer에서 처리
            GetComponent<InventoryItemVisualizer>()?.OnEndDrag();

            // 그리드에 배치 시도
            bool placed = TryPlaceOnGrid();

            // 배치 실패 시 원래 위치로 복귀
            if (!placed)
            {
                transform.SetParent(_originalParent);
                _rectTransform.anchoredPosition = _originalPosition;

                // 아이템이 원래 그리드에 있었다면 다시 배치
                if (_inventoryGrid != null && _itemReference.GridX != -1 && _itemReference.GridY != -1)
                {
                    _inventoryGrid.PlaceItem(_itemReference, _itemReference.GridX, _itemReference.GridY);
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
            if (_itemReference == null || _itemReference.ItemData == null) 
            {
                Debug.LogWarning("아이템 참조가 없어 하이라이트를 업데이트할 수 없습니다");
                return;
            }
            
            if (_inventoryGrid == null) 
            {
                Debug.LogWarning("그리드 참조가 없어 하이라이트를 업데이트할 수 없습니다");
                return;
            }
            
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null)
                {
                    Debug.LogWarning("카메라 참조가 없어 하이라이트를 업데이트할 수 없습니다");
                    return;
                }
            }

            // 마우스 위치를 그리드 좌표로 변환 시도
            if (_inventoryGrid.TryGetGridCoordinates(Input.mousePosition, _camera, out int gridX, out int gridY))
            {
                // 배치 가능 여부 확인 및 하이라이트
                bool canPlace = _inventoryGrid.CanPlaceItem(_itemReference, gridX, gridY);
                _inventoryGrid.HighlightCells(gridX, gridY, _itemReference.ShapeData, canPlace);
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
                _inventoryGrid = FindAnyObjectByType<InventoryGrid>();
                if (_inventoryGrid == null)
                {
                    Debug.LogError("인벤토리 그리드 참조를 찾을 수 없습니다");
                    return false;
                }
            }
    
            if (_itemReference == null || _itemReference.ItemData == null)
            {
                Debug.LogError("아이템 데이터가 없습니다");
                return false;
            }
    
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null)
                {
                    Debug.LogError("카메라 참조를 찾을 수 없습니다");
                    return false;
                }
            }
    
            // 마우스 위치를 그리드 좌표로 변환 시도
            if (_inventoryGrid.TryGetGridCoordinates(Input.mousePosition, _camera, out int gridX, out int gridY))
            {
                // 배치 가능 여부 확인
                bool canPlace = _inventoryGrid.CanPlaceItem(_itemReference, gridX, gridY);
                
                // 골드 확인
                if (!ShopManager.Instance.CanPurchase(_itemReference.ItemData))
                {
                    Debug.Log("골드가 부족하여 아이템을 배치할 수 없습니다.");
                    return false;
                }
        
                // 배치 시도
                if (_inventoryGrid.PlaceItem(_itemReference, gridX, gridY))
                {
                    // 골드 차감
                    if (!ShopManager.Instance.Purchase(_itemReference.ItemData))
                    {
                        // 골드 부족으로 구매 실패 시 배치 취소
                        _inventoryGrid.RemoveItem(_itemReference);
                        return false;
                    }
                    
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
            }
            else
            {
                Debug.Log("그리드 좌표 변환 실패: 마우스가 그리드 영역 밖에 있습니다");
            }
    
            return false;
        }
    }
}