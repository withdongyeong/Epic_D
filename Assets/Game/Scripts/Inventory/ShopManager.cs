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
        private int _tryCount = 1;

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

        // 시도 횟수 프로퍼티
        public int TryCount
        {
            get => _tryCount;
            set
            {
                _tryCount = value;
                // 시도 횟수 증가 시 골드 업데이트
                UpdateGoldBasedOnTryCount();
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

            // 초기 골드 설정
            UpdateGoldBasedOnTryCount();
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
        /// 시도 횟수에 따라 골드 업데이트
        /// </summary>
        private void UpdateGoldBasedOnTryCount()
        {
            // 10 + (시도횟수 × 1)
            SetMoney(10 + (_tryCount - 1));
        }

        /// <summary>
        /// 골드 설정
        /// </summary>
        public void SetMoney(int amount)
        {
            if (amount >= 0)
            {
                Gold = amount;
                Debug.Log($"골드 설정: {_gold} (시도 횟수: {_tryCount})");
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

        /// <summary>
        /// 시도 횟수 증가
        /// </summary>
        public void IncreaseTryCount()
        {
            TryCount++;
            Debug.Log($"시도 횟수 증가: {_tryCount}");
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}