using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 게임의 격자 시스템을 관리하는 클래스
/// </summary>
public class GridSystem : MonoBehaviour
{
    private int width = 8;
    private int height = 8;
    private float cellSize = 1f;
    
    private BaseTile[,] grid;

    // Getters & Setters
    public int Width { get => width; set => width = value; }
    public int Height { get => height; set => height = value; }
    public float CellSize { get => cellSize; set => cellSize = value; }
    
    /// <summary>
    /// 초기화
    /// </summary>
    void Awake()
    {
        grid = new BaseTile[width, height];
    }
    
    /// <summary>
    /// 격자 좌표를 월드 위치로 변환
    /// </summary>
    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y, 0) * cellSize;
    }
    
    /// <summary>
    /// 월드 위치를 격자 좌표로 변환
    /// </summary>
    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.y / cellSize);
    }
    
    /// <summary>
    /// 특정 위치에 타일 등록
    /// </summary>
    public void RegisterTile(BaseTile tile, int x, int y)
    {
        if (IsValidPosition(x, y))
        {
            grid[x, y] = tile;
        }
    }
    
    /// <summary>
    /// 특정 위치의 타일 반환
    /// </summary>
    public BaseTile GetTileAt(int x, int y)
    {
        if (IsValidPosition(x, y))
        {
            return grid[x, y];
        }
        return null;
    }
    
    /// <summary>
    /// 좌표가 격자 범위 내인지 확인
    /// </summary>
    public bool IsValidPosition(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }
}