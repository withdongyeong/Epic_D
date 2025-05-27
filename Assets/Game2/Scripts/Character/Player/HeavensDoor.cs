using System.Collections;
using System.Collections.Generic;
using Game2.Scripts.Character.Enemy;
using UnityEngine;

namespace Game2.Scripts.Character.Player
{
    /// <summary>
    /// 플레이어의 헤븐즈 도어 능력
    /// </summary>
    public class HeavensDoor : MonoBehaviour
    {
        [Header("대시 설정")] [SerializeField] private float _dashSpeed = 15f;
        [SerializeField] private float _dashDuration = 0.3f;
        [SerializeField] private float _dashCooldown = 1.5f;

        private bool _canDash = true;
        private PlayerController _player;
        private Monster _currentTarget;
        [SerializeField] private BookUI _bookUI;


        private void Start()
        {
            _player = GetComponent<PlayerController>();

            if (_bookUI == null)
            {
                _bookUI = FindObjectOfType<BookUI>();
            }
        }

        private void Update()
        {
            // 대시 공격 입력 감지
            if (Input.GetKeyDown(KeyCode.LeftShift) && _canDash)
            {
                StartCoroutine(DashAttack());
            }
        }

        /// <summary>
        /// 대시 공격 코루틴
        /// </summary>
        private IEnumerator DashAttack()
        {
            _canDash = false;

            // 플레이어 바라보는 방향으로 대시
            Vector3 dashDirection = _player.GetFacingDirection();
            float startTime = Time.time;

            while (Time.time < startTime + _dashDuration)
            {
                // 대시 이동
                transform.position += dashDirection * _dashSpeed * Time.deltaTime;

                // 대시 중 충돌 체크
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1.0f);
                foreach (Collider2D col in colliders)
                {
                    Monster monster = col.GetComponent<Monster>();
                    if (monster != null)
                    {
                        // 몬스터에 닿았을 때 처리
                        OpenMonsterBook(monster);
                        break; // 한 번에 하나의 몬스터만 처리
                    }
                }

                yield return null;
            }

            // 쿨다운
            yield return new WaitForSeconds(_dashCooldown);
            _canDash = true;
        }

        /// <summary>
        /// 몬스터 책 열기
        /// </summary>
        private void OpenMonsterBook(Monster monster)
        {
            _currentTarget = monster;

            // 슬로우 모션 효과
            Time.timeScale = 0.3f;

            // 책 UI 표시
            if (_bookUI != null)
            {
                _bookUI.ShowBook(monster);
            }
            else
            {
                Debug.LogError("BookUI 참조가 없습니다.");
            }
        }

        /// <summary>
        /// 책이 닫혔을 때 호출되는 함수
        /// </summary>
        public void OnBookClosed()
        {
            Time.timeScale = 1.0f;
            _currentTarget = null;
        }

        /// <summary>
        /// 책 UI 닫기
        /// </summary>
        public void CloseBook()
        {
            Time.timeScale = 1.0f;
            _currentTarget = null;
        }

    }
}