using Game3.Scripts.Core.Game3.Scripts.Core;

namespace Game3.Scripts.Core
{
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 인스턴스
    /// </summary>
    private static GameManager instance;
    
    /// <summary>
    /// 현재 점수
    /// </summary>
    private int currentScore = 0;
    
    /// <summary>
    /// 게임 UI 매니저 참조
    /// </summary>
    [SerializeField] private UIManager uiManager;
    
    /// <summary>
    /// 클리어 여부
    /// </summary>
    private bool isGameCleared = false;
    
    /// <summary>
    /// 게임오버 여부
    /// </summary>
    private bool isGameOver = false;
    
    /// <summary>
    /// 싱글톤 인스턴스 프로퍼티
    /// </summary>
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
                
                if (instance == null)
                {
                    GameObject gameManagerObject = new GameObject("GameManager");
                    instance = gameManagerObject.AddComponent<GameManager>();
                }
            }
            return instance;
        }
    }
    
    /// <summary>
    /// 현재 점수 프로퍼티
    /// </summary>
    public int CurrentScore { get => currentScore; private set => currentScore = value; }
    
    /// <summary>
    /// 게임 클리어 여부 프로퍼티
    /// </summary>
    public bool IsGameCleared { get => isGameCleared; private set => isGameCleared = value; }
    
    /// <summary>
    /// 게임오버 여부 프로퍼티
    /// </summary>
    public bool IsGameOver { get => isGameOver; private set => isGameOver = value; }

    private void Awake()
    {
        // 싱글톤 패턴 구현
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        instance = this;
        // DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // UI 매니저 찾기
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
        
        // 게임 초기화
        ResetGame();
    }

    /// <summary>
    /// 점수 추가 메서드
    /// </summary>
    /// <param name="score">추가할 점수</param>
    public void AddScore(int score)
    {
        currentScore += score;
        
        // UI 업데이트
        if (uiManager != null)
        {
            uiManager.UpdateScoreText(currentScore);
        }
    }

    /// <summary>
    /// 게임 클리어 처리
    /// </summary>
    public void GameCleared()
    {
        if (isGameOver) return;
        
        isGameCleared = true;
        
        // 시간 멈추기
        Time.timeScale = 0f;
        
        // UI 표시
        if (uiManager != null)
        {
            uiManager.ShowClearUI();
        }
    }

    /// <summary>
    /// 플레이어 사망 처리
    /// </summary>
    public void PlayerDied()
    {
        if (isGameCleared) return;
        
        isGameOver = true;
        
        // 시간 멈추기
        Time.timeScale = 0f;
        
        // UI 표시
        if (uiManager != null)
        {
            uiManager.ShowGameOverUI();
        }
    }

    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        // 시간 복원
        Time.timeScale = 1f;
        
        // 현재 씬 재로드
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// 게임 초기화
    /// </summary>
    private void ResetGame()
    {
        currentScore = 0;
        isGameCleared = false;
        isGameOver = false;
        
        // 시간 복원
        Time.timeScale = 1f;
        
        // UI 초기화
        if (uiManager != null)
        {
            uiManager.UpdateScoreText(currentScore);
        }
    }
}
}