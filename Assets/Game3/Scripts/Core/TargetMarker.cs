using UnityEngine;
using System.Collections;

namespace Game3.Scripts.Core
{
    public class TargetMarker : MonoBehaviour
    {
        /// <summary>
        /// 마커 스프라이트 렌더러
        /// </summary>
        private SpriteRenderer spriteRenderer;
        
        /// <summary>
        /// 마커 페이드 아웃 지속 시간
        /// </summary>
        private float fadeOutDuration = 0.3f;
        
        /// <summary>
        /// 마커 스케일링 효과 지속 시간
        /// </summary>
        private float scaleEffectDuration = 0.5f;
        
        /// <summary>
        /// 초기 스케일
        /// </summary>
        private Vector3 initialScale;
        
        /// <summary>
        /// 목표 스케일
        /// </summary>
        private Vector3 targetScale;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
            }
            
            // 초기 설정
            initialScale = transform.localScale;
            targetScale = initialScale * 0.6f; // 목표 크기는 초기 크기의 60%
        }
        
        /// <summary>
        /// 마커 활성화 및 초기화
        /// </summary>
        /// <param name="position">위치</param>
        public void ActivateMarker(Vector2 position)
        {
            gameObject.SetActive(true);
            transform.position = new Vector3(position.x, position.y, 0f);
            
            // 시각 효과 초기화
            if (spriteRenderer != null)
            {
                Color color = spriteRenderer.color;
                color.a = 1f;
                spriteRenderer.color = color;
            }
            
            transform.localScale = initialScale;
            
            // 스케일 효과 시작
            StartCoroutine(ScaleEffect());
        }
        
        /// <summary>
        /// 마커 페이드 아웃 및 비활성화
        /// </summary>
        public void DeactivateMarker()
        {
            StartCoroutine(FadeOut());
        }
        
        /// <summary>
        /// 페이드 아웃 코루틴
        /// </summary>
        private IEnumerator FadeOut()
        {
            float elapsedTime = 0f;
            
            // 초기 알파값 저장
            float startAlpha = spriteRenderer.color.a;
            
            // 지정된 시간 동안 페이드 아웃
            while (elapsedTime < fadeOutDuration)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = elapsedTime / fadeOutDuration;
                
                Color color = spriteRenderer.color;
                color.a = Mathf.Lerp(startAlpha, 0f, normalizedTime);
                spriteRenderer.color = color;
                
                yield return null;
            }
            
            // 완전히 페이드 아웃되면 비활성화
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 마커 크기 변화 효과 코루틴
        /// </summary>
        private IEnumerator ScaleEffect()
        {
            float elapsedTime = 0f;
            
            // 지정된 시간 동안 크기 축소
            while (elapsedTime < scaleEffectDuration)
            {
                elapsedTime += Time.deltaTime;
                float normalizedTime = elapsedTime / scaleEffectDuration;
                
                // 이지 아웃 효과
                float t = 1f - Mathf.Pow(1f - normalizedTime, 2f);
                
                transform.localScale = Vector3.Lerp(initialScale, targetScale, t);
                
                yield return null;
            }
            
            // 최종 크기 설정
            transform.localScale = targetScale;
        }
    }
}