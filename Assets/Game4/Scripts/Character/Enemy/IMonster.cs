namespace Game4.Scripts.Character.Enemy
{
    using System;

    /// <summary>
    /// 모든 몬스터가 구현해야 하는 인터페이스
    /// </summary>
    public interface IMonster
    {
        /// <summary>
        /// 공격 데미지
        /// </summary>
        int AttackDamage { get; }
    
        /// <summary>
        /// 데미지를 받는 메서드
        /// </summary>
        /// <param name="damage">받을 데미지</param>
        void TakeDamage(int damage);
    
        /// <summary>
        /// 몬스터 사망 이벤트
        /// </summary>
        event Action OnMonsterDeath;
    }
}