using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Game.Scripts.Inventory;

namespace Game.Scripts.UI
{
    public class BuildingSceneUI : MonoBehaviour
    {
        public static BuildingSceneUI Instance { get; private set; }
        
        [SerializeField] private Button startButton;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            FindStartButton();
        }
        
        private void FindStartButton()
        {
            if (startButton == null)
                startButton = GameObject.Find("Start")?.GetComponent<Button>();
                
            if (startButton != null)
                startButton.onClick.AddListener(StartGame);
            else
                Debug.LogError("StartButton을 찾을 수 없습니다");
        }
        
        private void Start()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "BuildingScene")
                FindStartButton();
        }
        
        public void StartGame()
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.CollectTilesFromGrid();
            
            SceneManager.LoadScene("GameplayScene");
        }
        
        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}