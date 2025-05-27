using Game4.Scripts.Character.Player;

namespace Game4.Scripts.Core
{
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 게임 매니저 싱글톤 인스턴스
    /// </summary>
    private static GameManager instance;
    
    /// <summary>
    /// 현재 게임 상태
    /// </summary>
    private GameState currentState = GameState.Start;
    
    /// <summary>
    /// 총 몬스터 수
    /// </summary>
    private int totalMonsters = 0;
    
    /// <summary>
    /// 현재 살아있는 몬스터 수
    /// </summary>
    private int remainingMonsters = 0;
    
    /// <summary>
    /// 플레이어 참조
    /// </summary>
    private PlayerController player;

    public GameObject playerSpawnPosition;
    
    /// <summary>
    /// 스폰 매니저 참조
    /// </summary>
    private SpawnManager spawnManager;
    
    /// <summary>
    /// UI 매니저 참조
    /// </summary>
    private UIManager uiManager;
    
    /// <summary>
    /// 보스 소환 이벤트
    /// </summary>
    public event Action OnBossSpawned;

    // Events
    /// <summary>
    /// 게임 상태 변경 이벤트
    /// </summary>
    public event Action<GameState> OnGameStateChanged;
    
    /// <summary>
    /// 몬스터 수 변경 이벤트
    /// </summary>
    public event Action<int> OnMonsterCountChanged;

    // Properties
    /// <summary>
    /// 게임 매니저 싱글톤 인스턴스 프로퍼티
    /// </summary>
    public static GameManager Instance { get => instance; private set => instance = value; }
    
    /// <summary>
    /// 현재 게임 상태 프로퍼티
    /// </summary>
    public GameState CurrentState { get => currentState; private set => currentState = value; }
    
    /// <summary>
    /// 총 몬스터 수 프로퍼티
    /// </summary>
    public int TotalMonsters { get => totalMonsters; private set => totalMonsters = value; }
    
    /// <summary>
    /// 현재 살아있는 몬스터 수 프로퍼티
    /// </summary>
    public int RemainingMonsters { get => remainingMonsters; private set => remainingMonsters = value; }

    /// <summary>
    /// 현재 보스 참조
    /// </summary>
    private GameObject currentBoss = null;

    /// <summary>
    /// 보스 소환 알림 (SpawnManager에서 호출)
    /// </summary>
    /// <param name="boss">소환된 보스</param>
    public void BossSpawned(GameObject boss)
    {
        currentBoss = boss;
        OnBossSpawned?.Invoke();
        Debug.Log("Boss spawned and tracked by GameManager");
    }
    /// <summary>
    /// 게임 상태 열거형
    /// </summary>
    public enum GameState
    {
        Start,      // 게임 시작 전
        Playing,    // 게임 진행 중
        GameOver,   // 게임 오버
        GameClear   // 게임 클리어
    }

    /// <summary>
    /// 싱글톤 초기화
    /// </summary>
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 게임 시작 시 초기화
    /// </summary>
    private void Start()
    {
        InitializeGame();
    }

    /// <summary>
    /// 게임 초기화
    /// </summary>
    private void InitializeGame()
    {
        // 매니저들 찾기
        spawnManager = FindObjectOfType<SpawnManager>();
        uiManager = FindObjectOfType<UIManager>();
        
        // 플레이어 찾기 (비활성화된 것도 포함)
        FindAndResetPlayer();
        
        // 초기 상태로 설정
        ChangeGameState(GameState.Start);
        
        Debug.Log("GameManager initialized");
    }

    /// <summary>
    /// 게임 시작
    /// </summary>
    public void StartGame()
    {
        Debug.Log($"StartGame called, current state: {currentState}");
    
        // Start 상태가 아니어도 시작할 수 있도록 수정 (재시작용)
        if (currentState == GameState.Playing) return;
    
        ChangeGameState(GameState.Playing);
    
        // 플레이어 활성화
        if (player != null)
        {
            player.CanControl = true;
        }
    
        // 보스 참조 초기화
        currentBoss = null;
    
        // 웨이브 시작 (기존 SpawnAllMonsters 대신)
        if (spawnManager != null)
        {
            spawnManager.StartWaves();
            totalMonsters = spawnManager.TotalMonsterCount;
            remainingMonsters = totalMonsters;
            OnMonsterCountChanged?.Invoke(remainingMonsters);
        }
    
        Debug.Log("Game Started with wave system!");
    }
    /// <summary>
    /// 게임 재시작
    /// </summary>
    public void RestartGame()
    {
        Debug.Log("RestartGame called");
    
        // 웨이브 중단
        if (spawnManager != null)
        {
            spawnManager.StopWaves();
        }
    
        // 기존 몬스터들 모두 제거
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject monster in monsters)
        {
            Destroy(monster);
        }
    
        // 플레이어 찾기 및 상태 초기화
        FindAndResetPlayer();
    
        // 게임 상태 완전 초기화
        totalMonsters = 0;
        remainingMonsters = 0;
        currentBoss = null; // 보스 참조 초기화 추가
    
        // 중요: 게임 상태를 Playing으로 직접 변경
        ChangeGameState(GameState.Playing);
    
        // 웨이브 다시 시작
        if (spawnManager != null)
        {
            spawnManager.StartWaves();
            totalMonsters = spawnManager.TotalMonsterCount;
            remainingMonsters = totalMonsters;
            OnMonsterCountChanged?.Invoke(remainingMonsters);
        }
    
        Debug.Log("Game Restarted with wave system!");
    }
    /// <summary>
    /// 플레이어 찾기 및 리셋
    /// </summary>
    private void FindAndResetPlayer()
    {
        // 모든 플레이어 오브젝트 찾기 (비활성화된 것도 포함)
        PlayerController[] allPlayers = FindObjectsOfType<PlayerController>(true);
        
        if (allPlayers.Length > 0)
        {
            player = allPlayers[0];
            player.ResetPlayer();
            Debug.Log("Player found and reset");
        }
        else
        {
            // 태그로 다시 찾기 시도
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.GetComponent<PlayerController>();
                if (player != null)
                {
                    player.ResetPlayer();
                    Debug.Log("Player found by tag and reset");
                }
            }
            else
            {
                Debug.LogError("Player not found! Make sure Player has 'Player' tag.");
            }
        }
    }

    /// <summary>
    /// 플레이어 사망 처리
    /// </summary>
    public void PlayerDied()
    {
        if (currentState != GameState.Playing) return;
        
        ChangeGameState(GameState.GameOver);
        
        Debug.Log("Player Died - Game Over!");
    }
    
    /// <summary>
    /// 몬스터 사망 처리
    /// </summary>
    public void MonsterDied()
    {
        if (currentState != GameState.Playing) return;
    
        remainingMonsters--;
        OnMonsterCountChanged?.Invoke(remainingMonsters);
    
        Debug.Log($"Monster died! Remaining: {remainingMonsters}");
    
        // 보스가 소환되었는지 확인
        bool bossHasSpawned = spawnManager != null && spawnManager.BossSpawned;
    
        // 보스가 소환되지 않았으면 클리어 안함
        if (!bossHasSpawned)
        {
            Debug.Log("Boss not spawned yet, continuing game...");
            return;
        }
    
        // 보스가 소환되었으면 보스가 죽었는지 확인
        bool bossIsDead = (currentBoss == null || !currentBoss.activeInHierarchy);
    
        if (bossIsDead)
        {
            ChangeGameState(GameState.GameClear);
            Debug.Log("Boss defeated - Game Clear!");
        }
        else
        {
            Debug.Log($"Boss still alive, remaining monsters: {remainingMonsters}");
        }
    }
    
    /// <summary>
    /// 보스 사망 처리 (보스 전용)
    /// </summary>
    public void BossDied()
    {
        if (currentState != GameState.Playing) return;
    
        Debug.Log("Boss has been defeated!");
    
        // 일반 몬스터 사망 처리도 함께
        MonsterDied();
    
        // 보스 참조 제거
        currentBoss = null;
    
        // 강제로 게임 클리어
        ChangeGameState(GameState.GameClear);
        Debug.Log("Boss defeated - Game Clear!");
    }
    
    /// <summary>
    /// 게임 상태 변경
    /// </summary>
    /// <param name="newState">새로운 게임 상태</param>
    private void ChangeGameState(GameState newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(currentState);
        
        // 상태에 따른 처리
        switch (currentState)
        {
            case GameState.Start:
                Time.timeScale = 0f; // 게임 일시정지
                break;
            case GameState.Playing:
                Time.timeScale = 1f; // 게임 재개
                break;
            case GameState.GameOver:
            case GameState.GameClear:
                Time.timeScale = 0f; // 게임 일시정지
                break;
        }
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
}