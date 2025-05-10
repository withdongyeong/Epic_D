using UnityEngine;

namespace Game.Scripts.Building
{
    /// <summary>
    /// 타일 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "New Tile", menuName = "Building/Tile Data")]
    public class TileData : ScriptableObject
    {
        public enum TileType
        {
            Attack,
            Defense,
            Buff
        }
        
        [Header("타일 기본 정보")]
        public string tileName = "New Tile";
        public TileType type = TileType.Attack;
        public Sprite tileSprite;
        
        [Header("타일 크기")]
        public int width = 1;
        public int height = 1;
        
        [Header("타일 회전")]
        public int rotation = 0; // 0, 90, 180, 270
        
        [Header("타일 속성")]
        public int damage = 10;
        public int defense = 0;
        public int buffValue = 0;
        
        /// <summary>
        /// 타일 복제본 생성
        /// </summary>
        public TileData Clone()
        {
            TileData clone = CreateInstance<TileData>();
            clone.tileName = this.tileName;
            clone.type = this.type;
            clone.tileSprite = this.tileSprite;
            clone.width = this.width;
            clone.height = this.height;
            clone.rotation = this.rotation;
            clone.damage = this.damage;
            clone.defense = this.defense;
            clone.buffValue = this.buffValue;
            
            return clone;
        }
    }
}