using Game.Scripts.Building;
using UnityEngine;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 인벤토리 아이템 데이터 클래스
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    public class InventoryItemData : ScriptableObject
    {
        [Header("아이템 기본 정보")]
        [SerializeField] private string _itemName = "New Item";
        [SerializeField] private Sprite _itemSprite;
        [SerializeField] private Vector2 _spritePivot = new Vector2(0.5f, 0.5f); // 스프라이트 중심점 (0~1)
        
        [Header("아이템 형태 정보")]
        [SerializeField] private bool[,] _shapeData = new bool[1, 1] { { true } }; // 기본값은 1x1
        [SerializeField] private TileData.TileType _tileType = TileData.TileType.Attack;
        
        [Header("아이템 속성")]
        [SerializeField] private int _damage = 10;
        [SerializeField] private int _defense = 0;
        [SerializeField] private int _buffValue = 0;
        [SerializeField] private int _cost = 1;
        
        /// <summary>
        /// 아이템 이름
        /// </summary>
        public string ItemName { get => _itemName; set => _itemName = value; }
        
        /// <summary>
        /// 아이템 스프라이트
        /// </summary>
        public Sprite ItemSprite { get => _itemSprite; set => _itemSprite = value; }
        
        /// <summary>
        /// 스프라이트 중심점 (0~1)
        /// </summary>
        public Vector2 SpritePivot { get => _spritePivot; set => _spritePivot = value; }
        
        /// <summary>
        /// 아이템 형태 데이터
        /// </summary>
        public bool[,] ShapeData { get => _shapeData; set => _shapeData = value; }
        
        /// <summary>
        /// 타일 타입
        /// </summary>
        public TileData.TileType TileType { get => _tileType; set => _tileType = value; }
        
        /// <summary>
        /// 아이템 공격력
        /// </summary>
        public int Damage { get => _damage; set => _damage = value; }
        
        /// <summary>
        /// 아이템 방어력
        /// </summary>
        public int Defense { get => _defense; set => _defense = value; }
        
        /// <summary>
        /// 아이템 버프 값
        /// </summary>
        public int BuffValue { get => _buffValue; set => _buffValue = value; }
        
        /// <summary>
        /// 아이템 비용
        /// </summary>
        public int Cost { get => _cost; set => _cost = value; }
        
        /// <summary>
        /// 아이템 형태 너비
        /// </summary>
        public int Width
        {
            get
            {
                return _shapeData.GetLength(1);
            }
        }
        
        /// <summary>
        /// 아이템 형태 높이
        /// </summary>
        public int Height
        {
            get
            {
                return _shapeData.GetLength(0);
            }
        }
        
        /// <summary>
        /// 아이템 복제본 생성
        /// </summary>
        public InventoryItemData Clone()
        {
            InventoryItemData clone = CreateInstance<InventoryItemData>();
            clone.ItemName = this.ItemName;
            clone.ItemSprite = this.ItemSprite;
            clone.SpritePivot = this.SpritePivot;
            clone.TileType = this.TileType;
            clone.Damage = this.Damage;
            clone.Defense = this.Defense;
            clone.BuffValue = this.BuffValue;
            clone.Cost = this.Cost;
            
            // 형태 데이터 복사
            bool[,] shapeClone = new bool[Height, Width];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    shapeClone[y, x] = _shapeData[y, x];
                }
            }
            clone.ShapeData = shapeClone;
            
            return clone;
        }
        
        /// <summary>
        /// 아이템 형태 데이터 회전 (90도)
        /// </summary>
        public void Rotate90Degrees()
        {
            int oldHeight = Height;
            int oldWidth = Width;
            bool[,] rotatedShape = new bool[oldWidth, oldHeight];
    
            // 90도 회전 변환
            for (int y = 0; y < oldHeight; y++)
            {
                for (int x = 0; x < oldWidth; x++)
                {
                    // (y, x) -> (x, height-1-y)
                    rotatedShape[x, oldHeight - 1 - y] = _shapeData[y, x];
                }
            }
    
            _shapeData = rotatedShape;
    
            // 디버깅
            Debug.Log($"회전: {oldWidth}x{oldHeight} -> {Height}x{Width}");
            string shapeStr = "";
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    shapeStr += _shapeData[y, x] ? "■" : "□";
                }
                shapeStr += "\n";
            }
            Debug.Log(shapeStr);
        }
    }
}