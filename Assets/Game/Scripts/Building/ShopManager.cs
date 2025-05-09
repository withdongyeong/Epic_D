using TMPro;

namespace Game.Scripts.Building
{
    using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Building
{
    /// <summary>
    /// 상점 시스템 관리 클래스
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        [Header("상점 설정")]
        public int maxRerollCount = 3;
        public int itemsPerRoll = 4;
        
        [Header("UI 요소")]
        public Transform shopItemContainer;
        public Button rerollButton;
        public TextMeshProUGUI rerollCountText;
        public GameObject tilePrefab;
        
        [Header("타일 데이터")]
        public List<TileData> availableTiles = new List<TileData>();
        
        private int _currentRerollCount;
        private List<GameObject> _currentShopItems = new List<GameObject>();
        
        private void Start()
        {
            
            // 기본 타일 데이터 추가
            if (availableTiles.Count == 0)
            {
                InitializeDefaultTiles();
            }
            
            _currentRerollCount = maxRerollCount;
            UpdateRerollCountUI();
            
            // 리롤 버튼 이벤트 연결
            if (rerollButton != null)
            {
                rerollButton.onClick.AddListener(RerollShopItems);
            }
            
            // 초기 상점 아이템 생성
            RerollShopItems();
        }
        
        /// <summary>
        /// 초기화
        /// </summary>
        private void InitializeDefaultTiles()
        {
            // 기본 1x1 공격 타일
            availableTiles.Add(new TileData(
                TileData.TileType.Attack,
                "Basic Attack",
                1, 1, 10));
        
            // 2x1 공격 타일
            availableTiles.Add(new TileData(
                TileData.TileType.Attack,
                "Double Attack",
                2, 1, 15));
        }
        
        /// <summary>
        /// 상점 아이템 리롤
        /// </summary>
        public void RerollShopItems()
        {
            if (_currentRerollCount <= 0)
            {
                Debug.Log("리롤 횟수가 부족합니다.");
                return;
            }
            
            // 리롤 횟수 차감
            _currentRerollCount--;
            UpdateRerollCountUI();
            
            // 기존 아이템 제거
            ClearShopItems();
            
            // 새 아이템 생성
            for (int i = 0; i < itemsPerRoll; i++)
            {
                CreateRandomShopItem();
            }
        }
        
        /// <summary>
        /// 리롤 횟수 UI 업데이트
        /// </summary>
        private void UpdateRerollCountUI()
        {
            if (rerollCountText != null)
            {
                rerollCountText.text = $"리롤: {_currentRerollCount}/{maxRerollCount}";
            }
            
            // 리롤 버튼 활성화/비활성화
            if (rerollButton != null)
            {
                rerollButton.interactable = _currentRerollCount > 0;
            }
        }
        
        /// <summary>
        /// 상점 아이템 제거
        /// </summary>
        private void ClearShopItems()
        {
            foreach (GameObject item in _currentShopItems)
            {
                Destroy(item);
            }
            
            _currentShopItems.Clear();
        }
        
        /// <summary>
        /// 랜덤 상점 아이템 생성
        /// </summary>
        private void CreateRandomShopItem()
        {
            if (availableTiles.Count == 0 || tilePrefab == null || shopItemContainer == null)
            {
                Debug.LogError("상점 아이템 생성 실패: 필요한 리소스가 부족합니다.");
                return;
            }
            
            // 랜덤 타일 데이터 선택
            TileData tileData = availableTiles[Random.Range(0, availableTiles.Count)];
            
            // 타일 UI 생성
            GameObject tileObj = Instantiate(tilePrefab, shopItemContainer);
            _currentShopItems.Add(tileObj);
            
            // 타일 드래그 핸들러 설정
            TileDragHandler dragHandler = tileObj.GetComponent<TileDragHandler>();
            if (dragHandler != null)
            {
                dragHandler.SetTileData(tileData);
            }
        }
    }
}
}