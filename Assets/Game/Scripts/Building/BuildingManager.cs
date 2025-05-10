using System.Collections.Generic;
using UnityEngine;

namespace Game.Scripts.Building
{
    /// <summary>
    /// 타일 배치 데이터 클래스
    /// </summary>
    [System.Serializable]
    public class TilePlacementData
    {
        public TileData tileData;
        public int x;
        public int y;
    }
    
    /// <summary>
    /// 빌딩 관리 클래스 (인벤토리에서 게임으로 데이터 전달)
    /// </summary>
    public static class BuildingManager
    {
        // 배치된 타일 목록 (씬 간 데이터 전달용)
        public static List<TilePlacementData> PlacedTiles = new List<TilePlacementData>();
    }
}