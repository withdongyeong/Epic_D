using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Core
{
    /// <summary>
    /// 씬 변경 시 Time.timeScale 관리
    /// </summary>
    public class TimeScaleManager : MonoBehaviour
    {
        private static TimeScaleManager _instance;
        
        public static TimeScaleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindAnyObjectByType<TimeScaleManager>();
                    if (_instance == null)
                    {
                        GameObject obj = new GameObject("TimeScaleManager");
                        _instance = obj.AddComponent<TimeScaleManager>();
                        DontDestroyOnLoad(obj);
                    }
                }
                return _instance;
            }
        }
        
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            // 씬 로드 이벤트 등록
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        /// <summary>
        /// 씬 전환시 타임 스케일 초기화 안전장치
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Time.timeScale = 1.0f;
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        
        /// <summary>
        /// 타임스케일 강제 초기화
        /// </summary>
        public void ResetTimeScale()
        {
            Time.timeScale = 1.0f;
        }
    }
}