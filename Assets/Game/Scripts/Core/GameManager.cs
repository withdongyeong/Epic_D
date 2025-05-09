using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject attackTilePrefab;
    private GridSystem gridSystem;
    
    private void Start()
    {
        gridSystem = GetComponent<GridSystem>();
        InitializeGrid();
        SpawnPlayer();
    }
    
    private void InitializeGrid()
    {
        // 테스트용 타일 배치
        SpawnTile(attackTilePrefab, 2, 2);
        SpawnTile(attackTilePrefab, 4, 3);
        SpawnTile(attackTilePrefab, 1, 5);
    }
    
    /// <summary>
    /// 타일 생성 및 그리드 등록
    /// </summary>
    private void SpawnTile(GameObject tilePrefab, int x, int y)
    {
        Vector3 position = gridSystem.GetWorldPosition(x, y);
        GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity);
        BaseTile tile = tileObj.GetComponent<BaseTile>();
        gridSystem.RegisterTile(tile, x, y);
        Debug.Log($"타일 등록: ({x}, {y}), 타일: {tile}");
    }
    
    private void SpawnPlayer()
    {
        Vector3 position = gridSystem.GetWorldPosition(0, 0);
        Instantiate(playerPrefab, position, Quaternion.identity);
    }
}