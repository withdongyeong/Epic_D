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
            
            // 기본 상태 설정
            SetGameState(GameState.MainMenu);
        }

        /// <summary>
        /// 게임 상태 변경
        /// </summary>
        public void SetGameState(GameState newState)
        {
            _currentState = newState;
            OnGameStateChanged?.Invoke(newState);
            
            Debug.Log($"게임 상태 변경: {newState}");
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
            SetGameState(GameState.Playing);
        }
    }
}