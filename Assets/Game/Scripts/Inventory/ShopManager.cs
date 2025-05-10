using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Inventory
{
    /// <summary>
    /// 상점 관리 클래스
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        [Header("상점 설정")]
        [SerializeField] private int _initialShopSlots = 3; // 상점 슬롯 수
        [SerializeField] private int _maxRerolls = 5; // 최대 리롤 횟수
        [SerializeField] private int _currentRerolls; // 현재 남은 리롤 횟수
        
        [Header("UI 요소")]
        [SerializeField] private RectTransform _shopContainer; // 상점 아이템 컨테이너
        [SerializeField] private Button _rerollButton; // 리롤 버튼
        [SerializeField] private Text _rerollCountText; // 리롤 횟수 텍스트
        
        [Header("프리팹")]
        [SerializeField] private GameObject _inventoryItemPrefab; // 인벤토리 아이템 프리팹
        
        [Header("아이템 데이터")]
        [SerializeField] private List<InventoryItemData> _availableItems = new List<InventoryItemData>(); // 사용 가능한 아이템 목록
        
        private List<InventoryItem> _currentShopItems = new List<InventoryItem>(); // 현재 상점에 표시된 아이템들
        
        /// <summary>
        /// 현재 남은 리롤 횟수 프로퍼티
        /// </summary>
        public int CurrentRerolls { get => _currentRerolls; }
        
        private void Start()
        {
            _currentRerolls = _maxRerolls;
            UpdateRerollUI();
            
            // 리롤 버튼 이벤트 연결
            if (_rerollButton != null)
            {
                _rerollButton.onClick.AddListener(RerollShop);
            }
            
            // 초기 상점 아이템 생성
            RerollShop();
        }
        
        /// <summary>
        /// 상점 아이템 리롤
        /// </summary>
        public void RerollShop()
        {
            // 리롤 가능 여부 확인
            if (_currentRerolls <= 0)
            {
                Debug.Log("리롤 횟수가 부족합니다.");
                return;
            }
            
            // 리롤 횟수 감소
            _currentRerolls--;
            UpdateRerollUI();
            
            // 기존 상점 아이템 제거
            ClearShopItems();
            
            // 새 아이템 생성
            for (int i = 0; i < _initialShopSlots; i++)
            {
                CreateRandomShopItem();
            }
        }
        
        /// <summary>
        /// 리롤 UI 업데이트
        /// </summary>
        private void UpdateRerollUI()
        {
            if (_rerollCountText != null)
            {
                _rerollCountText.text = $"리롤: {_currentRerolls} / {_maxRerolls}";
            }
            
            // 리롤 버튼 활성화/비활성화
            if (_rerollButton != null)
            {
                _rerollButton.interactable = _currentRerolls > 0;
            }
        }
        
        /// <summary>
        /// 기존 상점 아이템 모두 제거
        /// </summary>
        private void ClearShopItems()
        {
            foreach (InventoryItem item in _currentShopItems)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }
            
            _currentShopItems.Clear();
        }
        
        /// <summary>
        /// 랜덤 상점 아이템 생성
        /// </summary>
        private void CreateRandomShopItem()
        {
            if (_availableItems.Count == 0 || _inventoryItemPrefab == null || _shopContainer == null)
            {
                Debug.LogWarning("상점 아이템을 생성할 수 없습니다. 필요한 참조가 없습니다.");
                return;
            }
            
            // 랜덤 아이템 데이터 선택
            int randomIndex = Random.Range(0, _availableItems.Count);
            InventoryItemData itemData = _availableItems[randomIndex];
            
            // 아이템 생성
            GameObject itemObject = Instantiate(_inventoryItemPrefab, _shopContainer);
            InventoryItem item = itemObject.GetComponent<InventoryItem>();
            
            if (item != null)
            {
                // 아이템 데이터 설정
                item.Initialize(itemData.Clone());
                
                // 상점 내 위치 설정
                float itemSpacing = 110f; // 아이템 간격
                RectTransform rectTransform = itemObject.GetComponent<RectTransform>();
                rectTransform.anchoredPosition = new Vector2((_currentShopItems.Count * itemSpacing), 0);
                
                // 목록에 추가
                _currentShopItems.Add(item);
            }
        }
        
        /// <summary>
        /// 상점 데이터 리셋 (새 게임 시작 시)
        /// </summary>
        public void ResetShop()
        {
            _currentRerolls = _maxRerolls;
            UpdateRerollUI();
            RerollShop();
        }
    }
}