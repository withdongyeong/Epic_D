namespace Game.Scripts.Characters.Player
{ 
    using System.Collections;
    using UnityEngine;
    using Core;
    using Tiles;

    /// <summary>
    /// 플레이어 이동 및 타일 상호작용 관리
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        private float _moveSpeed = 5f;
        private GridSystem _gridSystem;
        private int _currentX, _currentY;
        private bool _isMoving;
        private float _moveTime = 0.2f;
        
        // Getters & Setters
        public float MoveSpeed { get => _moveSpeed; set => _moveSpeed = value; }
        public bool IsMoving { get => _isMoving; }
        
        private void Start()
        {
            _gridSystem = FindAnyObjectByType<GridSystem>();
            UpdateCurrentPosition();
        }
        
        private void Update()
        {
            if (!_isMoving)
            {
                HandleMovement();
            }
            CheckTileInteraction();
        }
        
        private void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
        
            if (horizontal != 0 || vertical != 0)
            {
                int dx = Mathf.RoundToInt(horizontal);
                int dy = Mathf.RoundToInt(vertical);
                TryMove(dx, dy);
            }
        }
        
        /// <summary>
        /// 이동 시도 - 유효한 위치인지 확인 후 애니메이션
        /// </summary>
        private bool TryMove(int dx, int dy)
        {
            int newX = _currentX + dx;
            int newY = _currentY + dy;
            
            if (_gridSystem.IsValidPosition(newX, newY))
            {
                StartCoroutine(MoveAnimation(newX, newY));
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// 점프 효과가 있는 이동 애니메이션
        /// </summary>
        private IEnumerator MoveAnimation(int targetX, int targetY)
        {
            _isMoving = true;
        
            Vector3 startPos = transform.position;
            Vector3 targetPos = _gridSystem.GetWorldPosition(targetX, targetY);
            float jumpHeight = 0.3f;
        
            float elapsedTime = 0;
        
            while (elapsedTime < _moveTime)
            {
                float t = elapsedTime / _moveTime;
            
                // XY 평면에서 이동 (Z는 항상 0)
                float x = Mathf.Lerp(startPos.x, targetPos.x, t);
                float y = Mathf.Lerp(startPos.y, targetPos.y, t);
            
                // 추가 점프 높이
                float extraHeight = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            
                transform.position = new Vector3(x, y + extraHeight, 0);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        
            transform.position = targetPos;
            _currentX = targetX;
            _currentY = targetY;
            _isMoving = false;
        }
        
        /// <summary>
        /// 현재 격자 위치 업데이트
        /// </summary>
        private void UpdateCurrentPosition()
        {
            _gridSystem.GetXY(transform.position, out _currentX, out _currentY);
        }
        
        /// <summary>
        /// 현재 위치 타일과 상호작용 확인
        /// </summary>
        private void CheckTileInteraction()
        {
            BaseTile currentTile = _gridSystem.GetTileAt(_currentX, _currentY);
        
            if (currentTile != null && currentTile.GetState() == BaseTile.TileState.Ready)
            {
                currentTile.Activate();
            }
        }
    }   
}