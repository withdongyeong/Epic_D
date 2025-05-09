namespace Game.Scripts.Tiles
{
    using UnityEngine;
    using TMPro;
    using UnityEngine.UI;
    
    /// <summary>
    /// 타일의 모든 시각적 표현 관리
    /// </summary>
    public class TileVisualController : MonoBehaviour
    {
        private Slider _chargeSlider;
        private GameObject _readyText;
        private BaseTile _tile;
        
        public Slider ChargeSlider { get => _chargeSlider; set => _chargeSlider = value; }
        public GameObject ReadyText { get => _readyText; set => _readyText = value; }
        
        /// <summary>
        /// 컴포넌트 초기화 및 이벤트 연결
        /// </summary>
        void Start()
        {
            _tile = GetComponent<BaseTile>();
            _readyText.SetActive(false);
            
            _tile.OnReady += HandleReady;
            _tile.OnActivated += HandleActivated;
        }
        
        /// <summary>
        /// 프레임별 시각 업데이트
        /// </summary>
        void Update()
        {
            UpdateVisuals();
        }
        
        /// <summary>
        /// 초기화
        /// </summary>
        
        void Awake()
        {
            _chargeSlider = GetComponentInChildren<Slider>();
            _readyText = GetComponentInChildren<TextMeshProUGUI>().gameObject;
        }
        
        /// <summary>
        /// 타일 상태에 따른 UI 요소 업데이트
        /// </summary>
        private void UpdateVisuals()
        {
            if (_tile.GetState() == BaseTile.TileState.Charging)
            {
                _chargeSlider.gameObject.SetActive(true);
                _readyText.SetActive(false);
                _chargeSlider.value = _tile.GetStateProgress();
            }
        }
        
        /// <summary>
        /// 타일 준비 상태 진입 처리
        /// </summary>
        private void HandleReady(BaseTile t)
        {
            _chargeSlider.gameObject.SetActive(false);
            _readyText.SetActive(true);
        }
        
        /// <summary>
        /// 타일 발동 상태 진입 처리
        /// </summary>
        private void HandleActivated(BaseTile t)
        {
            _readyText.SetActive(false);
        }
    }
}