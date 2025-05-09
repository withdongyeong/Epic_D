namespace Game.Scripts.Building
{
    using UnityEngine;

    namespace Game.Scripts.Building
    {
        /// <summary>
        /// 타일 데이터 클래스 - 빌딩 시스템에서 타일 정보 저장
        /// </summary>
        [System.Serializable]
        public class TileData
        {
            public enum TileType
            {
                Attack,
                Defense,
                Buff,
                Special
            }

            public TileType type;
            public string tileName;
            public int width = 1;
            public int height = 1;
            public int damage;
            public GameObject tilePrefab;
            public Sprite tileSprite;

            // 회전 상태 (0, 90, 180, 270)
            public int rotation = 0;

            public TileData(TileType type, string name, int width, int height, int damage = 10)
            {
                this.type = type;
                this.tileName = name;
                this.width = width;
                this.height = height;
                this.damage = damage;
            }
        }
    }
}