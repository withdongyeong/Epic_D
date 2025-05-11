using Game.Scripts.Core;
using Game.Scripts.Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Scripts.UI
{
    /// <summary>
    /// 게임 UI 관리 클래스
    /// </summary>
    public class GameUIManager : MonoBehaviour
    {
        [Header("결과 패널")]
        public GameObject victoryPanel;    // 승리 시 표시되는 패널
        public GameObject defeatPanel;     // 패배 시 표시되는 패널

        [Header("버튼")]
        public Button victoryReturnButton; // 승리 패널의 돌아가기 버튼
        public Button defeatReturnButton;  // 패배 패널의 돌아가기 버튼
        public Button retryButton;         // 재시도 버튼
        
        private Core.GameStateManager _gameStateManager;

        [Header("전환 씬 이름")] private string _buildingSceneName = "BuildingScene";

        /// <summary>
        /// 초기화 및 이벤트 연결
        /// </summary>
        private void Start()
        {
            // 패널 초기 상태 설정
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
            
            // 게임 상태 매니저 참조
            _gameStateManager = Core.GameStateManager.Instance;
            _gameStateManager.OnGameStateChanged += HandleGameStateChanged;
            
            // 버튼 이벤트 연결
            if (victoryReturnButton != null)
                victoryReturnButton.onClick.AddListener(ReturnToBuilding);
                
            if (defeatReturnButton != null)
                defeatReturnButton.onClick.AddListener(ReturnToBuilding);
                
            if (retryButton != null)
                retryButton.onClick.AddListener(RetryGame);
        }

        /// <summary>
        /// 게임 상태 변경 처리
        /// </summary>
        private void HandleGameStateChanged(Core.GameStateManager.GameState newState)
        {
            switch (newState)
            {
                case Core.GameStateManager.GameState.Victory:
                    ShowVictoryPanel();
                    break;
                case Core.GameStateManager.GameState.Defeat:
                    ShowDefeatPanel();
                    break;
                case Core.GameStateManager.GameState.Playing:
                    HideResultPanels();
                    break;
            }
        }

        /// <summary>
        /// 승리 패널 표시
        /// </summary>
        private void ShowVictoryPanel()
        {
            victoryPanel.SetActive(true);
            defeatPanel.SetActive(false);
        }

        /// <summary>
        /// 패배 패널 표시
        /// </summary>
        private void ShowDefeatPanel()
        {
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(true);
        }

        /// <summary>
        /// 결과 패널 숨기기
        /// </summary>
        private void HideResultPanels()
        {
            victoryPanel.SetActive(false);
            defeatPanel.SetActive(false);
        }

        /// <summary>
        /// 빌딩 씬으로 돌아가기
        /// </summary>
        private void ReturnToBuilding()
        {
            // TODO 임시. 시도 횟수 증가
            if (ShopManager.Instance != null)
            {
                ShopManager.Instance.IncreaseTryCount();
            }
            TimeScaleManager.Instance.ResetTimeScale();
            SceneManager.LoadScene(_buildingSceneName);
        }

        /// <summary>
        /// 게임 재시도
        /// </summary>
        private void RetryGame()
        {
            HideResultPanels();
            _gameStateManager.RestartGame();
        }

        /// <summary>
        /// 이벤트 연결 해제
        /// </summary>
        private void OnDestroy()
        {
            if (_gameStateManager != null)
                _gameStateManager.OnGameStateChanged -= HandleGameStateChanged;
        }
    }
}