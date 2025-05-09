namespace Game.Scripts.Tiles  
{
    using UnityEngine;

    public class AttackTile : BaseTile
    {
        private int _damage = 10;
        
        public int Damage { get => _damage; set => _damage = value; }
        
        public override void Activate()
        {
            base.Activate();
            if (GetState() == TileState.Activated)
            {
                Debug.Log($"공격 타일 발동! 데미지: {_damage}");
                // TODO: 실제 공격 로직
            }
        }
    }   
}