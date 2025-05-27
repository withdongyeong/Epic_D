using System.Collections;
using Game3.Scripts.Arrow;
using Game3.Scripts.Core;
using Game3.Scripts.Core.Game3.Scripts.Core;
using UnityEngine;

namespace Game3.Scripts.Character.Player
{
    public class PlayerController : MonoBehaviour
    {
        /// <summary>
        /// 목표 마커 프리팹
        /// </summary>
        public GameObject targetMarkerPrefab;
        
        /// <summary>
        /// 목표 마커 인스턴스
        /// </summary>
        private GameObject targetMarkerInstance;
        
        /// <summary>
        /// 가이드 라인 컴포넌트
        /// </summary>
        private AimLine aimLine;
        
        /// <summary>
        /// 현재 클릭된 목표 위치
        /// </summary>
        private Vector2 targetPosition;
        
        /// <summary>
        /// 최초 클릭 위치
        /// </summary>
        private Vector2 initialClickPosition;
        
        /// <summary>
        /// 현재 마우스 위치
        /// </summary>
        private Vector2 currentMousePosition;
        
        /// <summary>
        /// 화살 궤적 곡률 계수
        /// </summary>
        private float curveFactor = 0.5f;
        
        /// <summary>
        /// 최대 곡률 허용치
        /// </summary>
        private float maxCurveMagnitude = 5f;
        
        /// <summary>
        /// 플레이어 이동 속도
        /// </summary>
        private float moveSpeed = 5f;
        
        /// <summary>
        /// 플레이어 최대 체력
        /// </summary>
        private int maxHealth = 10000;
        
        /// <summary>
        /// 현재 플레이어 체력
        /// </summary>
        private int currentHealth;
        
        /// <summary>
        /// 화살 프리팹
        /// </summary>
        public GameObject arrowPrefab;
        
        /// <summary>
        /// 화살 발사 위치
        /// </summary>
        public Transform firePoint;
        
        /// <summary>
        /// 화살 발사 속도
        /// </summary>
        private float arrowSpeed = 10f;
        
        /// <summary>
        /// 화살 발사 쿨다운
        /// </summary>
        private float shootCooldown = 0f;
        
        /// <summary>
        /// 현재 쿨다운 시간
        /// </summary>
        private float currentCooldown = 0f;
        
        /// <summary>
        /// 현재 파워 게이지 값 (0-1 사이)
        /// </summary>
        private float currentPower = 0f;
        
        /// <summary>
        /// 파워 게이지 최대 차징 시간 (초)
        /// </summary>
        private float maxChargeTime = 2f;
        
        /// <summary>
        /// 최소 파워 임계값 (0.5 = 50%)
        /// </summary>
        private float minPowerThreshold = 0f;
        
        /// <summary>
        /// 차징 중 여부
        /// </summary>
        private bool isCharging = false;
        
        /// <summary>
        /// 화살 속도 증가 배율 (파워에 비례)
        /// </summary>
        private float powerSpeedMultiplier = 1.5f;

        /// <summary>
        /// 리지드바디 컴포넌트
        /// </summary>
        private Rigidbody2D rb;
        
        /// <summary>
        /// 플레이어 제어 가능 여부
        /// </summary>
        private bool canControl = true;
        
        /// <summary>
        /// 게임 매니저 참조
        /// </summary>
        private GameManager gameManager;
        
        /// <summary>
        /// UI 매니저 참조
        /// </summary>
        private UIManager uiManager;
        
        /// <summary>
        /// 플레이어 최대 체력 프로퍼티
        /// </summary>
        public int MaxHealth { get => maxHealth; set => maxHealth = value; }
        
        /// <summary>
        /// 현재 플레이어 체력 프로퍼티
        /// </summary>
        public int CurrentHealth { get => currentHealth; set => currentHealth = value; }
        
        /// <summary>
        /// 플레이어 제어 가능 여부 프로퍼티
        /// </summary>
        public bool CanControl { get => canControl; set => canControl = value; }
        
        /// <summary>
        /// 플레이어 이동 속도 프로퍼티
        /// </summary>
        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
        
        /// <summary>
        /// 화살 프리팹 프로퍼티
        /// </summary>
        public GameObject ArrowPrefab { get => arrowPrefab; set => arrowPrefab = value; }
        
        /// <summary>
        /// 화살 발사 위치 프로퍼티
        /// </summary>
        public Transform FirePoint { get => firePoint; set => firePoint = value; }
        
        /// <summary>
        /// 화살 발사 속도 프로퍼티
        /// </summary>
        public float ArrowSpeed { get => arrowSpeed; set => arrowSpeed = value; }
        
        /// <summary>
        /// 화살 발사 쿨다운 프로퍼티
        /// </summary>
        public float ShootCooldown { get => shootCooldown; set => shootCooldown = value; }
        
        /// <summary>
        /// 현재 파워 게이지 값 프로퍼티
        /// </summary>
        public float CurrentPower { get => currentPower; set => currentPower = value; }
        
        /// <summary>
        /// 파워 게이지 최대 차징 시간 프로퍼티
        /// </summary>
        public float MaxChargeTime { get => maxChargeTime; set => maxChargeTime = value; }
        
        /// <summary>
        /// 최소 파워 임계값 프로퍼티
        /// </summary>
        public float MinPowerThreshold { get => minPowerThreshold; set => minPowerThreshold = value; }
        
        /// <summary>
        /// 목표 마커 프리팹 프로퍼티
        /// </summary>
        public GameObject TargetMarkerPrefab { get => targetMarkerPrefab; set => targetMarkerPrefab = value; }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            
            // 가이드 라인 컴포넌트 추가
            aimLine = gameObject.AddComponent<AimLine>();
            
            // 목표 마커 생성
            if (targetMarkerPrefab != null)
            {
                targetMarkerInstance = Instantiate(targetMarkerPrefab);
                targetMarkerInstance.SetActive(false);
            }
            else
            {
                // 프리팹이 없으면 기본 마커 생성
                targetMarkerInstance = new GameObject("TargetMarker");
                SpriteRenderer spriteRenderer = targetMarkerInstance.AddComponent<SpriteRenderer>();
                
                // 간단한 원형 스프라이트 생성 (Unity 기본 스프라이트 사용)
                spriteRenderer.sprite = Resources.Load<Sprite>("UI/Circle");
                if (spriteRenderer.sprite == null)
                {
                    Debug.LogWarning("기본 원형 스프라이트를 찾을 수 없습니다. 목표 마커에 적절한 스프라이트를 할당해주세요.");
                }
                
                spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 0.7f);
                targetMarkerInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                targetMarkerInstance.SetActive(false);
                
                // TargetMarker 컴포넌트 추가
                targetMarkerInstance.AddComponent<TargetMarker>();
            }
        }

        private void Start()
        {
            currentHealth = maxHealth;
            gameManager = GameManager.Instance;
            uiManager = FindObjectOfType<UIManager>();
            
            // 시작 시 UI 매니저에 초기 체력 설정
            if (uiManager != null)
            {
                uiManager.UpdateHealthSlider(currentHealth);
                uiManager.UpdatePowerSlider(0f); // 초기 파워 게이지 설정
            }
            
            // 가이드 라인 파워 임계치 설정
            if (aimLine != null)
            {
                aimLine.SetPowerThreshold(minPowerThreshold);
            }
        }

        private void Update()
        {
            if (!canControl) return;
            
            HandleMovement();
            HandleAiming();
            HandleShooting();
            
            // 쿨다운 감소
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }
        }

        /// <summary>
        /// WASD 키를 사용하여 플레이어 이동 처리
        /// </summary>
        private void HandleMovement()
        {
            float horizontalInput = Input.GetAxisRaw("Horizontal");
            float verticalInput = Input.GetAxisRaw("Vertical");

            Vector2 movement = new Vector2(horizontalInput, verticalInput).normalized;
            rb.linearVelocity = movement * moveSpeed;
        }
        
        /// <summary>
        /// 마우스 방향으로 플레이어 회전
        /// </summary>
        private void HandleAiming()
        {
            // 차징 중이 아닐 때만 마우스를 따라 회전
            if (!isCharging)
            {
                // 마우스 위치를 월드 좌표로 변환
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePosition.z = 0f;
                
                // 플레이어에서 마우스 방향으로 향하는 벡터
                Vector2 direction = (mousePosition - transform.position).normalized;
                
                // 방향 벡터의 각도 계산
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                
                // 플레이어 회전
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            // 차징 중일 때는 이미 HandleShooting에서 목표 방향으로 회전 처리
        }

        /// <summary>
        /// 마우스 클릭 입력으로 화살 차징 및 발사 처리
        /// </summary>
        private void HandleShooting()
        {
            // 쿨다운 중이면 처리하지 않음
            if (currentCooldown > 0) return;
            
            // 마우스 버튼을 누를 때 목표 위치 설정 및 차징 시작
            if (Input.GetMouseButtonDown(0) && !isCharging)
            {
                // 클릭 위치를 월드 좌표로 변환
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                targetPosition = new Vector2(mousePosition.x, mousePosition.y);
                
                // 처음 클릭한 위치 저장
                initialClickPosition = targetPosition;
                currentMousePosition = targetPosition;
                
                // 목표 마커 활성화
                if (targetMarkerInstance != null)
                {
                    TargetMarker marker = targetMarkerInstance.GetComponent<TargetMarker>();
                    if (marker != null)
                    {
                        marker.ActivateMarker(targetPosition);
                    }
                    else
                    {
                        targetMarkerInstance.SetActive(true);
                        targetMarkerInstance.transform.position = new Vector3(targetPosition.x, targetPosition.y, 0f);
                    }
                }
                
                // 차징 시작
                isCharging = true;
                currentPower = 0f;
            }
            
            // 차징 중이면 파워 증가 및 가이드 라인 업데이트
            if (Input.GetMouseButton(0) && isCharging)
            {
                currentPower += Time.deltaTime / maxChargeTime;
                currentPower = Mathf.Clamp01(currentPower); // 0-1 사이로 제한
                
                // 현재 마우스 위치 업데이트
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentMousePosition = new Vector2(mousePosition.x, mousePosition.y);
                
                // UI 업데이트
                if (uiManager != null)
                {
                    uiManager.UpdatePowerSlider(currentPower);
                }
                
                // 베지어 곡선 중간점 계산
                Vector2 curveControlPoint = CalculateCurveControlPoint();
                
                // 가이드 라인 업데이트 (곡선으로)
                if (aimLine != null)
                {
                    aimLine.ShowCurvedLine(transform.position, targetPosition, curveControlPoint, currentPower);
                }
                
                // 플레이어를 목표 방향으로 회전
                Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            
            // 마우스 버튼을 떼면 차징 종료 및 발사
            if (Input.GetMouseButtonUp(0) && isCharging)
            {
                // 가이드 라인 숨기기
                if (aimLine != null)
                {
                    aimLine.HideLine();
                }
                
                if (currentPower >= minPowerThreshold)
                {
                    // 베지어 곡선 중간점 계산
                    Vector2 curveControlPoint = CalculateCurveControlPoint();
                    
                    // 곡선 화살 발사
                    ShootCurvedArrowToTarget(currentPower, curveControlPoint);
                    currentCooldown = shootCooldown;
                }
                else
                {
                    // 파워가 충분하지 않음 - 발사하지 않고 차징 취소
                    Debug.Log("파워가 부족합니다. 최소 " + (minPowerThreshold * 100) + "% 이상 차징해야 합니다.");
                    
                    // 목표 마커 비활성화
                    if (targetMarkerInstance != null)
                    {
                        TargetMarker marker = targetMarkerInstance.GetComponent<TargetMarker>();
                        if (marker != null)
                        {
                            marker.DeactivateMarker();
                        }
                        else
                        {
                            targetMarkerInstance.SetActive(false);
                        }
                    }
                }
                
                // 차징 상태 리셋
                isCharging = false;
                currentPower = 0f;
                
                // UI 업데이트
                if (uiManager != null)
                {
                    uiManager.UpdatePowerSlider(0f);
                }
            }
        }

        /// <summary>
        /// 베지어 곡선의 제어점을 계산
        /// </summary>
        /// <returns>곡선 제어점 위치</returns>
        private Vector2 CalculateCurveControlPoint()
        {
            // 시작 위치(플레이어)에서 목표까지의 직선 벡터
            Vector2 directVector = targetPosition - (Vector2)transform.position;
            
            // 초기 클릭에서 현재 마우스 위치까지의 오프셋
            Vector2 dragOffset = currentMousePosition - initialClickPosition;
            
            // 드래그 거리 제한
            float dragDistance = dragOffset.magnitude;
            if (dragDistance > maxCurveMagnitude)
            {
                dragOffset = dragOffset.normalized * maxCurveMagnitude;
            }
            
            // 드래그 방향과 크기에 따라 곡선 방향 및 크기 결정
            Vector2 perpendicular = new Vector2(-directVector.y, directVector.x).normalized;
            
            // 드래그 방향과 수직 방향의 내적을 이용해 곡선의 좌/우 방향 결정
            float sideDirection = Vector2.Dot(dragOffset, perpendicular);
            
            // 결정된 방향으로 곡선의 크기 조정
            perpendicular *= sideDirection * curveFactor * dragDistance;
            
            // 직선의 중간 지점 계산
            Vector2 midPoint = (Vector2)transform.position + directVector * 0.5f;
            
            // 중간점에서 수직 방향으로 이동하여 제어점 결정
            Vector2 controlPoint = midPoint + perpendicular;
            
            return controlPoint;
        }

        /// <summary>
        /// 목표 위치로 화살을 발사
        /// </summary>
        /// <param name="power">발사 파워 (0-1)</param>
        private void ShootArrowToTarget(float power)
        {
            if (arrowPrefab != null && firePoint != null)
            {
                // 플레이어와 목표 지점 사이의 거리 계산
                float distanceToTarget = Vector2.Distance((Vector2)firePoint.position, targetPosition);
                
                // 거리에 기반한 속도 조정 (거리가 멀수록 속도를 조금 높임)
                float distanceSpeedFactor = Mathf.Clamp(distanceToTarget / 10f, 0.8f, 1.5f);
                
                // 파워에 따라 화살 속도 증가 (최소 기본속도, 최대 기본속도 * 배율)
                float finalSpeed = arrowSpeed * distanceSpeedFactor * (1f + power * (powerSpeedMultiplier - 1f));
                
                // 발사 방향 (플레이어에서 목표 위치까지)
                Vector2 direction = ((Vector2)targetPosition - (Vector2)firePoint.position).normalized;
                
                // 화살 생성 (회전은 ArrowController에서 처리하므로 identity 사용)
                GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
                
                // Arrow에 TargetInfo 컴포넌트 추가 (목표 위치 정보 저장)
                ArrowTargetInfo targetInfo = arrow.AddComponent<ArrowTargetInfo>();
                targetInfo.targetPosition = targetPosition;
                targetInfo.destroyOnReachTarget = true;
                
                ArrowController arrowController = arrow.GetComponent<ArrowController>();
                
                if (arrowController != null)
                {
                    // 파워에 비례하는 데미지 설정
                    float powerMultiplier = 1f + power;  // 파워에 따라 1.0-2.0배 데미지
                    int damage = Mathf.RoundToInt(arrowController.Damage * powerMultiplier);
                    arrowController.Damage = damage;
                    
                    // 화살 초기화 (목표 위치 정보 전달)
                    arrowController.Initialize(direction, finalSpeed, power, targetPosition);
                    
                    // 목표 위치 정보 전달 (화살이 목표 위치에 도착했는지 확인하기 위해)
                    StartCoroutine(CheckArrowReachedTarget(arrow, targetPosition));
                }
                else
                {
                    Rigidbody2D arrowRb = arrow.GetComponent<Rigidbody2D>();
                    if (arrowRb != null)
                    {
                        arrowRb.linearVelocity = direction * finalSpeed;
                        
                        // 화살이 날아가는 방향으로 회전
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                        arrow.transform.rotation = Quaternion.Euler(0, 0, angle);
                        
                        // 목표 위치 정보 전달
                        StartCoroutine(CheckArrowReachedTarget(arrow, targetPosition));
                    }
                }
                
                // 화살이 5초 내에 목표 위치에 도달하지 못하면 파괴
                Destroy(arrow, 5f);
            }
        }
        
        /// <summary>
        /// 곡선 경로를 따라 화살을 발사
        /// </summary>
        /// <param name="power">발사 파워 (0-1)</param>
        /// <param name="controlPoint">베지어 곡선 제어점</param>
        private void ShootCurvedArrowToTarget(float power, Vector2 controlPoint)
        {
            if (arrowPrefab != null && firePoint != null)
            {
                // 플레이어와 목표 지점 사이의 거리 계산
                float distanceToTarget = Vector2.Distance((Vector2)firePoint.position, targetPosition);
                
                // 거리에 기반한 속도 조정 (거리가 멀수록 속도를 조금 높임)
                float distanceSpeedFactor = Mathf.Clamp(distanceToTarget / 10f, 0.8f, 1.5f);
                
                // 파워에 따라 화살 속도 증가 (최소 기본속도, 최대 기본속도 * 배율)
                float finalSpeed = arrowSpeed * distanceSpeedFactor * (1f + power * (powerSpeedMultiplier - 1f));
                
                // 발사 방향 (플레이어에서 제어점을 향하는 방향)
                Vector2 initialDirection = (controlPoint - (Vector2)firePoint.position).normalized;
                
                // 화살 생성
                GameObject arrow = Instantiate(arrowPrefab, firePoint.position, Quaternion.identity);
                
                // 화살에 곡선 이동 컴포넌트 추가
                ArrowCurvedMovement curvedMovement = arrow.AddComponent<ArrowCurvedMovement>();
                curvedMovement.Initialize(
                    (Vector2)firePoint.position,    // 시작 위치
                    targetPosition,                 // 목표 위치
                    controlPoint,                   // 곡선 제어점
                    finalSpeed,                     // 속도
                    power                           // 파워
                );
                
                // ArrowController 컴포넌트 설정
                ArrowController arrowController = arrow.GetComponent<ArrowController>();
                if (arrowController != null)
                {
                    // 파워에 비례하는 데미지 설정
                    float powerMultiplier = 1f + power;  // 파워에 따라 1.0-2.0배 데미지
                    int damage = Mathf.RoundToInt(arrowController.Damage * powerMultiplier);
                    arrowController.Damage = damage;
                    
                    // 곡선 이동은 ArrowCurvedMovement가 처리하므로 초기 방향만 설정
                    arrowController.Initialize(initialDirection, finalSpeed, power);
                    
                    // Arrow에 TargetInfo 컴포넌트 추가 (목표 위치 정보 저장)
                    ArrowTargetInfo targetInfo = arrow.AddComponent<ArrowTargetInfo>();
                    targetInfo.targetPosition = targetPosition;
                    targetInfo.destroyOnReachTarget = true;
                }
                
                // 목표 위치 정보 전달 (화살이 목표 위치에 도착했는지 확인하기 위해)
                StartCoroutine(CheckArrowReachedTarget(arrow, targetPosition));
                
                // 화살이 5초 내에 목표 위치에 도달하지 못하면 파괴
                Destroy(arrow, 5f);
            }
        }

      /// <summary>
        /// 화살이 목표 위치에 도달했는지 확인하는 코루틴
        /// </summary>
        /// <param name="arrow">화살 게임 오브젝트</param>
        /// <param name="target">목표 위치</param>
        private IEnumerator CheckArrowReachedTarget(GameObject arrow, Vector2 target)
        {
            float checkDistance = 0.5f; // 목표 위치에 얼마나 가까워야 도달한 것으로 판단할지
            bool targetReached = false;
            
            while (arrow != null && !targetReached)
            {
                // 화살과 목표 위치 사이의 거리 확인
                float distance = Vector2.Distance(arrow.transform.position, target);
                
                // 화살이 목표 위치에 충분히 가까워지면
                if (distance <= checkDistance)
                {
                    targetReached = true;
                    
                    // 목표 마커 비활성화
                    if (targetMarkerInstance != null)
                    {
                        TargetMarker marker = targetMarkerInstance.GetComponent<TargetMarker>();
                        if (marker != null)
                        {
                            marker.DeactivateMarker();
                        }
                        else
                        {
                            targetMarkerInstance.SetActive(false);
                        }
                    }
                    
                    // 화살 컨트롤러에 목적지 도달 알림
                    ArrowController arrowController = arrow.GetComponent<ArrowController>();
                    if (arrowController != null)
                    {
                        arrowController.OnReachDestination();
                    }
                    else
                    {
                        // ArrowController가 없거나 옵션이 설정되지 않은 경우 기존 방식대로 처리
                        ArrowTargetInfo targetInfo = arrow.GetComponent<ArrowTargetInfo>();
                        if (targetInfo != null && targetInfo.destroyOnReachTarget)
                        {
                            Destroy(arrow);
                        }
                    }
                    
                    break;
                }
                
                // 이전 프레임과 현재 프레임의 위치 차이를 계산해서 방향 전환 감지
                // 방향이 반대로 바뀌면 목표 지점을 지나친 것으로 간주
                if (arrow.GetComponent<Rigidbody2D>() != null)
                {
                    Vector2 currentPos = arrow.transform.position;
                    yield return new WaitForFixedUpdate();
                    
                    if (arrow == null) break;
                    
                    Vector2 newPos = arrow.transform.position;
                    Vector2 moveDir = (newPos - currentPos).normalized;
                    Vector2 targetDir = (target - currentPos).normalized;
                    
                    // 두 방향의 내적이 0보다 작으면 방향이 90도 이상 차이가 나는 것
                    // 즉, 화살이 목표 지점을 지나쳤다는 의미
                    float dotProduct = Vector2.Dot(moveDir, targetDir);
                    if (dotProduct < 0 && Vector2.Distance(currentPos, target) < 1.5f)
                    {
                        targetReached = true;
                        
                        // 목표 마커 비활성화
                        if (targetMarkerInstance != null)
                        {
                            TargetMarker marker = targetMarkerInstance.GetComponent<TargetMarker>();
                            if (marker != null)
                            {
                                marker.DeactivateMarker();
                            }
                            else
                            {
                                targetMarkerInstance.SetActive(false);
                            }
                        }
                        
                        // 화살 컨트롤러에 목적지 도달 알림
                        ArrowController arrowController = arrow.GetComponent<ArrowController>();
                        if (arrowController != null)
                        {
                            arrowController.OnReachDestination();
                        }
                        else
                        {
                            // 목표 지점을 지나친 경우에도 파괴 옵션 있으면 파괴
                            ArrowTargetInfo targetInfo = arrow.GetComponent<ArrowTargetInfo>();
                            if (targetInfo != null && targetInfo.destroyOnReachTarget)
                            {
                                Destroy(arrow);
                            }
                        }
                        
                        break;
                    }
                }
                else
                {
                    yield return null;
                }
            }
        }

        /// <summary>
        /// 플레이어가 데미지를 받는 메서드
        /// </summary>
        /// <param name="damage">받는 데미지 양</param>
        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            
            // UI 매니저를 체력 슬라이더 업데이트
            if (uiManager != null)
            {
                uiManager.UpdateHealthSlider(currentHealth);
            }
            
            // 체력이 0 이하면 사망 처리
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        /// <summary>
        /// 플레이어 사망 처리
        /// </summary>
        private void Die()
        {
            canControl = false;
            rb.linearVelocity = Vector2.zero;
            
            // 게임 매니저에 사망 알림
            if (gameManager != null)
            {
                gameManager.PlayerDied();
            }
        }
    }
}