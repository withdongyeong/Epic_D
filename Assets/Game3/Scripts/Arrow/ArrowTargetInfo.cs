using UnityEngine;

namespace Game3.Scripts.Arrow
{
    /// <summary>
    /// 화살의 목표 위치 정보를 저장하는 컴포넌트
    /// </summary>
    public class ArrowTargetInfo : MonoBehaviour
    {
        /// <summary>
        /// 목표 위치
        /// </summary>
        public Vector2 targetPosition;
        
        /// <summary>
        /// 목표에 도달했을 때 화살을 파괴할지 여부
        /// </summary>
        public bool destroyOnReachTarget = true;
        
        /// <summary>
        /// 목표 도달 후 몬스터 해제까지의 대기 시간
        /// </summary>
        public float releaseEnemyDelay = 1.5f;
        
        /// <summary>
        /// 목표에 도달했는지 여부
        /// </summary>
        [HideInInspector]
        public bool hasReachedTarget = false;
    }
}