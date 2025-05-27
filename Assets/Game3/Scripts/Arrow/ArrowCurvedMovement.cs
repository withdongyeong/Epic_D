using UnityEngine;

namespace Game3.Scripts.Arrow
{
    public class ArrowCurvedMovement : MonoBehaviour
    {
        /// <summary>
        /// 시작 위치
        /// </summary>
        private Vector2 startPoint;
        
        /// <summary>
        /// 목표 위치
        /// </summary>
        private Vector2 endPoint;
        
        /// <summary>
        /// 제어점 (곡선 결정)
        /// </summary>
        private Vector2 controlPoint;
        
        /// <summary>
        /// 화살 이동 속도
        /// </summary>
        private float speed = 10f;
        
        /// <summary>
        /// 현재 곡선 진행도 (0-1)
        /// </summary>
        private float t = 0f;
        
        /// <summary>
        /// 화살 파워 (0-1)
        /// </summary>
        private float power = 1f;
        
        /// <summary>
        /// 목표에 도달했는지 여부
        /// </summary>
        private bool reachedTarget = false;
        
        /// <summary>
        /// 리지드바디 컴포넌트
        /// </summary>
        private Rigidbody2D rb;
        
        /// <summary>
        /// 곡선 길이 근사치
        /// </summary>
        private float approximateCurveLength;
        
        /// <summary>
        /// 곡선 세분화 정도
        /// </summary>
        private int curveResolution = 50;
        
        /// <summary>
        /// 곡선상의 위치를 미리 계산하여 저장
        /// </summary>
        private Vector2[] curvePoints;
        
        /// <summary>
        /// 곡선상의 각 위치에서의 접선 벡터
        /// </summary>
        private Vector2[] curveTangents;
        
        /// <summary>
        /// 리지드바디 사용 여부 (false일 경우 직접 위치 제어)
        /// </summary>
        private bool useRigidbody = false;
        
        /// <summary>
        /// 콜라이더 컴포넌트
        /// </summary>
        private Collider2D col;
        
        /// <summary>
        /// 디버그 모드 - 곡선 경로 시각화
        /// </summary>
        public bool debugMode = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
            
            // 물리 설정
            if (rb != null)
            {
                // 중력 영향 없애기
                rb.gravityScale = 0f;
                
                if (!useRigidbody)
                {
                    // 리지드바디를 이용한 물리 시뮬레이션 비활성화
                    rb.isKinematic = true;
                }
            }
        }

        /// <summary>
        /// 곡선 이동 초기화
        /// </summary>
        /// <param name="start">시작 위치</param>
        /// <param name="end">목표 위치</param>
        /// <param name="control">제어점</param>
        /// <param name="moveSpeed">이동 속도</param>
        /// <param name="arrowPower">화살 파워</param>
        public void Initialize(Vector2 start, Vector2 end, Vector2 control, float moveSpeed, float arrowPower)
        {
            startPoint = start;
            endPoint = end;
            controlPoint = control;
            speed = moveSpeed;
            power = arrowPower;
            t = 0f;
            
            // 곡선 길이 근사치 계산 (정확한 계산은 복잡하므로 간단히 구현)
            float straightDistance = Vector2.Distance(start, end);
            float controlDistance = Vector2.Distance(start, control) + Vector2.Distance(control, end);
            approximateCurveLength = (straightDistance + controlDistance) * 0.5f;
            
            // 초기 위치 설정
            transform.position = new Vector3(start.x, start.y, transform.position.z);
            
            // 곡선 경로와 접선 미리 계산
            CalculateCurvePathAndTangents();
            
            // 초기 방향 설정 - 중요! 첫 접선 방향으로 화살 회전
            Vector2 firstTangent = curveTangents[0];
            float angle = Mathf.Atan2(firstTangent.y, firstTangent.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            // 디버그 모드일 때 곡선 시각화
            if (debugMode)
            {
                VisualizePathInDebug();
            }
        }
        
        /// <summary>
        /// 전체 곡선 경로와 각 지점에서의 접선 벡터를 미리 계산
        /// </summary>
        private void CalculateCurvePathAndTangents()
        {
            curvePoints = new Vector2[curveResolution];
            curveTangents = new Vector2[curveResolution];
            
            for (int i = 0; i < curveResolution; i++)
            {
                float time = i / (float)(curveResolution - 1);
                
                // 곡선 상의 위치 계산
                curvePoints[i] = CalculateQuadraticBezierPoint(time, startPoint, controlPoint, endPoint);
                
                // 현재 위치에서의 접선 벡터 계산 (미분으로 구함)
                curveTangents[i] = CalculateQuadraticBezierTangent(time, startPoint, controlPoint, endPoint);
            }
        }
        
        /// <summary>
        /// 디버그 모드에서 경로 시각화
        /// </summary>
        private void VisualizePathInDebug()
        {
            for (int i = 0; i < curveResolution - 1; i++)
            {
                Debug.DrawLine(curvePoints[i], curvePoints[i + 1], Color.yellow, 5f);
                
                // 접선 방향 시각화 (짧은 선으로)
                Debug.DrawRay(curvePoints[i], curveTangents[i] * 0.5f, Color.red, 5f);
            }
        }
        
        private void Update()
        {
            if (reachedTarget) return;
            
            // t 값 증가 (시간에 따라 일정 속도로)
            float tSpeed = speed / approximateCurveLength;
            t += tSpeed * Time.deltaTime;
            
            // 목표 도달 체크
            if (t >= 1f)
            {
                t = 1f;
                reachedTarget = true;
                
                // 목표 지점에 정확히 위치시키기
                Vector3 finalPosition = new Vector3(endPoint.x, endPoint.y, transform.position.z);
                transform.position = finalPosition;
                
                // ArrowTargetInfo 컴포넌트가 있는지 확인
                ArrowTargetInfo targetInfo = GetComponent<ArrowTargetInfo>();
                if (targetInfo != null && targetInfo.destroyOnReachTarget)
                {
                    Destroy(gameObject);
                }
                
                return;
            }
            
            // 현재 t값에 따른 위치와 회전 업데이트
            UpdateArrowPositionAndRotation();
        }
        
        /// <summary>
        /// 화살의 위치와 회전을 동시에 업데이트
        /// </summary>
        private void UpdateArrowPositionAndRotation()
        {
            // t 값을 인덱스로 변환
            int index = Mathf.FloorToInt(t * (curveResolution - 1));
            index = Mathf.Clamp(index, 0, curveResolution - 2);
            
            // 현재 인덱스와 다음 인덱스 사이의 보간값
            float localT = (t * (curveResolution - 1)) - index;
            
            // 두 점 사이를 선형 보간하여 새 위치 계산
            Vector2 newPosition = Vector2.Lerp(curvePoints[index], curvePoints[index + 1], localT);
            
            // 두 접선 사이를 선형 보간하여 현재 접선 계산
            Vector2 tangent = Vector2.Lerp(curveTangents[index], curveTangents[index + 1], localT).normalized;
            
            // 위치 업데이트
            transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
            
            // 접선 방향으로 화살 회전 (즉시 회전 - 부드러운 보간 없음)
            if (tangent != Vector2.zero)
            {
                float angle = Mathf.Atan2(tangent.y, tangent.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            
            // 디버그 시 현재 위치와 접선 표시
            if (debugMode)
            {
                Debug.DrawRay(newPosition, tangent * 0.5f, Color.green, 0.1f);
            }
        }
        
        /// <summary>
        /// 이차 베지어 곡선 위의 점 계산
        /// </summary>
        private Vector2 CalculateQuadraticBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            
            Vector2 p = uu * p0; // (1-t)^2 * P0
            p += 2 * u * t * p1; // 2 * (1-t) * t * P1
            p += tt * p2; // t^2 * P2
            
            return p;
        }
        
        /// <summary>
        /// 이차 베지어 곡선의 접선 벡터 계산 (미분 이용)
        /// </summary>
        private Vector2 CalculateQuadraticBezierTangent(float t, Vector2 p0, Vector2 p1, Vector2 p2)
        {
            // B'(t) = 2(1-t)(P1-P0) + 2t(P2-P1)
            Vector2 tangent = 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
            return tangent.normalized;
        }
        
        /// <summary>
        /// 곡선 이동 즉시 중지
        /// </summary>
        public void StopMovement()
        {
            reachedTarget = true;
            
            if (rb != null && !rb.isKinematic)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}