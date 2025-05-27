namespace Game3.Scripts.Core
{
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Game3.Scripts.Core
{
    public class UIManager : MonoBehaviour
    {
        /// <summary>
        /// 클리어 UI 패널
        /// </summary>
        [SerializeField] private GameObject clearPanel;
        
        /// <summary>
        /// 게임오버 UI 패널
        /// </summary>
        [SerializeField] private GameObject gameOverPanel;
        
        /// <summary>
        /// 점수 텍스트
        /// </summary>
        [SerializeField] private TextMeshProUGUI scoreText;
        
        /// <summary>
        /// 클리어 시 점수 텍스트
        /// </summary>
        [SerializeField] private TextMeshProUGUI clearScoreText;
        
        /// <summary>
        /// 게임오버 시 점수 텍스트
        /// </summary>
        [SerializeField] private TextMeshProUGUI gameOverScoreText;
        
        /// <summary>
        /// 재시작 버튼 (클리어 패널)
        /// </summary>
        [SerializeField] private Button clearRestartButton;
        
        /// <summary>
        /// 재시작 버튼 (게임오버 패널)
        /// </summary>
        [SerializeField] private Button gameOverRestartButton;
        
        /// <summary>
        /// 플레이어 체력 슬라이더
        /// </summary>
        [SerializeField] private Slider healthSlider;
        
        /// <summary>
        /// 화살 파워 게이지 슬라이더
        /// </summary>
        [SerializeField] private Slider powerSlider;
        
        /// <summary>
        /// 파워 게이지 배경 이미지 (색상 변경용)
        /// </summary>
        [SerializeField] private Image powerFillImage;
        
        /// <summary>
        /// 파워 게이지 부족 색상
        /// </summary>
        [SerializeField] private Color insufficientPowerColor = Color.red;
        
        /// <summary>
        /// 파워 게이지 충분 색상
        /// </summary>
        [SerializeField] private Color sufficientPowerColor = Color.green;
        
        /// <summary>
        /// 최소 파워 임계값 참조
        /// </summary>
        private float minPowerThreshold = 0.5f;
        
        /// <summary>
        /// 게임 매니저 참조
        /// </summary>
        private GameManager gameManager;
        
        /// <summary>
        /// 클리어 UI 패널 프로퍼티
        /// </summary>
        public GameObject ClearPanel { get => clearPanel; set => clearPanel = value; }
        
        /// <summary>
        /// 게임오버 UI 패널 프로퍼티
        /// </summary>
        public GameObject GameOverPanel { get => gameOverPanel; set => gameOverPanel = value; }

        private void Start()
        {
            // 게임 매니저 참조 가져오기
            gameManager = GameManager.Instance;
            
            // 패널 숨기기
            if (clearPanel != null)
            {
                clearPanel.SetActive(false);
            }
            
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(false);
            }
            
            // 버튼 리스너 설정
            if (clearRestartButton != null)
            {
                clearRestartButton.onClick.AddListener(OnRestartButtonClicked);
            }
            
            if (gameOverRestartButton != null)
            {
                gameOverRestartButton.onClick.AddListener(OnRestartButtonClicked);
            }
            
            // 플레이어 체력 슬라이더 초기화
            InitializeHealthSlider();
            
            // 파워 게이지 슬라이더 초기화
            InitializePowerSlider();
        }

        /// <summary>
        /// 점수 텍스트 업데이트
        /// </summary>
        /// <param name="score">표시할 점수</param>
        public void UpdateScoreText(int score)
        {
            if (scoreText != null)
            {
                scoreText.text = "점수: " + score.ToString();
            }
        }

        /// <summary>
        /// 플레이어 체력 슬라이더 초기화
        /// </summary>
        private void InitializeHealthSlider()
        {
            if (healthSlider != null)
            {
                var player = FindObjectOfType<Character.Player.PlayerController>();
                if (player != null)
                {
                    healthSlider.maxValue = player.MaxHealth;
                    healthSlider.value = player.CurrentHealth;
                    Debug.Log($"체력 슬라이더 초기화: 최대 체력 = {player.MaxHealth}, 현재 체력 = {player.CurrentHealth}");
                }
                else
                {
                    Debug.LogWarning("플레이어를 찾을 수 없어 체력 슬라이더를 초기화할 수 없습니다.");
                }
            }
            else
            {
                Debug.LogWarning("체력 슬라이더가 할당되지 않았습니다.");
            }
        }
        
        /// <summary>
        /// 파워 게이지 슬라이더 초기화
        /// </summary>
        private void InitializePowerSlider()
        {
            if (powerSlider != null)
            {
                powerSlider.minValue = 0f;
                powerSlider.maxValue = 1f;
                powerSlider.value = 0f;
                
                // 플레이어에서 최소 파워 임계값 가져오기
                var player = FindObjectOfType<Character.Player.PlayerController>();
                if (player != null)
                {
                    minPowerThreshold = player.MinPowerThreshold;
                }
                
                // 파워 게이지 색상 초기화
                UpdatePowerSliderColor(0f);
            }
            else
            {
                Debug.LogWarning("파워 게이지 슬라이더가 할당되지 않았습니다.");
            }
        }

        /// <summary>
        /// 플레이어 체력 슬라이더 업데이트
        /// </summary>
        /// <param name="currentHealth">현재 체력</param>
        public void UpdateHealthSlider(int currentHealth)
        {
            if (healthSlider != null)
            {
                healthSlider.value = currentHealth;
                Debug.Log($"체력 슬라이더 업데이트: 현재 체력 = {currentHealth}");
            }
            else
            {
                Debug.LogWarning("체력 슬라이더가 할당되지 않아 업데이트할 수 없습니다.");
            }
        }
        
        /// <summary>
        /// 파워 게이지 슬라이더 업데이트
        /// </summary>
        /// <param name="power">현재 파워 (0-1)</param>
        public void UpdatePowerSlider(float power)
        {
            if (powerSlider != null)
            {
                powerSlider.value = power;
                UpdatePowerSliderColor(power);
            }
        }
        
        /// <summary>
        /// 파워 게이지 색상 업데이트
        /// </summary>
        /// <param name="power">현재 파워 (0-1)</param>
        private void UpdatePowerSliderColor(float power)
        {
            if (powerFillImage != null)
            {
                // 충분한 파워인지 확인
                if (power >= minPowerThreshold)
                {
                    powerFillImage.color = sufficientPowerColor;
                }
                else
                {
                    powerFillImage.color = insufficientPowerColor;
                }
            }
        }

        /// <summary>
        /// 클리어 UI 표시
        /// </summary>
        public void ShowClearUI()
        {
            if (clearPanel != null)
            {
                clearPanel.SetActive(true);
            }
            
            if (clearScoreText != null && gameManager != null)
            {
                clearScoreText.text = "최종 점수: " + gameManager.CurrentScore.ToString();
            }
        }

        /// <summary>
        /// 게임오버 UI 표시
        /// </summary>
        public void ShowGameOverUI()
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
            }
            
            if (gameOverScoreText != null && gameManager != null)
            {
                gameOverScoreText.text = "최종 점수: " + gameManager.CurrentScore.ToString();
            }
        }

        /// <summary>
        /// 재시작 버튼 클릭 이벤트 처리
        /// </summary>
        private void OnRestartButtonClicked()
        {
            if (gameManager != null)
            {
                gameManager.RestartGame();
            }
        }
    }
}
}