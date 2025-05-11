using Game.Scripts.Tiles;
using UnityEngine;

namespace Game.Scripts.Core
{
    /// <summary>
    /// 게임의 격자 시스템을 관리하는 클래스
    /// </summary>
    public class GridSystem : MonoBehaviour
    {
        private int _width = 8;
        private int _height = 8;
        private float _cellSize = 1f;
        
        private BaseTile[,] _grid;
        private bool[,] _blockedCells; // 이동 불가 셀 상태
    
        // Getters & Setters
        public int Width { get => _width; set => _width = value; }
        public int Height { get => _height; set => _height = value; }
        public float CellSize { get => _cellSize; set => _cellSize = value; }
        
        /// <summary>
        /// 초기화
        /// </summary>
        void Awake()
        {
            _grid = new BaseTile[_width, _height];
            _blockedCells = new bool[_width, _height];
        }
        
        /// <summary>
        /// 격자 좌표를 월드 위치로 변환
        /// </summary>
        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y, 0) * _cellSize;
        }
        
        /// <summary>
        /// 월드 위치를 격자 좌표로 변환
        /// </summary>
        public void GetXY(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt(worldPosition.x / _cellSize);
            y = Mathf.FloorToInt(worldPosition.y / _cellSize);
        }
        
        /// <summary>
        /// 특정 위치에 타일 등록
        /// </summary>
        public void RegisterTile(BaseTile tile, int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                _grid[x, y] = tile;
            }
        }
        
        /// <summary>
        /// 특정 위치의 타일 반환
        /// </summary>
        public BaseTile GetTileAt(int x, int y)
        {
            if (IsValidPosition(x, y))
            {
                return _grid[x, y];
            }
            return null;
        }
        
        /// <summary>
        /// 좌표가 격자 범위 내이고 이동 가능한지 확인
        /// </summary>
        public bool IsValidPosition(int x, int y)
        {
            // 그리드 범위 체크
            bool isInBounds = x >= 0 && y >= 0 && x < _width && y < _height;
            
            // 범위 내에 있고 차단되지 않은 셀인지 확인
            return isInBounds && !IsBlocked(x, y);
        }
        
        /// <summary>
        /// 특정 위치가 차단되었는지 확인
        /// </summary>
        public bool IsBlocked(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                return _blockedCells[x, y];
            }
            // 그리드 바깥은 차단된 것으로 간주
            return true;
        }
        
        /// <summary>
        /// 특정 셀의 이동 가능 상태 설정
        /// </summary>
        public void SetCellBlocked(int x, int y, bool blocked)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                _blockedCells[x, y] = blocked;
                Debug.Log($"셀 차단 상태 변경: ({x}, {y}) -> {blocked}");
            }
        }
    }
}