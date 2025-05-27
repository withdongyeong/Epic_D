namespace Game4.Scripts.VFX
{
#pragma warning disable CS0618, CS0612, CS0672
using UnityEngine;
using System.Collections;
#pragma warning restore CS0618, CS0612, CS0672

public class HitEffect : MonoBehaviour
{
    /// <summary>
    /// 피격 효과 지속 시간
    /// </summary>
    private float hitEffectDuration = 0.3f;
    
    /// <summary>
    /// 피격 시 색상
    /// </summary>
    private Color hitColor = Color.red;
    
    /// <summary>
    /// 크기 변화 정도 (0.8 = 20% 축소)
    /// </summary>
    private float scaleMultiplier = 0.8f;
    
    /// <summary>
    /// 스프라이트 렌더러
    /// </summary>
    private SpriteRenderer spriteRenderer;
    
    /// <summary>
    /// 원본 색상
    /// </summary>
    private Color originalColor;
    
    /// <summary>
    /// 원본 크기
    /// </summary>
    private Vector3 originalScale;
    
    /// <summary>
    /// 현재 효과 진행 중인지
    /// </summary>
    private bool isEffectPlaying = false;

    // Properties
    /// <summary>
    /// 피격 효과 지속 시간 프로퍼티
    /// </summary>
    public float HitEffectDuration { get => hitEffectDuration; set => hitEffectDuration = value; }
    
    /// <summary>
    /// 피격 시 색상 프로퍼티
    /// </summary>
    public Color HitColor { get => hitColor; set => hitColor = value; }
    
    /// <summary>
    /// 크기 변화 정도 프로퍼티
    /// </summary>
    public float ScaleMultiplier { get => scaleMultiplier; set => scaleMultiplier = value; }
    
    /// <summary>
    /// 효과 진행 중인지 프로퍼티
    /// </summary>
    public bool IsEffectPlaying { get => isEffectPlaying; private set => isEffectPlaying = value; }

    /// <summary>
    /// 초기화
    /// </summary>
    private void Awake()
    {
        // SpriteRenderer 찾기 (자식 오브젝트에서)
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            // 원본 색상과 크기 저장
            originalColor = spriteRenderer.color;
            originalScale = spriteRenderer.transform.localScale;
        }
        else
        {
            Debug.LogWarning($"SpriteRenderer not found in children of {gameObject.name}");
        }
    }

    /// <summary>
    /// 피격 효과 재생
    /// </summary>
    public void PlayHitEffect()
    {
        if (spriteRenderer == null || isEffectPlaying) return;
        
        StartCoroutine(HitEffectCoroutine());
    }

    /// <summary>
    /// 피격 효과 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator HitEffectCoroutine()
    {
        isEffectPlaying = true;
        
        float halfDuration = hitEffectDuration * 0.5f;
        
        // 1단계: 빨간색으로 변하고 크기 축소
        float elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            float progress = elapsedTime / halfDuration;
            
            // 색상 변화
            Color currentColor = Color.Lerp(originalColor, hitColor, progress);
            spriteRenderer.color = currentColor;
            
            // 크기 변화
            Vector3 currentScale = Vector3.Lerp(originalScale, originalScale * scaleMultiplier, progress);
            spriteRenderer.transform.localScale = currentScale;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 2단계: 원래 색상과 크기로 복구
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            float progress = elapsedTime / halfDuration;
            
            // 색상 복구
            Color currentColor = Color.Lerp(hitColor, originalColor, progress);
            spriteRenderer.color = currentColor;
            
            // 크기 복구
            Vector3 currentScale = Vector3.Lerp(originalScale * scaleMultiplier, originalScale, progress);
            spriteRenderer.transform.localScale = currentScale;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // 완전히 원래 상태로 복구
        spriteRenderer.color = originalColor;
        spriteRenderer.transform.localScale = originalScale;
        
        isEffectPlaying = false;
    }

    /// <summary>
    /// 즉시 원래 상태로 복구
    /// </summary>
    public void ResetToOriginal()
    {
        if (spriteRenderer == null) return;
        
        StopAllCoroutines();
        spriteRenderer.color = originalColor;
        spriteRenderer.transform.localScale = originalScale;
        isEffectPlaying = false;
    }

    /// <summary>
    /// 원본 색상 업데이트 (색상이 바뀐 경우)
    /// </summary>
    public void UpdateOriginalColor()
    {
        if (spriteRenderer != null && !isEffectPlaying)
        {
            originalColor = spriteRenderer.color;
        }
    }

    /// <summary>
    /// 원본 크기 업데이트 (크기가 바뀐 경우)
    /// </summary>
    public void UpdateOriginalScale()
    {
        if (spriteRenderer != null && !isEffectPlaying)
        {
            originalScale = spriteRenderer.transform.localScale;
        }
    }
}
}