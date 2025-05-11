using Game.Scripts.Characters.Player;
using UnityEngine;

namespace Game.Scripts.Tiles
{
    /// <summary>
    /// 회복 타일 클래스 - 플레이어 체력 회복
    /// </summary>
    public class HealTile : BaseTile
    {
        [SerializeField] private int _healAmount = 10;
        private PlayerHealth _playerHealth;
        
        /// <summary>
        /// 회복량 프로퍼티
        /// </summary>
        public int HealAmount { get => _healAmount; set => _healAmount = value; }
        
        private void Start()
        {
            // 플레이어 참조 찾기
            _playerHealth = FindAnyObjectByType<PlayerHealth>();
        }
        
        /// <summary>
        /// 타일 발동 - 플레이어 체력 회복
        /// </summary>
        public override void Activate()
        {
            base.Activate();
            
            if (GetState() == TileState.Activated && _playerHealth != null)
            {
                ApplyHealing();
            }
        }
        
        /// <summary>
        /// 플레이어에게 회복 효과 적용
        /// </summary>
        private void ApplyHealing()
        {
            // PlayerHealth 클래스의 힐링 메서드 호출
            _playerHealth.Heal(_healAmount);
            
            // 회복 시각 효과 표시
            ShowHealEffect();
        }
        
        /// <summary>
        /// 회복 효과 시각화
        /// </summary>
        private void ShowHealEffect()
        {
            Debug.Log($"회복 효과 적용: {_healAmount} HP 회복");
            
            // 회복 파티클 효과나 애니메이션 재생
            // 여기에 파티클 시스템이나 시각 효과 추가
        }
    }
}