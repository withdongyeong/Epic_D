using System.Collections;
using UnityEngine;
using Game.Scripts.Characters.Player;
using Game.Scripts.Core;
using Game.Scripts.Projectiles;

namespace Game.Scripts.Characters.Enemies
{
    /// <summary>
    /// 고정 위치 적 캐릭터 구현
    /// </summary>
    public class FixedEnemy : BaseEnemy
    {
        [Header("프리팹")]
        public GameObject projectilePrefab;
        public GameObject warningTilePrefab;
        public GameObject explosionEffectPrefab;
        public GameObject meteorPrefab;
        
        private GridSystem _gridSystem;
        private PlayerController _player;
        private PlayerHealth _playerHealth;
        private float _patternCooldown = 0.5f;
        
        private EnemyAttackManager _attackManager;
        private EnemyPatternManager _patternManager;
        
        protected override void Awake()
        {
            base.Awake();
            _attackManager = gameObject.AddComponent<EnemyAttackManager>();
            _patternManager = gameObject.AddComponent<EnemyPatternManager>();
        }
        
        void Start()
        {
            InitializeComponents();
            StartCoroutine(AttackRoutine());
        }
        
        /// <summary>
        /// 컴포넌트 초기화
        /// </summary>
        private void InitializeComponents()
        {
            _gridSystem = FindAnyObjectByType<GridSystem>();
            _player = FindAnyObjectByType<PlayerController>();
            _playerHealth = _player.GetComponent<PlayerHealth>();
            
            // 공격 매니저 초기화
            _attackManager.Initialize(
                _gridSystem,
                _player,
                _playerHealth,
                warningTilePrefab,
                explosionEffectPrefab,
                projectilePrefab,
                meteorPrefab,
                transform
            );
            
            // 패턴 매니저 초기화
            _patternManager.Initialize(_attackManager);
        }
        
        /// <summary>
        /// 공격 패턴 실행 루틴
        /// </summary>
        private IEnumerator AttackRoutine()
        {
            yield return new WaitForSeconds(1f);
        
            while (!IsDead)
            {
                _patternManager.ExecuteRandomPattern();
                yield return new WaitForSeconds(_patternCooldown);
            }
        }
        
        /// <summary>
        /// 메테오 공격 트리거 조건 - 체력이 50% 이하이고 플레이어가 공격을 받았을 때
        /// </summary>
        public override bool ShouldTriggerMeteorAttack()
        {
            // 체력이 50% 이하일 때 메테오 공격 확률 증가
            return CurrentHealth <= maxHealth * 0.5f && Random.value < 0.7f;
        }
    }
}