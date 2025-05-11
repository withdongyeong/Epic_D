using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 상점 관리 시스템
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        private static ShopManager _instance;

        [SerializeField] private int _gold = 10;
        [SerializeField] private TextMeshProUGUI _goldText;

        // 싱글톤 인스턴스
        public static ShopManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject shopManagerObj = new GameObject("ShopManager");
                    _instance = shopManagerObj.AddComponent<ShopManager>();
                    DontDestroyOnLoad(shopManagerObj);
                }

                return _instance;
            }
        }

        // 보유 골드 프로퍼티
        public int Gold
        {
            get => _gold;
            private set
            {
                _gold = value;
                UpdateGoldUI();
            }
        }

        private void Awake()
        {
            // 싱글톤 처리
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // Money 텍스트 찾기
            FindMoneyText();

            // Scene 로드 이벤트 등록
            SceneManager.sceneLoaded += OnSceneLoaded;

            // UI 초기화
            UpdateGoldUI();
        }

        /// <summary>
        /// Scene 로드 시 Money 텍스트 다시 찾기
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            FindMoneyText();
        }

        /// <summary>
        /// Money 텍스트 찾기
        /// </summary>
        private void FindMoneyText()
        {
            if (_goldText == null)
            {
                GameObject moneyTextObj = GameObject.Find("Money");
                if (moneyTextObj != null)
                {
                    _goldText = moneyTextObj.GetComponent<TextMeshProUGUI>();
                    if (_goldText == null)
                    {
                        _goldText = moneyTextObj.GetComponent<Text>()?.GetComponent<TextMeshProUGUI>();
                    }
                }

                if (_goldText == null)
                {
                    Debug.LogWarning("Money 텍스트를 찾을 수 없습니다.");
                }
                else
                {
                    UpdateGoldUI();
                }
            }
        }

        /// <summary>
        /// 골드 UI 업데이트
        /// </summary>
        private void UpdateGoldUI()
        {
            if (_goldText != null)
            {
                _goldText.text = $"Gold: {_gold}";
            }
        }

        /// <summary>
        /// 아이템 구매 가능 여부 확인
        /// </summary>
        /// <param name="itemData">구매할 아이템 데이터</param>
        /// <returns>구매 가능 여부</returns>
        public bool CanPurchase(InventoryItemData itemData)
        {
            return itemData != null && _gold >= itemData.Cost;
        }

        /// <summary>
        /// 아이템 구매 처리
        /// </summary>
        /// <param name="itemData">구매할 아이템 데이터</param>
        /// <returns>구매 성공 여부</returns>
        public bool Purchase(InventoryItemData itemData)
        {
            if (!CanPurchase(itemData))
            {
                Debug.Log($"구매 실패: 골드 부족 (보유: {_gold}, 필요: {itemData.Cost})");
                return false;
            }

            // 골드 차감
            Gold -= itemData.Cost;
            Debug.Log($"아이템 구매 완료: {itemData.ItemName}, 비용: {itemData.Cost}, 남은 골드: {_gold}");

            return true;
        }

        /// <summary>
        /// 골드 추가
        /// </summary>
        public void AddGold(int amount)
        {
            if (amount > 0)
            {
                Gold += amount;
                Debug.Log($"골드 획득: +{amount}, 총 골드: {_gold}");
            }
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}