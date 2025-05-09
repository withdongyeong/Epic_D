using Game.Scripts.Characters.Enemies;
using Game.Scripts.Characters.Player;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game.Scripts.UI
{
    /// <summary>
    /// 게임 UI 관리 클래스
    /// </summary>
    public class GameUIManager : MonoBehaviour
    {
        [Header("UI 패널")]
        public GameObject mainMenuPanel;
        public GameObject hudPanel;
        public GameObject victoryPanel;
        public GameObject defeatPanel;

        [Header("버튼")]
        public Button startButton;
        public Button restartButton;
        public Button restartFromDefeatButton;
        public Button restartFromVictoryButton;

        [Header("적 체력")]
        public Slider enemyHealthSlider;
        public TextMeshProUGUI enemyHealthText;

        private BaseEnemy _enemy;
        private PlayerHealth _playerHealth;
        private Core.GameStateManager _gameStateManager;

        /// <summary>
        /// 초기화 및 이벤트 연결
        /// </summary>
        private void Start()
        {
            _gameStateManager = Core.GameStateManager.Instance;
            _gameStateManager.OnGameStateChanged += HandleGameStateChanged;

            // 버튼 이벤트 연결
            if (startButton != null)
                startButton.onClick.AddListener(OnStartButtonClicked);
            
            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartButtonClicked);
            
            if (restartFromDefeatButton != null)
                restartFromDefeatButton.onClick.AddListener(OnRestartButtonClicked);
            
            if (restartFromVictoryButton != null)
                restartFromVictoryButton.onClick.AddListener(OnRestartButtonClicked);

            // 플레이어 사망 이벤트 연결
            _playerHealth = FindAnyObjectByType<PlayerHealth>();
            if (_playerHealth != null)
            {
                _playerHealth.OnPlayerDeath += OnPlayerDeath;
            }

            // 적 체력 업데이트 준비
            UpdateEnemyReference();
            
            // 초기 UI 상태 설정
            HandleGameStateChanged(_gameStateManager.CurrentState);
        }

        /// <summary>
        /// 게임 상태 변경에 따른 UI 업데이트
        /// </summary>
        private void HandleGameStateChanged(Core.GameStateManager.GameState newState)
        {
            // 모든 패널 비활성화
            mainMenuPanel.SetActive(false);
            hudPanel.SetActive(false);
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);

            // 새 상태에 맞는 패널 활성화
            switch (newState)
            {
                case Core.GameStateManager.GameState.MainMenu:
                    mainMenuPanel.SetActive(true);
                    break;
                case Core.GameStateManager.GameState.Playing:
                    hudPanel.SetActive(true);
                    UpdateEnemyReference();
                    break;
                case Core.GameStateManager.GameState.Victory:
                    victoryPanel.SetActive(true);
                    break;
                case Core.GameStateManager.GameState.Defeat:
                    defeatPanel.SetActive(true);
                    break;
            }
        }

        /// <summary>
        /// 적 참조 업데이트 및 이벤트 연결
        /// </summary>
        private void UpdateEnemyReference()
        {
            _enemy = FindAnyObjectByType<BaseEnemy>();
            
            if (_enemy != null && enemyHealthSlider != null)
            {
                enemyHealthSlider.maxValue = _enemy.Health;
                UpdateEnemyHealthUI();
                
                // Health 속성 변경 감지를 위한 주기적 업데이트
                InvokeRepeating("UpdateEnemyHealthUI", 0.0f, 0.1f);
            }
        }

        /// <summary>
        /// 적 체력 UI 업데이트
        /// </summary>
        private void UpdateEnemyHealthUI()
        {
            if (_enemy == null || _enemy.IsDead)
            {
                CancelInvoke("UpdateEnemyHealthUI");
                
                if (_enemy != null && _enemy.IsDead)
                {
                    _gameStateManager.WinGame();
                }
                return;
            }

            enemyHealthSlider.value = _enemy.Health;
            if (enemyHealthText != null)
            {
                enemyHealthText.text = $"{_enemy.Health} / {enemyHealthSlider.maxValue}";
            }
        }

        #region 버튼 이벤트 핸들러

        /// <summary>
        /// 시작 버튼 클릭 핸들러
        /// </summary>
        private void OnStartButtonClicked()
        {
            _gameStateManager.StartGame();
        }

        /// <summary>
        /// 재시작 버튼 클릭 핸들러
        /// </summary>
        private void OnRestartButtonClicked()
        {
            _gameStateManager.RestartGame();
            RestartGameplay();
        }

        #endregion

        /// <summary>
        /// 플레이어 사망 처리
        /// </summary>
        private void OnPlayerDeath()
        {
            _gameStateManager.LoseGame();
        }

        /// <summary>
        /// 게임플레이 재시작 로직
        /// </summary>
        private void RestartGameplay()
        {
            // 여기에 게임 요소 재설정 로직 추가
            if (_playerHealth != null)
            {
                _playerHealth.Heal(_playerHealth.MaxHealth); // 체력 회복
            }

            // 적 재생성 또는 체력 회복
            UpdateEnemyReference();
            if (_enemy != null)
            {
                _enemy.Health = 100; // 체력 리셋
            }
        }
    }
}