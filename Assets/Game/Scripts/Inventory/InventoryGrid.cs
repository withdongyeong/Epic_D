using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 인벤토리 그리드 시스템 클래스
    /// </summary>
    public class InventoryGrid : MonoBehaviour
    {
        [Header("그리드 설정")]
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 8;
        [SerializeField] private float _cellSize = 64f;
        
        [Header("시각적 설정")]
        [SerializeField] private RectTransform _gridContainer;
        [SerializeField] private GameObject _cellHighlightPrefab; // 셀 하이라이트 프리팹
        [SerializeField] private GameObject _cellBasePrefab; // 기본 프리팹
        [SerializeField] private Color _validPlacementColor = Color.green;
        [SerializeField] private Color _invalidPlacementColor = Color.red;
        
        private InventoryItem[,] _grid; // 그리드 내 아이템 상태
        private List<GameObject> _highlightCells = new List<GameObject>(); // 현재 활성화된 하이라이트 셀들
        private List<GameObject> _visualizationCells = new List<GameObject>();
        
        // 이벤트 정의
        public event Action OnGridChanged;
        
        /// <summary>
        /// 그리드 너비 프로퍼티
        /// </summary>
        public int Width { get => _width; }
        
        /// <summary>
        /// 그리드 높이 프로퍼티
        /// </summary>
        public int Height { get => _height; }
        
        /// <summary>
        /// 셀 크기 프로퍼티
        /// </summary>
        public float CellSize { get => _cellSize; }
        
        /// <summary>
        /// 그리드 컨테이너 프로퍼티
        /// </summary>
        public RectTransform GridContainer { get => _gridContainer; }
        
        private void Awake()
        {
            InitializeGrid();
        }
        
        private void Start()
        {
            // 그리드 셀 시각화
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    // 셀 하이라이트 생성
                    GameObject cellHighlight = Instantiate(_cellBasePrefab, _gridContainer);
                    RectTransform rectTransform = cellHighlight.GetComponent<RectTransform>();
                    rectTransform.anchoredPosition = GetCellPosition(x, y);
                    rectTransform.sizeDelta = new Vector2(_cellSize, _cellSize);
            
                    // 체스판 패턴 색상 설정
                    Image image = cellHighlight.GetComponent<Image>();
                    if (image != null)
                    {
                        // 체스판 패턴으로 색상 설정
                        bool isEvenCell = (x + y) % 2 == 0;
                        image.color = isEvenCell ? 
                            new Color(0.8f, 0.8f, 0.8f, 0.3f) : // 밝은 셀
                            new Color(0.5f, 0.5f, 0.5f, 0.2f);  // 어두운 셀
                    }
                }
            }
        }
        
        /// <summary>
        /// 그리드 초기화
        /// </summary>
        private void InitializeGrid()
        {
            _grid = new InventoryItem[_width, _height];
    
            // 그리드 컨테이너 크기 설정
            if (_gridContainer != null)
            {
                _gridContainer.sizeDelta = new Vector2(_width * _cellSize, _height * _cellSize);
            }
            else
            {
                Debug.LogError("그리드 컨테이너가 null입니다");
            }
        }
        
        /// <summary>
        /// 아이템을 그리드에 배치할 수 있는지 확인
        /// </summary>
        public bool CanPlaceItem(InventoryItem item, int gridX, int gridY)
        {
            if (item == null || item.ItemData == null)
            {
                Debug.LogError("아이템 또는 아이템 데이터가 null입니다");
                return false;
            }
            
            // 그리드 범위 체크
            if (gridX < 0 || gridY < 0 || 
                gridX + item.Width > _width || 
                gridY + item.Height > _height)
            {
                return false;
            }
    
            // 아이템 형태에 따라 각 셀 체크
            for (int y = 0; y < item.Height; y++)
            {
                for (int x = 0; x < item.Width; x++)
                {
                    // 아이템 형태 데이터에서 현재 위치에 셀이 있는지 확인
                    if (item.ShapeData[y, x])
                    {
                        // 해당 셀이 이미 점유되었는지 확인
                        if (_grid[gridX + x, gridY + y] != null)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        
        /// <summary>
        /// 아이템을 그리드에 배치
        /// </summary>
        public bool PlaceItem(InventoryItem item, int gridX, int gridY)
        {
            if (!CanPlaceItem(item, gridX, gridY))
            {
                Debug.Log($"배치 실패: 위치({gridX}, {gridY})에 아이템 배치 불가");
                return false;
            }
    
            Debug.Log($"배치 성공: 위치({gridX}, {gridY})에 아이템 배치됨");
    
            // 아이템 형태에 따라 각 셀 점유
            for (int y = 0; y < item.Height; y++)
            {
                for (int x = 0; x < item.Width; x++)
                {
                    if (item.ShapeData[y, x])
                    {
                        _grid[gridX + x, gridY + y] = item;
                    }
                }
            }
    
            // 아이템의 그리드 위치 저장
            item.SetGridPosition(gridX, gridY);
    
            // 이벤트 발생
            OnGridChanged?.Invoke();
            
            // 배치 후 그리드 시각화 업데이트
            VisualizeGrid();
    
            return true;
        }
        /// <summary>
        /// 그리드 시각화
        /// </summary>
        public void VisualizeGrid()
        {
            // 기존 시각화 제거
            ClearGridVisualization();
    
            // 그리드 셀 상태 시각화
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_grid[x, y] != null)
                    {
                        // 점유된 셀 표시
                        GameObject highlight = Instantiate(_cellHighlightPrefab, _gridContainer);
                        RectTransform rectTransform = highlight.GetComponent<RectTransform>();
                        rectTransform.anchoredPosition = GetCellPosition(x, y);
                        rectTransform.sizeDelta = new Vector2(_cellSize, _cellSize);
                
                        // 색상 설정 (점유됨)
                        Image image = highlight.GetComponent<Image>();
                        if (image != null)
                        {
                            image.color = new Color(0.8f, 0.2f, 0.2f, 0.5f); // 빨간색 (점유)
                        }
                
                        _visualizationCells.Add(highlight);
                    }
                }
            }
        }

        // 시각화 제거 메서드
        private void ClearGridVisualization()
        {
            foreach (GameObject cell in _visualizationCells)
            {
                Destroy(cell);
            }
            _visualizationCells.Clear();
        }
        
        /// <summary>
        /// 아이템을 그리드에서 제거
        /// </summary>
        /// <param name="item">제거할 아이템</param>
        public void RemoveItem(InventoryItem item)
        {
            if (item == null) return;
            
            int gridX = item.GridX;
            int gridY = item.GridY;
            
            // 아이템이 차지하는 모든 셀을 비움
            for (int y = 0; y < item.Height; y++)
            {
                for (int x = 0; x < item.Width; x++)
                {
                    if (gridX + x >= 0 && gridX + x < _width && 
                        gridY + y >= 0 && gridY + y < _height &&
                        item.ShapeData[y, x])
                    {
                        _grid[gridX + x, gridY + y] = null;
                    }
                }
            }
            
            // 이벤트 발생
            OnGridChanged?.Invoke();
        }
        /// <summary>
        /// 테스트용
        /// </summary>
        public void PrintGridState()
        {
            string gridState = "그리드 상태:\n";
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    gridState += (_grid[x, y] != null) ? "■" : "□";
                }
                gridState += "\n";
            }
            Debug.Log(gridState);
        }
        
        /// <summary>
        /// 모든 하이라이트 셀 제거
        /// </summary>
        public void ResetHighlights()
        {
            foreach (GameObject highlight in _highlightCells)
            {
                Destroy(highlight);
            }
            
            _highlightCells.Clear();
        }
        
        /// <summary>
        /// 셀 하이라이트 활성화
        /// </summary>
        /// <param name="gridX">그리드 X 좌표</param>
        /// <param name="gridY">그리드 Y 좌표</param>
        /// <param name="width">하이라이트 너비</param>
        /// <param name="height">하이라이트 높이</param>
        /// <param name="valid">유효한 배치인지 여부</param>
        public void HighlightCells(int gridX, int gridY, bool[,] shape, bool valid)
        {
            ResetHighlights();
    
            if (_cellHighlightPrefab == null || _gridContainer == null)
                return;
        
            int shapeHeight = shape.GetLength(0);
            int shapeWidth = shape.GetLength(1);
            
            // 형태 데이터에 맞게 하이라이트 셀 생성
            for (int y = 0; y < shapeHeight; y++)
            {
                for (int x = 0; x < shapeWidth; x++)
                {
                    if (shape[y, x])
                    {
                        int cellX = gridX + x;
                        int cellY = gridY + y;
                
                        // 그리드 범위 내에 있는지 확인
                        if (cellX >= 0 && cellX < _width && cellY >= 0 && cellY < _height)
                        {
                            // 하이라이트 셀 생성
                            GameObject highlight = Instantiate(_cellHighlightPrefab, _gridContainer);
                    
                            // 위치 설정
                            RectTransform rectTransform = highlight.GetComponent<RectTransform>();
                            rectTransform.anchoredPosition = GetCellPosition(cellX, cellY);
                            rectTransform.sizeDelta = new Vector2(_cellSize, _cellSize);
                    
                            // 색상 설정
                            Image image = highlight.GetComponent<Image>();
                            if (image != null)
                            {
                                Color color = Color.blue;
                                color.a = 1f;
                                image.color = color;
                            }
                    
                            _highlightCells.Add(highlight);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// 그리드 셀 위치 계산
        /// </summary>
        public Vector2 GetCellPosition(int gridX, int gridY)
        {
            // 그리드의 왼쪽 상단 모서리 좌표 (로컬 기준)
            float leftEdge = -_gridContainer.rect.width / 2;
            float topEdge = _gridContainer.rect.height / 2;
    
            // 셀 중앙 위치 계산
            float posX = leftEdge + (gridX + 0.5f) * _cellSize;
            float posY = topEdge - (gridY + 0.5f) * _cellSize;
    
            return new Vector2(posX, posY);
        }
        
        /// <summary>
        /// 마우스의 그리드 좌표 계산
        /// </summary>
        public bool TryGetGridCoordinates(Vector2 mousePosition, Camera camera, out int gridX, out int gridY)
        {
            // 그리드 월드 좌표 계산
            Vector3[] corners = new Vector3[4];
            _gridContainer.GetWorldCorners(corners);
    
            // 마우스 위치에서 그리드 좌측 상단 좌표를 빼서 상대적 위치 계산
            Vector2 relativePos = mousePosition - new Vector2(corners[0].x, corners[0].y);

            // 그리드 인덱스 계산
            gridX = (int)(relativePos.x / 64);
    
            // Y축 반전
            int totalRows = 8; // 그리드 행 수
            gridY = totalRows - 1 - (int)(relativePos.y / 64);

            return true;
        }
        
        /// <summary>
        /// 그리드 내 아이템 목록 반환
        /// </summary>
        public List<InventoryItem> GetAllItems()
        {
            HashSet<InventoryItem> uniqueItems = new HashSet<InventoryItem>();
            
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    if (_grid[x, y] != null)
                    {
                        uniqueItems.Add(_grid[x, y]);
                    }
                }
            }
            
            return new List<InventoryItem>(uniqueItems);
        }
    }
}