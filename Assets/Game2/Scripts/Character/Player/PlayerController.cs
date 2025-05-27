using UnityEngine;

namespace Game2.Scripts.Character.Player
{
    /// <summary>
    /// 플레이어 캐릭터의 움직임을 제어하는 클래스
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float _moveSpeed = 5f;
        [SerializeField] private Animator _animator;

        private Vector3 _facingDirection = Vector3.right;

        /// <summary>
        /// 플레이어가 바라보는 방향 반환
        /// </summary>
        public Vector3 GetFacingDirection()
        {
            return _facingDirection.normalized;
        }

        private void Update()
        {
            // 이동 입력 감지
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(horizontal, vertical, 0).normalized;

            // 이동 처리
            transform.position += movement * _moveSpeed * Time.deltaTime;

            // 바라보는 방향 업데이트
            if (movement.magnitude > 0.1f)
            {
                _facingDirection = movement;
                UpdateFacingDirection();
            }

            // 애니메이션 파라미터 업데이트
            if (_animator != null)
            {
                _animator.SetFloat("Speed", movement.magnitude);
            }
        }

        /// <summary>
        /// 캐릭터가 바라보는 방향에 따라 스프라이트 방향 조정
        /// </summary>
        private void UpdateFacingDirection()
        {
            // 스프라이트 뒤집기 처리
            if (_facingDirection.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (_facingDirection.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }
}