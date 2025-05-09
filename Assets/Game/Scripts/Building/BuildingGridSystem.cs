using Game.Scripts.Building.Game.Scripts.Building;
using UnityEngine;

namespace Game.Scripts.Building
{
    /// <summary>
    /// 빌딩 씬의 8x8 인벤토리 그리드 시스템
    /// </summary>
    public class BuildingGridSystem : MonoBehaviour
    {
        [Header("그리드 설정")]
        public int width = 8;
        public int height = 8;
        public float cellSize = 64f; // UI 그리드 셀 크기 (픽셀)
        
        [Header("UI 요소")]
        public GameObject gridCellPrefab; // 그리드 셀 프리팹
        public Transform gridContainer; // 그리드 셀을 담을 컨테이너
        
        private GridCell[,] _grid; // 그리드 셀 배열
        private TileData[,] _tileDataGrid; // 배치된 타일 데이터
        
        // 싱글톤 패턴 구현
        private static BuildingGridSystem _instance;
        public static BuildingGridSystem Instance
        {
            get { return _instance; }
        }
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            InitializeGrid();
        }
        
        /// <summary>
        /// 그리드 초기화 및 UI 생성
        /// </summary>
        private void InitializeGrid()
        {
            _grid = new GridCell[width, height];
            _tileDataGrid = new TileData[width, height];
            
            // 그리드 UI 생성
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // 그리드 셀 생성
                    GameObject cellObj = Instantiate(gridCellPrefab, gridContainer);
                    GridCell cell = cellObj.GetComponent<GridCell>();
                    
                    // 셀 초기화
                    if (cell != null)
                    {
                        cell.SetPosition(x, y);
                        
                        // RectTransform 위치 설정
                        RectTransform rectTransform = cellObj.GetComponent<RectTransform>();
                        if (rectTransform != null)
                        {
                            rectTransform.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);
                        }
                        
                        // 그리드에 저장
                        _grid[x, y] = cell;
                    }
                }
            }
        }
        
        /// <summary>
        /// 그리드 위치가 유효한지 확인
        /// </summary>
        public bool IsValidPosition(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }
        
        /// <summary>
        /// 특정 위치의 타일 데이터 반환
        /// </summary>
        public TileData GetTileDataAt(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return _tileDataGrid[x, y];
            }
            return null;
        }
        
        /// <summary>
        /// 특정 위치의 셀 반환
        /// </summary>
        public GridCell GetCellAt(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return _grid[x, y];
            }
            return null;
        }
        
        /// <summary>
        /// 타일 배치 가능 여부 확인
        /// </summary>
        public bool CanPlaceTile(TileData tileData, int startX, int startY)
        {
            // 타일 회전 상태에 따른 실제 크기 계산
            int tileWidth = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.height : tileData.width;
            int tileHeight = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.width : tileData.height;
            
            // 범위 체크
            if (startX < 0 || startY < 0 || startX + tileWidth > this.width || startY + tileHeight > this.height)
            {
                return false;
            }
            
            // 해당 위치에 이미 타일이 있는지 확인
            for (int y = 0; y < tileHeight; y++)
            {
                for (int x = 0; x < tileWidth; x++)
                {
                    if (_grid[startX + x, startY + y].isOccupied)
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// 타일 배치
        /// </summary>
        public void PlaceTile(TileData tileData, int startX, int startY)
        {
            if (!CanPlaceTile(tileData, startX, startY))
            {
                return;
            }
            
            // 타일 회전 상태에 따른 실제 크기 계산
            int tileWidth = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.height : tileData.width;
            int tileHeight = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.width : tileData.height;
            
            // 그리드에 타일 데이터 등록 및 셀 점유 상태 설정
            for (int y = 0; y < tileHeight; y++)
            {
                for (int x = 0; x < tileWidth; x++)
                {
                    _tileDataGrid[startX + x, startY + y] = tileData;
                    _grid[startX + x, startY + y].SetOccupied(true);
                }
            }
            
            Debug.Log($"타일 배치 완료: {startX},{startY} 크기: {tileWidth}x{tileHeight}");
        }
        
        /// <summary>
        /// 타일 제거
        /// </summary>
        public void RemoveTile(int startX, int startY)
        {
            TileData tileData = GetTileDataAt(startX, startY);
            
            if (tileData == null)
            {
                return;
            }
            
            // 타일 회전 상태에 따른 실제 크기 계산
            int tileWidth = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.height : tileData.width;
            int tileHeight = tileData.rotation == 90 || tileData.rotation == 270 ? tileData.width : tileData.height;
            
            // 타일이 차지하는 모든 셀에서 데이터 제거
            for (int y = 0; y < tileHeight; y++)
            {
                for (int x = 0; x < tileWidth; x++)
                {
                    if (IsValidPosition(startX + x, startY + y))
                    {
                        _tileDataGrid[startX + x, startY + y] = null;
                        _grid[startX + x, startY + y].SetOccupied(false);
                    }
                }
            }
        }
        
        /// <summary>
        /// 전체 그리드 데이터 반환 (게임 씬으로 전달용)
        /// </summary>
        public TileData[,] GetGridData()
        {
            return _tileDataGrid;
        }
        
        /// <summary>
        /// 그리드 하이라이트 표시
        /// </summary>
        public void HighlightCells(int startX, int startY, int width, int height, bool isValidPlacement)
        {
            // 모든 셀 하이라이트 초기화
            ResetHighlights();
            
            // 타일이 차지할 영역 하이라이트
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int currentX = startX + x;
                    int currentY = startY + y;
                    
                    if (IsValidPosition(currentX, currentY))
                    {
                        GridCell cell = _grid[currentX, currentY];
                        cell.MarkInvalid(!isValidPlacement);
                    }
                }
            }
        }
        
        /// <summary>
        /// 그리드 하이라이트 초기화
        /// </summary>
        public void ResetHighlights()
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    _grid[x, y].ResetHighlight();
                }
            }
        }
    }
}