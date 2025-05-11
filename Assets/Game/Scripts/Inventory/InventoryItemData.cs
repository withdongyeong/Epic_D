using UnityEngine;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 인벤토리 아이템 데이터 클래스
    /// </summary>
    ///
    public enum TileType
    {
        Attack,
        Defense,
        Heal
    }
    
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
    public class InventoryItemData : ScriptableObject
    {
        [Header("아이템 기본 정보")]
        [SerializeField] private string _itemName = "New Item";
        [SerializeField] private Sprite _itemSprite;
        [SerializeField] private Vector2 _spritePivot = new Vector2(0.5f, 0.5f); // 스프라이트 중심점 (0~1)
        
        [Header("아이템 형태 정보")]
        [SerializeField] private ItemShapeType _shapeType = ItemShapeType.Single; // 형태 타입
        [SerializeField] private bool[,] _shapeData = new bool[1, 1] { { true } }; // 기본값은 1x1
        [SerializeField] private TileType _tileType = TileType.Attack;
        
        [Header("타일 타이밍 설정")]
        [SerializeField] private float _chargeTime = 3f;
        
        [Header("아이템 속성")]
        [SerializeField] private int _damage = 10;        // 공격 타일용
        [SerializeField] private float _invincibilityDuration = 3f; // 방어 타일 무적 지속시간
        [SerializeField] private int _healAmount = 25;    // 회복 타일용
        [SerializeField] private float _obstacleDuration = 5f; // 장애물 타일용
        [SerializeField] private int _cost = 1;
        
        
        // 현재 회전 상태 (0, 90, 180, 270)
        private int _currentRotation = 0;
        
        public int CurrentRotation { get => _currentRotation; }

        
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
        /// 형태 타입
        /// </summary>
        public ItemShapeType ShapeType { get => _shapeType; set => _shapeType = value; }
        
        /// <summary>
        /// 아이템 형태 데이터
        /// </summary>
        public bool[,] ShapeData { get => _shapeData; set => _shapeData = value; }
        
        /// <summary>
        /// 타일 타입
        /// </summary>
        public TileType TileType { get => _tileType; set => _tileType = value; }
        
        /// <summary>
        /// 타일 충전 시간
        /// </summary>
        public float ChargeTime { get => _chargeTime; set => _chargeTime = value; }
        
        /// <summary>
        /// 아이템 공격력
        /// </summary>
        public int Damage { get => _damage; set => _damage = value; }
        
        /// <summary>
        /// 무적 지속시간
        /// </summary>
        public float InvincibilityDuration { get => _invincibilityDuration; set => _invincibilityDuration = value; }
        
        /// <summary>
        /// 아이템 회복량
        /// </summary>
        public int HealAmount { get => _healAmount; set => _healAmount = value; }
        
        /// <summary>
        /// 장애물 지속시간
        /// </summary>
        public float ObstacleDuration { get => _obstacleDuration; set => _obstacleDuration = value; }
        
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
        /// ShapeType에 따라 ShapeData 업데이트
        /// </summary>
        public void UpdateShapeFromType()
        {
            switch (_shapeType)
            {
                case ItemShapeType.Single:
                    // 1칸짜리
                    _shapeData = new bool[1, 1] { { true } };
                    break;

                case ItemShapeType.Double:
                    // 2칸짜리 (가로)
                    _shapeData = new bool[1, 2] { { true, true } };
                    break;

                case ItemShapeType.LShape:
                    // 3칸짜리 ㄱ 형태
                    _shapeData = new bool[2, 2] {
                        { true, false },
                        { true, true }
                    };
                    break;

                case ItemShapeType.TetrisL:
                    // 4칸짜리 (기존 L 형태)
                    _shapeData = new bool[2, 3] {
                        { true, false, false },
                        { true, true, true }
                    };
                    break;

                case ItemShapeType.Horizontal4:
                    // 4칸짜리 (가로 4칸)
                    _shapeData = new bool[1, 4] { { true, true, true, true } };
                    break;

                default:
                    _shapeData = new bool[1, 1] { { true } };
                    break;
            }
        }
        
        /// <summary>
        /// OnValidate에서 ShapeType이 변경되면 ShapeData 업데이트
        /// </summary>
        private void OnValidate()
        {
            UpdateShapeFromType();
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
            clone.ShapeType = this.ShapeType;
            clone.TileType = this.TileType;
            clone.ChargeTime = this.ChargeTime;
            clone.Damage = this.Damage;
            clone.InvincibilityDuration = this.InvincibilityDuration;
            clone.HealAmount = this.HealAmount;
            clone.ObstacleDuration = this.ObstacleDuration;
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
            // 회전 상태 갱신 (0 -> 90 -> 180 -> 270 -> 0)
            _currentRotation = (_currentRotation + 90) % 360;
            
            // 현재 ShapeType에 따라 회전된 형태 직접 설정
            switch (_shapeType)
            {
                case ItemShapeType.Single:
                    // 1칸짜리는 회전해도 변화 없음
                    _shapeData = new bool[1, 1] { { true } };
                    break;
                    
                case ItemShapeType.Double:
                    // 2칸짜리 (가로/세로)
                    if (_currentRotation == 0 || _currentRotation == 180)
                    {
                        // 가로 방향 (0도, 180도)
                        _shapeData = new bool[1, 2] { { true, true } };
                    }
                    else
                    {
                        // 세로 방향 (90도, 270도)
                        _shapeData = new bool[2, 1] { { true }, { true } };
                    }
                    break;
                    
                case ItemShapeType.LShape:
                    // 3칸짜리 ㄱ 형태 (4가지 회전 상태)
                    switch (_currentRotation)
                    {
                        case 0: // 기본 ㄱ 형태
                            _shapeData = new bool[2, 2] {
                                { true, false },
                                { true, true }
                            };
                            break;
                        case 90: // 90도 회전
                            _shapeData = new bool[2, 2] {
                                { true, true },
                                { true, false }
                            };
                            break;
                        case 180: // 180도 회전
                            _shapeData = new bool[2, 2] {
                                { true, true },
                                { false, true }
                            };
                            break;
                        case 270: // 270도 회전
                            _shapeData = new bool[2, 2] {
                                { false, true },
                                { true, true }
                            };
                            break;
                    }
                    break;
                    
                case ItemShapeType.TetrisL:
                    // 4칸짜리 L 형태 (4가지 회전 상태)
                    switch (_currentRotation)
                    {
                        case 0: // 기본 L 형태
                            _shapeData = new bool[2, 3] {
                                { true, false, false },
                                { true, true, true }
                            };
                            break;
                        case 90: // 90도 회전
                            _shapeData = new bool[3, 2] {
                                { true, true },
                                { true, false },
                                { true, false }
                            };
                            break;
                        case 180: // 180도 회전
                            _shapeData = new bool[2, 3] {
                                { true, true, true },
                                { false, false, true }
                            };
                            break;
                        case 270: // 270도 회전
                            _shapeData = new bool[3, 2] {
                                { false, true },
                                { false, true },
                                { true, true }
                            };
                            break;
                    }
                    break;
                    
                case ItemShapeType.Horizontal4:
                    // 4칸짜리 가로/세로 (2가지 회전 상태)
                    if (_currentRotation == 0 || _currentRotation == 180)
                    {
                        // 가로 방향 (0도, 180도)
                        _shapeData = new bool[1, 4] { { true, true, true, true } };
                    }
                    else
                    {
                        // 세로 방향 (90도, 270도)
                        _shapeData = new bool[4, 1] { { true }, { true }, { true }, { true } };
                    }
                    break;
            }
        }
        
        /// <summary>
        /// 회전 초기화 (0도로 설정)
        /// </summary>
        public void ResetRotation()
        {
            _currentRotation = 0;
            UpdateShapeFromType();
        }
    }
}
