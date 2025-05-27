using UnityEngine;
using System.Collections.Generic;

namespace Game3.Scripts.Core
{
    public class AimLine : MonoBehaviour
    {
        /// <summary>
        /// 라인 렌더러 컴포넌트
        /// </summary>
        private LineRenderer lineRenderer;
        
        /// <summary>
        /// 라인 색상
        /// </summary>
        [SerializeField] private Color lineColor = new Color(1f, 1f, 1f, 0.5f);
        
        /// <summary>
        /// 라인 두께
        /// </summary>
        [SerializeField] private float lineWidth = 0.1f;
        
        /// <summary>
        /// 최대 파워에서의 라인 색상
        /// </summary>
        [SerializeField] private Color maxPowerColor = Color.red;
        
        /// <summary>
        /// 파워 임계치에서의 라인 색상
        /// </summary>
        [SerializeField] private Color thresholdPowerColor = Color.yellow;
        
        /// <summary>
        /// 최소 파워 임계치
        /// </summary>
        private float minPowerThreshold = 0.5f;
        
        /// <summary>
        /// 곡선 세분화 정도 (점 개수)
        /// </summary>
        private int curveResolution = 20;

        private void Awake()
        {
            lineRenderer = GetComponent<LineRenderer>();
            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
            }
            
            // 라인 렌더러 초기 설정
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth * 0.7f; // 끝으로 갈수록 조금 가늘어지게
            lineRenderer.startColor = lineColor;
            lineRenderer.endColor = lineColor;
            lineRenderer.positionCount = 2;  // 시작점과 끝점
            lineRenderer.useWorldSpace = true;
            
            // 기본적으로 비활성화
            lineRenderer.enabled = false;
        }
        
        /// <summary>
        /// 직선 가이드 라인 표시
        /// </summary>
        /// <param name="startPosition">시작 위치</param>
        /// <param name="endPosition">목표 위치</param>
        /// <param name="power">현재 파워 (0-1)</param>
        public void ShowLine(Vector2 startPosition, Vector2 endPosition, float power)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = 2;
            
            // 라인 위치 설정
            lineRenderer.SetPosition(0, startPosition);
            lineRenderer.SetPosition(1, endPosition);
            
            // 파워에 따라 색상 변경
            UpdateLineColor(power);
        }
        
        /// <summary>
        /// 곡선 가이드 라인 표시
        /// </summary>
        /// <param name="startPosition">시작 위치</param>
        /// <param name="endPosition">목표 위치</param>
        /// <param name="controlPoint">제어점 위치</param>
        /// <param name="power">현재 파워 (0-1)</param>
        public void ShowCurvedLine(Vector2 startPosition, Vector2 endPosition, Vector2 controlPoint, float power)
        {
            lineRenderer.enabled = true;
            lineRenderer.positionCount = curveResolution;
            
            // 베지어 곡선 점들 계산
            for (int i = 0; i < curveResolution; i++)
            {
                float t = i / (float)(curveResolution - 1);
                Vector2 point = CalculateQuadraticBezierPoint(t, startPosition, controlPoint, endPosition);
                lineRenderer.SetPosition(i, point);
            }
            
            // 파워에 따라 색상 변경
            UpdateLineColor(power);
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
        /// 가이드 라인 숨기기
        /// </summary>
        public void HideLine()
        {
            lineRenderer.enabled = false;
        }
        
        /// <summary>
        /// 파워에 따라 라인 색상 업데이트
        /// </summary>
        /// <param name="power">현재 파워 (0-1)</param>
        private void UpdateLineColor(float power)
        {
            Color currentColor;
            
            if (power < minPowerThreshold)
            {
                // 파워가 임계치 미만이면 기본 색상에서 임계치 색상으로 보간
                float t = power / minPowerThreshold;
                currentColor = Color.Lerp(lineColor, thresholdPowerColor, t);
            }
            else
            {
                // 파워가 임계치 이상이면 임계치 색상에서 최대 파워 색상으로 보간
                float t = (power - minPowerThreshold) / (1f - minPowerThreshold);
                currentColor = Color.Lerp(thresholdPowerColor, maxPowerColor, t);
            }
            
            lineRenderer.startColor = currentColor;
            
            // 끝쪽은 조금 더 투명하게
            Color endColor = currentColor;
            endColor.a *= 0.7f;
            lineRenderer.endColor = endColor;
        }
        
        /// <summary>
        /// 최소 파워 임계치 설정
        /// </summary>
        /// <param name="threshold">임계치 (0-1)</param>
        public void SetPowerThreshold(float threshold)
        {
            minPowerThreshold = threshold;
        }
        
        /// <summary>
        /// 곡선 해상도 설정
        /// </summary>
        /// <param name="resolution">곡선을 구성하는 점의 수</param>
        public void SetCurveResolution(int resolution)
        {
            if (resolution >= 2)
            {
                curveResolution = resolution;
            }
        }
    }
}