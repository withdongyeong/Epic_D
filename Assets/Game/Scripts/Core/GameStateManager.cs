using System;
using UnityEngine;

namespace Game.Scripts.Core
{
    /// <summary>
    /// 게임 상태를 관리하는 매니저 클래스
    /// </summary>
    public class GameStateManager : MonoBehaviour
    {
        public enum GameState
        {
            MainMenu,
            Playing,
            Victory,
            Defeat
        }

        private static GameStateManager _instance;
        private GameState _currentState;
        private TimeScaleManager _timeScaleManager;

        // 이벤트 정의
        public event Action<GameState> OnGameStateChanged;

        // 싱글톤 인스턴스
        public static GameStateManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<GameStateManager>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("GameStateManager");
                        _instance = obj.AddComponent<GameStateManager>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }

        public GameState CurrentState { get => _currentState; }

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 타임스케일 매니저 참조 확보
            _timeScaleManager = TimeScaleManager.Instance;
            
            // 기본 상태 설정
            SetGameState(GameState.Playing);
        }

        /// <summary>
        /// 게임 상태 변경
        /// </summary>
        public void SetGameState(GameState newState)
        {
            _currentState = newState;
            
            // 상태에 따른 타임스케일 조절
            switch (newState)
            {
                case GameState.Playing:
                    Time.timeScale = 1.0f;
                    break;
                case GameState.Victory:
                case GameState.Defeat:
                    Time.timeScale = 0.0f;
                    break;
            }
            
            OnGameStateChanged?.Invoke(newState);
            Debug.Log($"게임 상태 변경: {newState}, 타임스케일: {Time.timeScale}");
        }

        /// <summary>
        /// 게임 시작
        /// </summary>
        public void StartGame()
        {
            SetGameState(GameState.Playing);
        }

        /// <summary>
        /// 게임 승리
        /// </summary>
        public void WinGame()
        {
            SetGameState(GameState.Victory);
        }

        /// <summary>
        /// 게임 패배
        /// </summary>
        public void LoseGame()
        {
            SetGameState(GameState.Defeat);
        }

        /// <summary>
        /// 게임 재시작
        /// </summary>
        public void RestartGame()
        {
            _timeScaleManager.ResetTimeScale();
            SetGameState(GameState.Playing);
        }
    }
}