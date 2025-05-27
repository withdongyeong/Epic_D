using Game4.Scripts.Character.Player;

namespace Game4.Scripts.Core
{
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    /// <summary>
    /// 시작 패널
    /// </summary>
    public GameObject startPanel;
    
    /// <summary>
    /// 게임 UI 패널
    /// </summary>
    public GameObject gamePanel;
    
    /// <summary>
    /// 게임 오버 패널
    /// </summary>
    public GameObject gameOverPanel;
    
    /// <summary>
    /// 게임 클리어 패널
    /// </summary>
    public GameObject gameClearPanel;
    
    /// <summary>
    /// 시작하기 버튼
    /// </summary>
    public Button startButton;
    
    /// <summary>
    /// 게임 오버 다시하기 버튼
    /// </summary>
    public Button gameOverRestartButton;
    
    /// <summary>
    /// 게임 클리어 다시하기 버튼
    /// </summary>
    public Button gameClearRestartButton;
    
    /// <summary>
    /// 남은 몬스터 수 텍스트
    /// </summary>
    public TextMeshProUGUI bossCountdownText;
    
    /// <summary>
    /// 플레이어 체력 텍스트
    /// </summary>
    public TextMeshProUGUI playerHealthText;
    
    /// <summary>
    /// 스킬 쿨타임 슬라이더
    /// </summary>
    public Slider skillCooldownSlider;
    
    /// <summary>
    /// 스킬 쿨타임 텍스트
    /// </summary>
    public TextMeshProUGUI skillCooldownText;
    
    /// <summary>
    /// 게임 매니저 참조
    /// </summary>
    private GameManager gameManager;
    
    /// <summary>
    /// 스킬 매니저 참조
    /// </summary>
    private SkillManager skillManager;

    // Properties
    /// <summary>
    /// 시작 패널 프로퍼티
    /// </summary>
    public GameObject StartPanel { get => startPanel; set => startPanel = value; }
    
    /// <summary>
    /// 게임 UI 패널 프로퍼티
    /// </summary>
    public GameObject GamePanel { get => gamePanel; set => gamePanel = value; }
    
    /// <summary>
    /// 게임 오버 패널 프로퍼티
    /// </summary>
    public GameObject GameOverPanel { get => gameOverPanel; set => gameOverPanel = value; }
    
    /// <summary>
    /// 게임 클리어 패널 프로퍼티
    /// </summary>
    public GameObject GameClearPanel { get => gameClearPanel; set => gameClearPanel = value; }
    
    /// <summary>
    /// 시작하기 버튼 프로퍼티
    /// </summary>
    public Button StartButton { get => startButton; set => startButton = value; }
    
    /// <summary>
    /// 게임 오버 다시하기 버튼 프로퍼티
    /// </summary>
    public Button GameOverRestartButton { get => gameOverRestartButton; set => gameOverRestartButton = value; }
    
    /// <summary>
    /// 게임 클리어 다시하기 버튼 프로퍼티
    /// </summary>
    public Button GameClearRestartButton { get => gameClearRestartButton; set => gameClearRestartButton = value; }
    
    /// <summary>
    /// 남은 몬스터 수 텍스트 프로퍼티
    /// </summary>
    public TextMeshProUGUI BossCountdownText { get => bossCountdownText; set => bossCountdownText = value; }
    
    /// <summary>
    /// 플레이어 체력 텍스트 프로퍼티
    /// </summary>
    public TextMeshProUGUI PlayerHealthText { get => playerHealthText; set => playerHealthText = value; }
    
    /// <summary>
    /// 스킬 쿨타임 슬라이더 프로퍼티
    /// </summary>
    public Slider SkillCooldownSlider { get => skillCooldownSlider; set => skillCooldownSlider = value; }
    
    /// <summary>
    /// 스킬 쿨타임 텍스트 프로퍼티
    /// </summary>
    public TextMeshProUGUI SkillCooldownText { get => skillCooldownText; set => skillCooldownText = value; }

    /// <summary>
    /// 칼날폭풍 스킬 쿨타임 슬라이더
    /// </summary>
    public Slider bladeStormCooldownSlider;

    /// <summary>
    /// 칼날폭풍 스킬 쿨타임 텍스트
    /// </summary>
    public TextMeshProUGUI bladeStormCooldownText;

    /// <summary>
    /// 칼날폭풍 스킬 쿨타임 슬라이더 프로퍼티
    /// </summary>
    public Slider BladeStormCooldownSlider { get => bladeStormCooldownSlider; set => bladeStormCooldownSlider = value; }

    /// <summary>
    /// 칼날폭풍 스킬 쿨타임 텍스트 프로퍼티
    /// </summary>
    public TextMeshProUGUI BladeStormCooldownText { get => bladeStormCooldownText; set => bladeStormCooldownText = value; }
    
    /// <summary>
    /// 초기화
    /// </summary>
    private void Start()
    {
        InitializeUI();
        SetupEventListeners();
    }

    /// <summary>
    /// UI 초기화
    /// </summary>
    private void InitializeUI()
    {
        gameManager = GameManager.Instance;
        skillManager = FindObjectOfType<SkillManager>();
        
        if (gameManager != null)
        {
            // 게임 매니저 이벤트 구독
            gameManager.OnGameStateChanged += HandleGameStateChanged;
            gameManager.OnMonsterCountChanged += UpdateMonsterCount;
            gameManager.OnBossSpawned += UpdateBossStatus;
        }
        
        if (skillManager != null)
        {
            // 스킬 매니저 이벤트 구독
            skillManager.OnCooldownUpdate += UpdateSkillCooldown;
            skillManager.OnBladeStormCooldownUpdate += UpdateBladeStormCooldown;
        }

        // 초기 UI 상태 설정
        ShowStartPanel();
        InitializeSkillUI();
        
        // 초기 UI 상태 설정
        ShowStartPanel();
        InitializeSkillUI();
        
        Debug.Log("UIManager initialized");
    }
    
    /// <summary>
    /// 스킬 UI 초기화
    /// </summary>
    private void InitializeSkillUI()
    {
        if (skillCooldownSlider != null)
        {
            skillCooldownSlider.minValue = 0f;
            skillCooldownSlider.maxValue = 1f;
            skillCooldownSlider.value = 1f; // 시작 시 스킬 사용 가능
        }
    
        if (bladeStormCooldownSlider != null)
        {
            bladeStormCooldownSlider.minValue = 0f;
            bladeStormCooldownSlider.maxValue = 1f;
            bladeStormCooldownSlider.value = 1f; // 시작 시 스킬 사용 가능
        }
    
        UpdateSkillCooldown(0f, 5f); // 초기 표시
        UpdateBladeStormCooldown(0f, 8f); // 초기 표시
    }
    
    /// <summary>
    /// 칼날폭풍 스킬 쿨타임 업데이트
    /// </summary>
    /// <param name="remainingTime">남은 쿨타임</param>
    /// <param name="totalTime">전체 쿨타임</param>
    private void UpdateBladeStormCooldown(float remainingTime, float totalTime)
    {
        if (bladeStormCooldownSlider != null)
        {
            float progress = remainingTime <= 0f ? 1f : (totalTime - remainingTime) / totalTime;
            bladeStormCooldownSlider.value = progress;
        }
    
        if (bladeStormCooldownText != null)
        {
            if (remainingTime <= 0f)
            {
                bladeStormCooldownText.text = "READY";
                bladeStormCooldownText.color = Color.green;
            }
            else
            {
                bladeStormCooldownText.text = $"{remainingTime:F1}s";
                bladeStormCooldownText.color = Color.red;
            }
        }
    }

    /// <summary>
    /// 버튼 이벤트 리스너 설정
    /// </summary>
    private void SetupEventListeners()
    {
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }
        
        if (gameOverRestartButton != null)
        {
            gameOverRestartButton.onClick.AddListener(OnRestartButtonClicked);
        }
        
        if (gameClearRestartButton != null)
        {
            gameClearRestartButton.onClick.AddListener(OnRestartButtonClicked);
        }
    }

    /// <summary>
    /// 게임 상태 변경 처리
    /// </summary>
    /// <param name="newState">새로운 게임 상태</param>
    private void HandleGameStateChanged(GameManager.GameState newState)
    {
        Debug.Log($"UI State changed to: {newState}");
        
        switch (newState)
        {
            case GameManager.GameState.Start:
                ShowStartPanel();
                break;
            case GameManager.GameState.Playing:
                ShowGamePanel();
                break;
            case GameManager.GameState.GameOver:
                ShowGameOverPanel();
                break;
            case GameManager.GameState.GameClear:
                ShowGameClearPanel();
                break;
        }
    }

    /// <summary>
    /// 시작 패널 표시
    /// </summary>
    private void ShowStartPanel()
    {
        SetPanelActive(startPanel, true);
        SetPanelActive(gamePanel, false);
        SetPanelActive(gameOverPanel, false);
        SetPanelActive(gameClearPanel, false);
    }

    /// <summary>
    /// 게임 패널 표시
    /// </summary>
    private void ShowGamePanel()
    {
        SetPanelActive(startPanel, false);
        SetPanelActive(gamePanel, true);
        SetPanelActive(gameOverPanel, false);
        SetPanelActive(gameClearPanel, false);
    }

    /// <summary>
    /// 게임 오버 패널 표시
    /// </summary>
    private void ShowGameOverPanel()
    {
        SetPanelActive(startPanel, false);
        SetPanelActive(gamePanel, true); // 게임 UI도 함께 표시
        SetPanelActive(gameOverPanel, true);
        SetPanelActive(gameClearPanel, false);
    }

    /// <summary>
    /// 게임 클리어 패널 표시
    /// </summary>
    private void ShowGameClearPanel()
    {
        SetPanelActive(startPanel, false);
        SetPanelActive(gamePanel, true); // 게임 UI도 함께 표시
        SetPanelActive(gameOverPanel, false);
        SetPanelActive(gameClearPanel, true);
    }

    /// <summary>
    /// 패널 활성화/비활성화
    /// </summary>
    /// <param name="panel">대상 패널</param>
    /// <param name="active">활성화 여부</param>
    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }

    /// <summary>
    /// 시작 버튼 클릭 처리
    /// </summary>
    private void OnStartButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.StartGame();
        }
        
        Debug.Log("Start button clicked");
    }

    /// <summary>
    /// 다시하기 버튼 클릭 처리
    /// </summary>
    private void OnRestartButtonClicked()
    {
        if (gameManager != null)
        {
            gameManager.RestartGame();
        }
        
        Debug.Log("Restart button clicked");
    }

    /// <summary>
    /// 몬스터 수 업데이트 (보스 상태에 따라 다른 표시)
    /// </summary>
    /// <param name="remainingCount">남은 몬스터 수</param>
    private void UpdateMonsterCount(int remainingCount)
    {
        if (bossCountdownText != null)
        {
            bossCountdownText.text = $"Monsters: {remainingCount}";
        }
    
        // 보스 카운트다운 업데이트
        UpdateBossCountdown();
    }

    /// <summary>
    /// 보스 카운트다운 업데이트
    /// </summary>
    private void UpdateBossCountdown()
    {
        if (bossCountdownText == null) return;
    
        SpawnManager spawnManager = FindObjectOfType<SpawnManager>();
        if (spawnManager == null) return;
    
        if (spawnManager.BossSpawned)
        {
            // 보스가 이미 소환되었으면
            bossCountdownText.text = "DEFEAT THE BOSS!";
            bossCountdownText.color = Color.red;
        }
        else
        {
            // 보스 등장까지 남은 킬 수 표시
            int killsNeeded = spawnManager.BossSpawnThreshold - spawnManager.TotalKilledMonsters;
            killsNeeded = Mathf.Max(0, killsNeeded);
        
            if (killsNeeded > 0)
            {
                bossCountdownText.text = $"Boss in {killsNeeded} kills";
                bossCountdownText.color = Color.yellow;
            }
            else
            {
                bossCountdownText.text = "Boss incoming...";
                bossCountdownText.color = Color.magenta;
            }
        }
    }

    /// <summary>
    /// 보스 상태 업데이트 (GameManager에서 호출)
    /// </summary>
    private void UpdateBossStatus()
    {
        UpdateBossCountdown();
    }

    /// <summary>
    /// 플레이어 체력 업데이트
    /// </summary>
    /// <param name="currentHealth">현재 체력</param>
    /// <param name="maxHealth">최대 체력</param>
    public void UpdatePlayerHealth(int currentHealth, int maxHealth)
    {
        if (playerHealthText != null)
        {
            playerHealthText.text = $"Health: {currentHealth}/{maxHealth}";
        }
    }
    
    /// <summary>
    /// 스킬 쿨타임 업데이트
    /// </summary>
    /// <param name="remainingTime">남은 쿨타임</param>
    /// <param name="totalTime">전체 쿨타임</param>
    private void UpdateSkillCooldown(float remainingTime, float totalTime)
    {
        if (skillCooldownSlider != null)
        {
            float progress = remainingTime <= 0f ? 1f : (totalTime - remainingTime) / totalTime;
            skillCooldownSlider.value = progress;
        }
        
        if (skillCooldownText != null)
        {
            if (remainingTime <= 0f)
            {
                skillCooldownText.text = "READY";
                skillCooldownText.color = Color.green;
            }
            else
            {
                skillCooldownText.text = $"{remainingTime:F1}s";
                skillCooldownText.color = Color.red;
            }
        }
    }

    /// <summary>
    /// 매 프레임 UI 업데이트
    /// </summary>
    private void Update()
    {
        UpdatePlayerHealthDisplay();
    }

    /// <summary>
    /// 플레이어 체력 표시 업데이트
    /// </summary>
    private void UpdatePlayerHealthDisplay()
    {
        if (gameManager != null && gameManager.CurrentState == GameManager.GameState.Playing)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                PlayerController player = playerObj.GetComponent<PlayerController>();
                if (player != null)
                {
                    UpdatePlayerHealth(player.CurrentHealth, player.MaxHealth);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnGameStateChanged -= HandleGameStateChanged;
            gameManager.OnMonsterCountChanged -= UpdateMonsterCount;
            gameManager.OnBossSpawned -= UpdateBossStatus;
        }
    
        if (skillManager != null)
        {
            skillManager.OnCooldownUpdate -= UpdateSkillCooldown;
            skillManager.OnBladeStormCooldownUpdate -= UpdateBladeStormCooldown;
        }
    }
}
}