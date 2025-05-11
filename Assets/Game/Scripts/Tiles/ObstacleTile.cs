using Game.Scripts.Core;
using UnityEngine;

namespace Game.Scripts.Tiles
{
    /// <summary>
    /// 장애물 타일 클래스 - 플레이어가 이동할 수 없는 칸 생성
    /// </summary>
    public class ObstacleTile : BaseTile
    {
        private float _duration = 5f;
        private GridSystem _gridSystem;
        private int _gridX, _gridY;
        private bool _isObstacleActive = false;
        
        /// <summary>
        /// 장애물 지속 시간 프로퍼티
        /// </summary>
        public float Duration { get => _duration; set => _duration = value; }
        
        private void Start()
        {
            _gridSystem = FindAnyObjectByType<GridSystem>();
            
            // 그리드 내 위치 확인
            if (_gridSystem != null)
            {
                _gridSystem.GetXY(transform.position, out _gridX, out _gridY);
            }
        }
        
        /// <summary>
        /// 타일 발동 - 해당 칸을 이동 불가능한 상태로 변경
        /// </summary>
        public override void Activate()
        {
            base.Activate();
            
            if (GetState() == TileState.Activated && _gridSystem != null && !_isObstacleActive)
            {
                ActivateObstacle();
            }
        }
        
        /// <summary>
        /// 장애물 활성화 - 해당 그리드 셀을 이동 불가 상태로 설정
        /// </summary>
        private void ActivateObstacle()
        {
            _isObstacleActive = true;
            
            // 그리드 시스템에 해당 칸 이동 불가 설정
            _gridSystem.SetCellBlocked(_gridX, _gridY, true);
            
            // 시각 효과 표시
            ShowObstacleEffect(true);
            
            // 지속 시간 이후 장애물 제거 예약
            Invoke("DeactivateObstacle", _duration);
            
            Debug.Log($"장애물 활성화: 위치({_gridX}, {_gridY}), 지속시간: {_duration}초");
        }
        
        /// <summary>
        /// 장애물 비활성화
        /// </summary>
        private void DeactivateObstacle()
        {
            if (_isObstacleActive)
            {
                _isObstacleActive = false;
                
                // 그리드 시스템에서 이동 불가 상태 해제
                if (_gridSystem != null)
                {
                    _gridSystem.SetCellBlocked(_gridX, _gridY, false);
                }
                
                // 시각 효과 제거
                ShowObstacleEffect(false);
                
                Debug.Log($"장애물 비활성화: 위치({_gridX}, {_gridY})");
            }
        }
        
        /// <summary>
        /// 장애물 효과 시각화
        /// </summary>
        private void ShowObstacleEffect(bool active)
        {
            // 장애물 시각화 (색상 변경, 파티클 효과 등)
            SpriteRenderer renderer = GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                if (active)
                {
                    // 활성화 시 색상 (더 진한 색으로 변경)
                    renderer.color = new Color(0.7f, 0.2f, 0.2f, 0.8f);
                }
                else
                {
                    // 비활성화 시 원래 색상으로 복원
                    renderer.color = new Color(1f, 1f, 1f, 1f);
                }
            }
            
            // 추가 파티클 효과 등을 여기에 구현
        }
        
        private void OnDestroy()
        {
            // 파괴될 때 그리드 상태 복원
            if (_isObstacleActive && _gridSystem != null)
            {
                _gridSystem.SetCellBlocked(_gridX, _gridY, false);
            }
        }
    }
}