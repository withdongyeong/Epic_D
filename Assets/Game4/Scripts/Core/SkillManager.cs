using Game4.Scripts.Character.Player;
using Game4.Scripts.Sword;

namespace Game4.Scripts.Core
{
#pragma warning disable CS0618, CS0612, CS0672
using UnityEngine;
using System.Collections.Generic;
using System;
#pragma warning restore CS0618, CS0612, CS0672

public class SkillManager : MonoBehaviour
{
    /// <summary>
    /// 칼날폭풍 스킬 쿨타임
    /// </summary>
    private float bladeStormCooldown = 5f;

    /// <summary>
    /// 칼날폭풍 현재 쿨타임
    /// </summary>
    private float bladeStormCurrentCooldown = 0f;

    /// <summary>
    /// 칼날폭풍 스킬 사용 가능 여부
    /// </summary>
    public bool IsBladeStormReady => bladeStormCurrentCooldown <= 0f;
    
    /// <summary>
    /// 스킬 쿨타임 (초)
    /// </summary>
    private float skillCooldown = 1f;
    
    /// <summary>
    /// 현재 쿨타임 타이머
    /// </summary>
    private float currentCooldownTimer = 0f;
    
    /// <summary>
    /// 모든 검 컨트롤러 리스트
    /// </summary>
    private List<SwordController> swords = new List<SwordController>();

    // Events
    /// <summary>
    /// 스킬 사용 이벤트
    /// </summary>
    public event Action OnSkillUsed;
    
    /// <summary>
    /// 쿨타임 업데이트 이벤트 (남은 시간, 전체 시간)
    /// </summary>
    public event Action<float, float> OnCooldownUpdate;

    // Properties
    /// <summary>
    /// 스킬 쿨타임 프로퍼티
    /// </summary>
    public float SkillCooldown { get => skillCooldown; set => skillCooldown = value; }
    
    /// <summary>
    /// 현재 쿨타임 타이머 프로퍼티
    /// </summary>
    public float CurrentCooldownTimer { get => currentCooldownTimer; private set => currentCooldownTimer = value; }
    
    /// <summary>
    /// 스킬 사용 가능 여부 프로퍼티
    /// </summary>
    public bool IsSkillReady => currentCooldownTimer <= 0f;
    
    /// <summary>
    /// 쿨타임 진행률 프로퍼티 (0~1)
    /// </summary>
    public float CooldownProgress => IsSkillReady ? 1f : (skillCooldown - currentCooldownTimer) / skillCooldown;

    /// <summary>
    /// 칼날폭풍 쿨타임 업데이트 이벤트 (남은 시간, 전체 시간)
    /// </summary>
    public event Action<float, float> OnBladeStormCooldownUpdate;
    
    /// <summary>
    /// 초기화
    /// </summary>
    private void Start()
    {
        RefreshSwordList();
        Debug.Log("SkillManager initialized");
    }

    /// <summary>
    /// 매 프레임 업데이트
    /// </summary>
    private void Update()
    {
        HandleInput();
        UpdateCooldown();
    }

    private void HandleInput()
    {
        // 기존 마우스 클릭 스킬
        if (Input.GetMouseButtonDown(0) && IsSkillReady)
        {
            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPosition.z = 0f;
            ActivateSkill(mouseWorldPosition);
        }
    
        // 새로운 스페이스 스킬
        if (Input.GetKeyDown(KeyCode.Space) && IsBladeStormReady)
        {
            ActivateBladeStorm();
        }
    }

    /// <summary>
    /// 칼날폭풍 스킬 발동
    /// </summary>
    public void ActivateBladeStorm()
    {
        // 쿨타임 시작
        bladeStormCurrentCooldown = bladeStormCooldown;
        OnBladeStormCooldownUpdate?.Invoke(bladeStormCurrentCooldown, bladeStormCooldown);
        
        // 플레이어 현재 위치 저장
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;
    
        Vector3 playerStartPosition = playerObj.transform.position;
        PlayerController playerController = playerObj.GetComponent<PlayerController>();
    
        // 플레이어 이동 방향 계산
        Vector2 moveDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.W)) moveDirection.y += 1;
        if (Input.GetKey(KeyCode.S)) moveDirection.y -= 1;
        if (Input.GetKey(KeyCode.A)) moveDirection.x -= 1;
        if (Input.GetKey(KeyCode.D)) moveDirection.x += 1;
    
        if (moveDirection.magnitude < 0.1f)
        {
            moveDirection = Vector2.up; // 기본 방향
        }
    
        // 플레이어 대시
        if (playerController != null)
        {
            playerController.StartDash(moveDirection);
        }
    
        // 검들을 시작 위치로 모으기
        RefreshSwordList();
        foreach (SwordController sword in swords)
        {
            if (sword != null && sword.CurrentState == SwordController.SwordState.Flying)
            {
                sword.ActivateBladeStorm(playerStartPosition);
            }
        }
    
        // 쿨타임 시작
        bladeStormCurrentCooldown = bladeStormCooldown;
    
        Debug.Log("Blade Storm activated!");
    }

    private void UpdateCooldown()
    {
        // 기존 스킬 쿨타임
        if (currentCooldownTimer > 0f)
        {
            currentCooldownTimer -= Time.deltaTime;
            OnCooldownUpdate?.Invoke(currentCooldownTimer, skillCooldown);
        
            if (currentCooldownTimer <= 0f)
            {
                currentCooldownTimer = 0f;
            }
        }
    
        // 칼날폭풍 쿨타임
        if (bladeStormCurrentCooldown > 0f)
        {
            bladeStormCurrentCooldown -= Time.deltaTime;
            OnBladeStormCooldownUpdate?.Invoke(bladeStormCurrentCooldown, bladeStormCooldown);
        
            if (bladeStormCurrentCooldown <= 0f)
            {
                bladeStormCurrentCooldown = 0f;
                Debug.Log("Blade Storm ready!");
            }
        }
    }

    /// <summary>
    /// 스킬 발동
    /// </summary>
    /// <param name="targetPosition">목표 위치</param>
    public void ActivateSkill(Vector3 targetPosition)
    {
        if (!IsSkillReady) return;
        
        // 검 리스트 새로고침 (혹시 검이 추가/제거된 경우)
        RefreshSwordList();
        
        // 모든 검에게 스킬 명령
        foreach (SwordController sword in swords)
        {
            if (sword != null && sword.CurrentState == SwordController.SwordState.Flying)
            {
                sword.ActivateSkill(targetPosition);
            }
        }
        
        // 쿨타임 시작
        currentCooldownTimer = skillCooldown;
        
        // 이벤트 호출
        OnSkillUsed?.Invoke();
        OnCooldownUpdate?.Invoke(currentCooldownTimer, skillCooldown);
        
        Debug.Log($"Skill activated at position: {targetPosition}");
    }

    /// <summary>
    /// 검 리스트 새로고침
    /// </summary>
    public void RefreshSwordList()
    {
        swords.Clear();
        SwordController[] foundSwords = FindObjectsOfType<SwordController>();
        swords.AddRange(foundSwords);
        
        Debug.Log($"Found {swords.Count} swords");
    }

    /// <summary>
    /// 특정 검을 리스트에 추가
    /// </summary>
    /// <param name="sword">추가할 검</param>
    public void AddSword(SwordController sword)
    {
        if (sword != null && !swords.Contains(sword))
        {
            swords.Add(sword);
        }
    }

    /// <summary>
    /// 특정 검을 리스트에서 제거
    /// </summary>
    /// <param name="sword">제거할 검</param>
    public void RemoveSword(SwordController sword)
    {
        if (swords.Contains(sword))
        {
            swords.Remove(sword);
        }
    }

    /// <summary>
    /// 강제로 스킬 쿨타임 초기화 (치트용)
    /// </summary>
    public void ResetCooldown()
    {
        currentCooldownTimer = 0f;
        OnCooldownUpdate?.Invoke(currentCooldownTimer, skillCooldown);
        Debug.Log("Skill cooldown reset");
    }
}
}