using Game.Scripts.Characters.Player;
using UnityEngine;

namespace Game.Scripts.Tiles
{
    public class HealTile : BaseTile
    {
        [SerializeField] private int _healAmount = 25;
        [SerializeField] private GameObject _healEffectPrefab;
        
        private PlayerHealth _playerHealth;
        
        public int HealAmount { get => _healAmount; set => _healAmount = value; }
        
        private void Start()
        {
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
                HealPlayer();
            }
        }
        
        /// <summary>
        /// 플레이어 체력 회복 처리
        /// </summary>
        private void HealPlayer()
        {
            // 플레이어 체력 회복
            _playerHealth.Heal(_healAmount);
            
            // 회복 이펙트 생성
            CreateHealEffect();
        }
        
        /// <summary>
        /// 회복 이펙트 생성
        /// </summary>
        private void CreateHealEffect()
        {
            if (_healEffectPrefab != null && _playerHealth != null)
            {
                // 플레이어 위치에 회복 이펙트 생성
                GameObject effectObj = Instantiate(
                    _healEffectPrefab,
                    _playerHealth.transform.position,
                    Quaternion.identity
                );
                
                // 일정 시간 후 이펙트 제거
                Destroy(effectObj, 0.1f);
            }
        }
    }
}