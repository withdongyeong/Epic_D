using UnityEngine;

public class AttackTile : BaseTile
{
    private int damage = 10;
    
    public int Damage { get => damage; set => damage = value; }
    
    public override void Activate()
    {
        base.Activate();
        if (GetState() == TileState.Activated)
        {
            Debug.Log($"공격 타일 발동! 데미지: {damage}");
            // TODO: 실제 공격 로직
        }
    }
}