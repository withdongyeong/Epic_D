using UnityEngine;

namespace Game2.Scripts.Character.Enemy
{
    /// <summary>
    /// 슬라임 몬스터 클래스
    /// </summary>
    public class SlimeMonster : Monster
    {
        [SerializeField] private Transform _target;

        /// <summary>
        /// 점프 높이
        /// </summary>
        public float JumpHeight { get; private set; } = 2f;

        /// <summary>
        /// 점프 공격 사용 여부
        /// </summary>
        public bool UseJumpAttack { get; private set; } = true;

        /// <summary>
        /// 슬라임 색상
        /// </summary>
        public string SlimeColor { get; private set; } = "초록색";

        private void Awake()
        {
            // 슬라임 특성 초기화
            InitializeTraits();
        }

        /// <summary>
        /// 슬라임 특성 초기화
        /// </summary>
        private void InitializeTraits()
        {
            // 팀 특성
            _traits["team"] = new MonsterTrait(
                "난 마왕 세력이다",
                "난 용사 세력이다",
                () => { ChangeTeam(TeamType == Team.Enemy ? Team.Ally : Team.Enemy); }
            );

            // 이동 속도 특성
            _traits["speed"] = new MonsterTrait(
                "난 빠르게 움직인다",
                "난 느리게 움직인다",
                () => { ChangeMoveSpeed(MoveSpeed > 1.5f ? 0.5f : 4f); }
            );

            // 점프 공격 특성
            _traits["jump"] = new MonsterTrait(
                "난 점프해서 공격한다",
                "난 점프로 공격하지 않는다",
                () => { UseJumpAttack = !UseJumpAttack; }
            );

            // 색상 특성
            _traits["color"] = new MonsterTrait(
                $"난 {SlimeColor} 슬라임이다",
                $"난 {(SlimeColor == "초록색" ? "파란색" : "초록색")} 슬라임이다",
                () => { ChangeColor(SlimeColor == "초록색" ? "파란색" : "초록색"); }
            );
        }

        private void Update()
        {
            if (_target != null)
            {
                // 타겟을 향해 이동
                if (TeamType == Team.Enemy)
                {
                    MoveToTarget();
                }
                else if (TeamType == Team.Ally)
                {
                    FollowPlayer();
                }
            }
        }

        /// <summary>
        /// 타겟을 향해 이동
        /// </summary>
        private void MoveToTarget()
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            transform.position += direction * MoveSpeed * Time.deltaTime;
        }

        /// <summary>
        /// 플레이어를 따라다님 (아군일 때)
        /// </summary>
        private void FollowPlayer()
        {
            float distance = Vector3.Distance(transform.position, _target.position);

            if (distance > 3f)
            {
                Vector3 direction = (_target.position - transform.position).normalized;
                transform.position += direction * MoveSpeed * 0.7f * Time.deltaTime;
            }
        }

        /// <summary>
        /// 점프 공격 설정 함수
        /// </summary>
        public void SetJumpAttack(bool useJump)
        {
            UseJumpAttack = useJump;
        }

        /// <summary>
        /// 색상 변경 함수
        /// </summary>
        public void ChangeColor(string color)
        {
            SlimeColor = color;

            if (color == "파란색")
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.green;
            }
        }
    }
}