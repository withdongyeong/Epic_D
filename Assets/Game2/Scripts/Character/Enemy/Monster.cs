using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game2.Scripts.Character.Enemy
{
    /// <summary>
    /// 몬스터의 기본 특성을 정의하는 클래스
    /// </summary>
    public abstract class Monster : MonoBehaviour
    {
        // Monster.cs 파일에 추가

        /// <summary>
        /// 몬스터 특성 클래스
        /// </summary>
        [System.Serializable]
        public class MonsterTrait
        {
            public string currentText; // 현재 텍스트
            public string oppositeText; // 반대 텍스트
            public System.Action onSwap; // 스왑 시 호출될 액션

            public MonsterTrait(string current, string opposite, System.Action swap)
            {
                currentText = current;
                oppositeText = opposite;
                onSwap = swap;
            }

            /// <summary>
            /// 특성 스왑 함수
            /// </summary>
            public void Swap()
            {
                // 텍스트 교체
                string temp = currentText;
                currentText = oppositeText;
                oppositeText = temp;

                // 실제 효과 적용
                onSwap?.Invoke();
            }
        }

        protected Dictionary<string, MonsterTrait> _traits = new Dictionary<string, MonsterTrait>();

        /// <summary>
        /// 몬스터의 특성 목록을 반환
        /// </summary>
        public Dictionary<string, MonsterTrait> GetTraits()
        {
            return _traits;
        }

        // private 변수들 (직접 접근 불가)
        private float _health = 100f;
        private float _damage = 10f;
        private float _attackCooldown = 1.5f;

        // public 프로퍼티 (접근 가능)
        /// <summary>
        /// 현재 팀 타입
        /// </summary>
        public Team TeamType { get; private set; } = Team.Enemy;

        /// <summary>
        /// 현재 이동 속도
        /// </summary>
        public float MoveSpeed { get; private set; } = 3f;

        /// <summary>
        /// 몬스터 팀 타입 열거형
        /// </summary>
        public enum Team
        {
            Enemy, // 적
            Ally, // 아군
            Neutral // 중립
        }

        /// <summary>
        /// 대상 변경 함수
        /// </summary>
        public void ChangeTeam(Team newTeam)
        {
            TeamType = newTeam;

            // 팀 변경 시 시각적 효과
            if (newTeam == Team.Enemy)
            {
                GetComponent<SpriteRenderer>().color = Color.red;
            }
            else if (newTeam == Team.Ally)
            {
                GetComponent<SpriteRenderer>().color = Color.blue;
            }
        }

        /// <summary>
        /// 이동 속도 변경 함수
        /// </summary>
        public void ChangeMoveSpeed(float newSpeed)
        {
            MoveSpeed = newSpeed;
        }
    }
}