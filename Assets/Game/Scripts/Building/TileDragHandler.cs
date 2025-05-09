using Game.Scripts.Building.Game.Scripts.Building;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.Scripts.Building
{
    /// <summary>
    /// 타일 드래그 앤 드롭 처리
    /// </summary>
    public class TileDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("타일 설정")]
        public TileData tileData;
        
        [Header("UI 요소")]
        public Image tileImage;
        public GameObject rotationHintObject;
        
        private RectTransform _rectTransform;
        private Canvas _canvas;
        private CanvasGroup _canvasGroup;
        private Vector2 _originalPosition;
        private Transform _originalParent;
        private BuildingGridSystem _gridSystem;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvas = FindObjectOfType<Canvas>();
            _gridSystem = BuildingGridSystem.Instance;
            
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            if (rotationHintObject != null)
            {
                rotationHintObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// 타일 데이터 설정
        /// </summary>
        public void SetTileData(TileData data)
        {
            tileData = data;
            
            // 이미지 및 크기 업데이트
            if (tileImage != null && data.tileSprite != null)
            {
                tileImage.sprite = data.tileSprite;
            }
            
            // RectTransform 크기 설정 (회전 고려)
            UpdateRectSize();
        }
        
        /// <summary>
        /// 타일 회전
        /// </summary>
        public void RotateTile()
        {
            // 90도 회전 (0 -> 90 -> 180 -> 270 -> 0)
            tileData.rotation = (tileData.rotation + 90) % 360;
            
            // 이미지 회전
            _rectTransform.rotation = Quaternion.Euler(0, 0, tileData.rotation);
            
            // 크기 업데이트 (가로/세로 스왑)
            UpdateRectSize();
        }
        
        /// <summary>
        /// RectTransform 크기 업데이트
        /// </summary>
        private void UpdateRectSize()
        {
            float cellSize = _gridSystem.cellSize;
            
            // 회전 상태에 따라 가로/세로 크기 결정
            float width = (tileData.rotation == 90 || tileData.rotation == 270) 
                ? tileData.height * cellSize 
                : tileData.width * cellSize;
                
            float height = (tileData.rotation == 90 || tileData.rotation == 270) 
                ? tileData.width * cellSize 
                : tileData.height * cellSize;
                
            _rectTransform.sizeDelta = new Vector2(width, height);
        }
        
        /// <summary>
        /// 드래그 시작
        /// </summary>
        public void OnBeginDrag(PointerEventData eventData)
        {
            _originalPosition = _rectTransform.anchoredPosition;
            _originalParent = transform.parent;
            
            // 드래그 중 UI 설정
            _canvasGroup.alpha = 0.6f;
            _canvasGroup.blocksRaycasts = false;
            
            // 최상위 캔버스로 이동하여 다른 UI 요소 위에 표시
            transform.SetParent(_canvas.transform);
            
            // 회전 힌트 표시
            if (rotationHintObject != null)
            {
                rotationHintObject.SetActive(true);
            }
        }
        
        /// <summary>
        /// 드래그 중
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            // 마우스 위치로 이동
            _rectTransform.position = eventData.position;
            
            // 회전 키 처리 (R 키)
            if (Input.GetKeyDown(KeyCode.R))
            {
                RotateTile();
            }
            
            // 그리드 위치 계산 및 하이라이트
            UpdateGridHighlight();
        }
        
        /// <summary>
        /// 드래그 종료
        /// </summary>
        public void OnEndDrag(PointerEventData eventData)
        {
            // UI 설정 복원
            _canvasGroup.alpha = 1f;
            _canvasGroup.blocksRaycasts = true;
            
            // 회전 힌트 숨기기
            if (rotationHintObject != null)
            {
                rotationHintObject.SetActive(false);
            }
            
            // 그리드에 배치 시도
            bool placed = TryPlaceOnGrid();
            
            // 배치 실패 시 원래 위치로 복귀
            if (!placed)
            {
                transform.SetParent(_originalParent);
                _rectTransform.anchoredPosition = _originalPosition;
            }
            
            // 그리드 하이라이트 초기화
            _gridSystem.ResetHighlights();
        }
        
        /// <summary>
        /// 그리드 하이라이트 업데이트
        /// </summary>
        private void UpdateGridHighlight()
        {
            Vector2 mousePosition = Input.mousePosition;
            
            // 마우스 위치를 그리드 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _gridSystem.gridContainer as RectTransform,
                mousePosition,
                _canvas.worldCamera,
                out Vector2 localPoint
            );
            
            // 그리드 셀 크기로 나눠 인덱스 계산
            float cellSize = _gridSystem.cellSize;
            int x = Mathf.FloorToInt((localPoint.x + (_gridSystem.width * cellSize) / 2) / cellSize);
            int y = Mathf.FloorToInt((-localPoint.y + (_gridSystem.height * cellSize) / 2) / cellSize);
            
            // 회전 상태에 따른 실제 크기 계산
            int width = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.height : tileData.width;
            int height = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.width : tileData.height;
            
            // 배치 가능 여부 확인 및 하이라이트
            bool canPlace = _gridSystem.CanPlaceTile(tileData, x, y);
            _gridSystem.HighlightCells(x, y, width, height, canPlace);
        }
        
        /// <summary>
        /// 그리드에 타일 배치 시도
        /// </summary>
        private bool TryPlaceOnGrid()
        {
            Vector2 mousePosition = Input.mousePosition;
            
            // 마우스 위치를 그리드 좌표로 변환
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _gridSystem.gridContainer as RectTransform,
                mousePosition,
                _canvas.worldCamera,
                out Vector2 localPoint
            );
            
            // 그리드 셀 크기로 나눠 인덱스 계산
            float cellSize = _gridSystem.cellSize;
            int x = Mathf.FloorToInt((localPoint.x + (_gridSystem.width * cellSize) / 2) / cellSize);
            int y = Mathf.FloorToInt((-localPoint.y + (_gridSystem.height * cellSize) / 2) / cellSize);
            
            // 배치 가능 여부 확인
            if (_gridSystem.CanPlaceTile(tileData, x, y))
            {
                // 그리드에 타일 데이터 배치
                _gridSystem.PlaceTile(tileData, x, y);
                
                // 타일 UI 위치 설정
                transform.SetParent(_gridSystem.gridContainer);
                
                // 회전 상태에 따른 실제 크기 계산
                int width = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.height : tileData.width;
                int height = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.width : tileData.height;
                
                // 타일 위치 조정 (왼쪽 상단 기준점에서 중앙으로 조정)
                float tileWidth = width * cellSize;
                float tileHeight = height * cellSize;
                
                Vector2 placedPosition = new Vector2(
                    x * cellSize + tileWidth / 2,
                    -y * cellSize - tileHeight / 2
                );
                
                _rectTransform.anchoredPosition = placedPosition;
                
                return true;
            }
            
            return false;
        }
    }
}