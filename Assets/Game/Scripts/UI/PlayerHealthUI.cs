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
                _playerHealth.OnHealthChanged += UpdateHealthUI;
                _healthSlider.maxValue = _playerHealth.MaxHealth;
                _healthSlider.value = _playerHealth.CurrentHealth;
            }
        }
    
        /// <summary>
        /// 체력 UI 업데이트
        /// </summary>
        private void UpdateHealthUI(int health)
        {
            _healthSlider.value = health;
        }
    }
}