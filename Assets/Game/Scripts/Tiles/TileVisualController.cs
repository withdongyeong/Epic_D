using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// 타일의 모든 시각적 표현 관리
/// </summary>
public class TileVisualController : MonoBehaviour
{
    private Slider chargeSlider;
    private GameObject readyText;
    private BaseTile tile;
    
    public Slider ChargeSlider { get => chargeSlider; set => chargeSlider = value; }
    public GameObject ReadyText { get => readyText; set => readyText = value; }
    
    /// <summary>
    /// 컴포넌트 초기화 및 이벤트 연결
    /// </summary>
    void Start()
    {
        tile = GetComponent<BaseTile>();
        readyText.SetActive(false);
        
        tile.OnReady += HandleReady;
        tile.OnActivated += HandleActivated;
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
        chargeSlider = GetComponentInChildren<Slider>();
        readyText = GetComponentInChildren<TextMeshProUGUI>().gameObject;
    }
    
    /// <summary>
    /// 타일 상태에 따른 UI 요소 업데이트
    /// </summary>
    private void UpdateVisuals()
    {
        if (tile.GetState() == BaseTile.TileState.Charging)
        {
            chargeSlider.gameObject.SetActive(true);
            readyText.SetActive(false);
            chargeSlider.value = tile.GetStateProgress();
        }
    }
    
    /// <summary>
    /// 타일 준비 상태 진입 처리
    /// </summary>
    private void HandleReady(BaseTile t)
    {
        chargeSlider.gameObject.SetActive(false);
        readyText.SetActive(true);
    }
    
    /// <summary>
    /// 타일 발동 상태 진입 처리
    /// </summary>
    private void HandleActivated(BaseTile t)
    {
        readyText.SetActive(false);
    }
}