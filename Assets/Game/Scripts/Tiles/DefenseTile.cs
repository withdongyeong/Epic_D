using Game.Scripts.Characters.Player;
using UnityEngine;

namespace Game.Scripts.Tiles
{
    public class DefenseTile : BaseTile
    {
        [SerializeField] private float _invincibilityDuration = 0.5f;
        [SerializeField] private GameObject _shieldEffectPrefab;
        
        private PlayerHealth _playerHealth;
        private GameObject _activeShieldEffect;
        
        public float InvincibilityDuration { get => _invincibilityDuration; set => _invincibilityDuration = value; }
        
        private void Start()
        {
            _playerHealth = FindAnyObjectByType<PlayerHealth>();
        }
        
        /// <summary>
        /// 타일 발동 - 플레이어 무적 효과 부여
        /// </summary>
        public override void Activate()
        {
            base.Activate();
            if (GetState() == TileState.Activated && _playerHealth != null)
            {
                GrantInvincibility();
            }
        }
        
        /// <summary>
        /// 플레이어에게 무적 상태 부여
        /// </summary>
        private void GrantInvincibility()
        {
            _playerHealth.SetInvincible(true, _invincibilityDuration);
            
            // 무적 이펙트 생성
            CreateShieldEffect();
            
            // 일정 시간 후 이펙트 제거
            Invoke(nameof(RemoveShieldEffect), _invincibilityDuration);
        }
        
        /// <summary>
        /// 방어 실드 이펙트 생성
        /// </summary>
        private void CreateShieldEffect()
        {
            if (_shieldEffectPrefab != null && _playerHealth != null)
            {
                // 기존 이펙트가 있다면 제거
                if (_activeShieldEffect != null)
                {
                    Destroy(_activeShieldEffect);
                }
                
                // 플레이어 위치에 실드 이펙트 생성
                _activeShieldEffect = Instantiate(
                    _shieldEffectPrefab, 
                    _playerHealth.transform.position, 
                    Quaternion.identity, 
                    _playerHealth.transform
                );
            }
        }
        
        /// <summary>
        /// 방어 실드 이펙트 제거
        /// </summary>
        private void RemoveShieldEffect()
        {
            if (_activeShieldEffect != null)
            {
                Destroy(_activeShieldEffect);
                _activeShieldEffect = null;
            }
        }
        
        private void OnDestroy()
        {
            // 실행 중인 Invoke 취소
            CancelInvoke(nameof(RemoveShieldEffect));
            
            // 이펙트 제거
            if (_activeShieldEffect != null)
            {
                Destroy(_activeShieldEffect);
            }
        }
    }
}