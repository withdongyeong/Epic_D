using System.Collections;
using System.Collections.Generic;
using Game2.Scripts.Character.Enemy;
using Game2.Scripts.Character.Player;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 몬스터 책 UI를 관리하는 클래스
/// </summary>
public class BookUI : MonoBehaviour
{
    [Header("UI 요소")]
    [SerializeField] private GameObject _bookPanel;
    [SerializeField] private TMP_Text _monsterNameText;
    [SerializeField] private Button _closeButton;
    [SerializeField] private RectTransform _contentContainer;
    [SerializeField] private GameObject _traitButtonPrefab;
    
    [Header("시간 제한 설정")]
    [SerializeField] private float _timeLimit = 3f; // 기본 3초
    [SerializeField] private Slider _timerSlider; // 타이머 UI
    [SerializeField] private TMP_Text _timerText; // 타이머 텍스트 (선택 사항)
    
    private Monster _currentMonster;
    private List<GameObject> _spawnedTraits = new List<GameObject>();
    private HeavensDoor _heavensDoor;
    private Coroutine _timerCoroutine;
    
    private void Awake()
    {
        _bookPanel.SetActive(false);
        _closeButton.onClick.AddListener(OnCloseButtonClicked);
        
        _heavensDoor = FindObjectOfType<HeavensDoor>();
        
        // 타이머 슬라이더 초기화
        if (_timerSlider != null)
        {
            _timerSlider.minValue = 0f;
            _timerSlider.maxValue = _timeLimit;
            _timerSlider.value = _timeLimit;
        }
    }
    /// <summary>
    /// 몬스터 책 표시
    /// </summary>
    public void ShowBook(Monster monster)
    {
        _currentMonster = monster;
        
        // 기존 타이머가 있다면 중지
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
        }
        
        // 기존 특성 UI 정리
        ClearTraitElements();
        
        // 몬스터 이름 설정
        _monsterNameText.text = monster.GetType().Name.Replace("Monster", "");
        
        // 특성 버튼 생성
        CreateTraitButtons(monster.GetTraits());
        
        // 타이머 초기화
        if (_timerSlider != null)
        {
            _timerSlider.value = _timeLimit;
        }
        
        if (_timerText != null)
        {
            _timerText.text = _timeLimit.ToString("F1") + "초";
        }
        
        // 패널 표시
        _bookPanel.SetActive(true);
        
        // 타이머 시작
        _timerCoroutine = StartCoroutine(BookTimerRoutine());
    }
    
    /// <summary>
    /// 책 타이머 코루틴
    /// </summary>
    private IEnumerator BookTimerRoutine()
    {
        float remainingTime = _timeLimit;
        
        while (remainingTime > 0)
        {
            // 매 프레임마다 시간 감소
            remainingTime -= Time.unscaledDeltaTime; // unscaledDeltaTime을 사용하여 타임스케일 영향 받지 않도록 함
            
            // UI 업데이트
            if (_timerSlider != null)
            {
                _timerSlider.value = remainingTime;
            }
            
            if (_timerText != null)
            {
                _timerText.text = remainingTime.ToString("F1") + "초";
                
                // 남은 시간이 적으면 색상 변경 (경고)
                if (remainingTime <= 3.0f)
                {
                    _timerText.color = Color.red;
                    
                    // 깜빡임 효과 (1초 남았을 때)
                    if (remainingTime <= 1.0f)
                    {
                        _timerText.color = Mathf.Sin(Time.unscaledTime * 10f) > 0 ? Color.red : Color.white;
                    }
                }
            }
            
            yield return null;
        }
        
        // 시간이 다 되면 자동으로 닫기
        OnCloseButtonClicked();
    }
    
    /// <summary>
    /// 닫기 버튼 클릭 처리
    /// </summary>
    private void OnCloseButtonClicked()
    {
        // 타이머 중지
        if (_timerCoroutine != null)
        {
            StopCoroutine(_timerCoroutine);
            _timerCoroutine = null;
        }
        
        _bookPanel.SetActive(false);
        
        // HeavensDoor에게 책이 닫혔음을 알림
        if (_heavensDoor != null)
        {
            _heavensDoor.OnBookClosed();
        }
    }
    
    /// <summary>
    /// 특성 버튼 생성
    /// </summary>
    private void CreateTraitButtons(Dictionary<string, Monster.MonsterTrait> traits)
    {
        foreach (var trait in traits)
        {
            GameObject buttonObj = Instantiate(_traitButtonPrefab, _contentContainer);
            _spawnedTraits.Add(buttonObj);
            
            // 버튼 텍스트 설정
            TMP_Text buttonText = buttonObj.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = trait.Value.currentText;
            }
            
            // 클릭 이벤트 설정
            Button button = buttonObj.GetComponent<Button>();
            if (button != null)
            {
                string traitKey = trait.Key;
                button.onClick.AddListener(() => OnTraitButtonClicked(traitKey));
            }
        }
    }
    
    /// <summary>
    /// 특성 버튼 클릭 처리
    /// </summary>
    private void OnTraitButtonClicked(string traitKey)
    {
        if (_currentMonster != null && _currentMonster.GetTraits().TryGetValue(traitKey, out Monster.MonsterTrait trait))
        {
            // 특성 스왑
            trait.Swap();
            
            // UI 갱신
            RefreshBookUI();
        }
    }
    
    /// <summary>
    /// 책 UI 갱신
    /// </summary>
    private void RefreshBookUI()
    {
        ClearTraitElements();
        CreateTraitButtons(_currentMonster.GetTraits());
    }
    
    /// <summary>
    /// 특성 요소 정리
    /// </summary>
    private void ClearTraitElements()
    {
        foreach (GameObject obj in _spawnedTraits)
        {
            Destroy(obj);
        }
        _spawnedTraits.Clear();
    }
    
    /// <summary>
    /// 책 닫기
    /// </summary>
    /// <summary>
    /// 책 닫기
    /// </summary>
    private void CloseBook()
    {
        _bookPanel.SetActive(false);
    
        // HeavensDoor의 CloseBook 함수 호출
        HeavensDoor heavensDoor = FindObjectOfType<HeavensDoor>();
        if (heavensDoor != null)
        {
            heavensDoor.CloseBook();
        }
    }
}