using Game.Scripts.Characters.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    /// <summary>
    /// 플레이어 체력 UI 표시
    /// </summary>
    public class PlayerHealthUI : MonoBehaviour
    {
        private Slider _healthSlider;
        private PlayerHealth _playerHealth;
    
        void Start()
        {
            _healthSlider = GetComponent<Slider>();
            _playerHealth = FindAnyObjectByType<PlayerHealth>();
        
            if (_playerHealth != null)
            {
                // 이벤트 연결
                _playerHealth.OnHealthChanged += UpdateHealthUI;
                
                // 슬라이더 초기 설정
                _healthSlider.maxValue = _playerHealth.MaxHealth;
                _healthSlider.value = _playerHealth.CurrentHealth;
                
                Debug.Log($"플레이어 체력 UI 초기화: {_playerHealth.CurrentHealth}/{_playerHealth.MaxHealth}");
            }
            else
            {
                Debug.LogError("PlayerHealth 컴포넌트를 찾을 수 없습니다.");
            }
        }
    
        /// <summary>
        /// 체력 UI 업데이트
        /// </summary>
        private void UpdateHealthUI(int health)
        {
            if (_healthSlider != null)
            {
                _healthSlider.value = health;
                Debug.Log($"체력 UI 업데이트: {health}");
            }
        }
        
        private void OnDestroy()
        {
            if (_playerHealth != null)
            {
                _playerHealth.OnHealthChanged -= UpdateHealthUI;
            }
        }
    }
}