namespace Game4.Scripts.Camera
{
#pragma warning disable CS0618, CS0612, CS0672
    using UnityEngine;
#pragma warning restore CS0618, CS0612, CS0672

    public class CameraFollow : MonoBehaviour
    {
        /// <summary>
        /// 따라다닐 플레이어 타겟
        /// </summary>
        private Transform target;
    
        /// <summary>
        /// 카메라 부드러운 이동 속도
        /// </summary>
        private float smoothSpeed = 0.125f;
    
        /// <summary>
        /// 카메라 오프셋 (플레이어로부터의 거리)
        /// </summary>
        private Vector3 offset = new Vector3(0, 0, -10);

        // Properties
        /// <summary>
        /// 타겟 프로퍼티
        /// </summary>
        public Transform Target { get => target; set => target = value; }
    
        /// <summary>
        /// 부드러운 이동 속도 프로퍼티
        /// </summary>
        public float SmoothSpeed { get => smoothSpeed; set => smoothSpeed = value; }
    
        /// <summary>
        /// 오프셋 프로퍼티
        /// </summary>
        public Vector3 Offset { get => offset; set => offset = value; }

        /// <summary>
        /// 초기화
        /// </summary>
        private void Start()
        {
            // 플레이어 자동 찾기
            if (target == null)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");
                if (player != null)
                {
                    target = player.transform;
                }
            }
        }

        /// <summary>
        /// 카메라 이동 (LateUpdate에서 처리)
        /// </summary>
        private void LateUpdate()
        {
            if (target == null) return;
        
            // 목표 위치 계산
            Vector3 desiredPosition = target.position + offset;
        
            // 부드럽게 이동
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
            transform.position = smoothedPosition;
        }
    }
}