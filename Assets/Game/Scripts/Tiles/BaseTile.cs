namespace Game.Scripts.Tiles
{
    using UnityEngine;
    using System;

    /// <summary>
    /// 모든 타일의 기본 클래스
    /// </summary>
    public class BaseTile : MonoBehaviour
    {
        public enum TileState
        {
            Charging,  // 충전/쿨다운
            Ready,     // 준비 완료
            Activated  // 발동 중
        }
        
        private float _chargeTime = 3f;
        private float _cooldownTime = 1f;
        
        private TileState _currentState = TileState.Charging;
        private float _stateTimer;
        
        // Getters & Setters
        public float ChargeTime { get => _chargeTime; set => _chargeTime = value; }
        public float CooldownTime { get => _cooldownTime; set => _cooldownTime = value; }
        
        // 이벤트 정의
        public event Action<BaseTile> OnReady;
        public event Action<BaseTile> OnActivated;
        
        protected virtual void Update()
        {
            UpdateState();
        }
        
        /// <summary>
        /// 타일 상태 업데이트 및 상태 전환 관리
        /// </summary>
        protected virtual void UpdateState()
        {
            switch (_currentState)
            {
                case TileState.Charging:
                    _stateTimer += Time.deltaTime;
                    if (_stateTimer >= _chargeTime)
                    {
                        SetState(TileState.Ready);
                    }
                    break;
            
                case TileState.Ready:
                    // 준비 상태 유지
                    break;
            
                case TileState.Activated:
                    // 발동 후 즉시 충전 상태로
                    SetState(TileState.Charging);
                    _stateTimer = 0f;
                    break;
            }
        }
        
        /// <summary>
        /// 타일 발동 - 플레이어가 밟을 때 호출
        /// </summary>
        public virtual void Activate()
        {
            if (_currentState == TileState.Ready)
            {
                SetState(TileState.Activated);
                OnActivated?.Invoke(this);
            }
        }
        
        /// <summary>
        /// 타일 상태 변경 처리
        /// </summary>
        protected void SetState(TileState newState)
        {
            _currentState = newState;
            _stateTimer = 0f;
            
            if (newState == TileState.Ready)
            {
                OnReady?.Invoke(this);
            }
        }
        
        /// <summary>
        /// 현재 타일 상태 반환
        /// </summary>
        public TileState GetState()
        {
            return _currentState;
        }
        
        /// <summary>
        /// 현재 상태의 진행도(0-1) 반환
        /// </summary>
        public float GetStateProgress()
        {
            switch (_currentState)
            {
                case TileState.Charging:
                    return _stateTimer / _chargeTime;
                default:
                    return 0f;
            }
        }
    }   
}