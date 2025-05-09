using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Building
{
    /// <summary>
    /// 빌딩 씬 관리 클래스
    /// </summary>
    public class BuildingManager : MonoBehaviour
    {
        [Header("UI 요소")]
        public Button startGameButton;
        private string _gameplaySceneName = "GameplayScene";

        private void Start()
        {
            Debug.Log("BuildingManager Start 호출");
            InitializeUI();
        }
        
        private void OnEnable()
        {
            Debug.Log("BuildingManager OnEnable 호출");
            InitializeUI();
        }
        
        /// <summary>
        /// UI 초기화 - 버튼 이벤트 연결
        /// </summary>
        private void InitializeUI()
        {
            // 타임스케일 초기화
            Time.timeScale = 1.0f;
            Debug.Log($"빌딩 씬 UI 초기화: 타임스케일 = {Time.timeScale}");
            
            if (startGameButton != null)
            {
                // 기존 리스너 제거 후 새로 할당
                startGameButton.onClick.RemoveAllListeners();
                startGameButton.onClick.AddListener(OnStartGameClicked);
                Debug.Log("시작 버튼 이벤트 연결 완료");
            }
            else
            {
                Debug.LogError("시작 버튼이 할당되지 않았습니다.");
            }
        }

        /// <summary>
        /// 게임 시작 버튼 클릭 핸들러
        /// </summary>
        private void OnStartGameClicked()
        {
            Debug.Log("게임 시작 버튼 클릭: 씬 전환 시도");
            
            // 직접 타임스케일 설정
            Time.timeScale = 1.0f;
            
            // 직접 씬 전환
            SceneManager.LoadScene(_gameplaySceneName);
        }
    }
}