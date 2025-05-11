using Game.Scripts.Characters.Player;
using UnityEngine;
using System.Collections;

namespace Game.Scripts.Tiles
{
    /// <summary>
    /// 방어 타일 클래스 - 플레이어에게 일시적인 무적 효과 제공
    /// </summary>
    public class DefenseTile : BaseTile
    {
        private float _invincibilityDuration = 3f;
        private GameObject _shieldEffect;
        private PlayerHealth _playerHealth;
        
        /// <summary>
        /// 무적 지속 시간 프로퍼티
        /// </summary>
        public float InvincibilityDuration { get => _invincibilityDuration; set => _invincibilityDuration = value; }
        
        [SerializeField] private GameObject _shieldEffectPrefab;
        
        private void Start()
        {
            // 플레이어 참조 찾기
            _playerHealth = FindAnyObjectByType<PlayerHealth>();
        }
        
        /// <summary>
        /// 타일 발동 - 플레이어에게 무적 효과 부여
        /// </summary>
        public override void Activate()
        {
            base.Activate();
            
            if (GetState() == TileState.Activated && _playerHealth != null)
            {
                ApplyInvincibility();
            }
        }
        
        /// <summary>
        /// 플레이어에게 일시적인 무적 효과 제공
        /// </summary>
        private void ApplyInvincibility()
        {
            // 플레이어 무적 상태 설정
            _playerHealth.SetInvincible(true);
            
            // 방어막 시각 효과 표시
            ShowShieldEffect();
            
            // 무적 해제 타이머 시작
            StartCoroutine(RemoveInvincibilityAfterDuration());
        }
        
        /// <summary>
        /// 방어막 효과 시각화
        /// </summary>
        private void ShowShieldEffect()
        {
            if (_shieldEffectPrefab != null && _playerHealth != null)
            {
                // 플레이어에게 방어막 효과 생성
                _shieldEffect = Instantiate(_shieldEffectPrefab, _playerHealth.transform);
                _shieldEffect.transform.localPosition = Vector3.zero;
            }
            else
            {
                Debug.Log("방어막 효과 적용 (시각 효과 없음)");
            }
        }
        
        /// <summary>
        /// 지정 시간 후 무적 효과 제거
        /// </summary>
        private IEnumerator RemoveInvincibilityAfterDuration()
        {
            yield return new WaitForSeconds(_invincibilityDuration);
            
            // 무적 해제
            if (_playerHealth != null)
            {
                _playerHealth.SetInvincible(false);
            }
            
            // 방어막 효과 제거
            if (_shieldEffect != null)
            {
                Destroy(_shieldEffect);
                _shieldEffect = null;
            }
            
            Debug.Log("무적 효과 종료");
        }
        
        private void OnDestroy()
        {
            // 남아있는 효과 정리
            if (_shieldEffect != null)
            {
                Destroy(_shieldEffect);
            }
        }
    }
}