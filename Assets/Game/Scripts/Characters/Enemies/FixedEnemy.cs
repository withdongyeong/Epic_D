namespace Game.Scripts.Characters.Enemies
{
    using System.Collections;
    using UnityEngine;
    using Core;
    using Player;

/// <summary>
/// 고정 위치 적 캐릭터 구현
/// </summary>
public class FixedEnemy : BaseEnemy
{
    private float _attackCooldown = 3f;
    private int _currentPattern;
    private GridSystem _gridSystem;
    private PlayerController _player;
    
    public float AttackCooldown { get => _attackCooldown; set => _attackCooldown = value; }
    
    void Start()
    {
        _gridSystem = FindAnyObjectByType<GridSystem>();
        _player = FindAnyObjectByType<PlayerController>();
        
        StartCoroutine(AttackRoutine());
    }
    
    /// <summary>
    /// 공격 패턴 실행 루틴
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        while (!IsDead)
        {
            yield return new WaitForSeconds(_attackCooldown);
            
            if (!_player.IsMoving)
            {
                ExecutePattern();
                _currentPattern = (_currentPattern + 1) % 3;
            }
        }
    }
    
    /// <summary>
    /// 현재 패턴 실행
    /// </summary>
    private void ExecutePattern()
    {
        switch (_currentPattern)
        {
            case 0:
                StartCoroutine(ShootProjectile());
                break;
            case 1:
                StartCoroutine(AreaAttack());
                break;
            case 2:
                StartCoroutine(HalfGridAttack());
                break;
        }
    }
    
    /// <summary>
    /// 패턴 1: 투사체 발사
    /// </summary>
    private IEnumerator ShootProjectile()
    {
        Debug.Log("적: 투사체 발사");
        // TODO: 투사체 생성 및 발사
        yield return null;
    }
    
    /// <summary>
    /// 패턴 2: 플레이어 위치 범위 공격
    /// </summary>
    private IEnumerator AreaAttack()
    {
        Debug.Log("적: 영역 공격 준비");
        // 경고 표시
        yield return new WaitForSeconds(1.5f);
        Debug.Log("적: 영역 공격 발동");
        // 데미지 적용
    }
    
    /// <summary>
    /// 패턴 3: 그리드 절반 공격
    /// </summary>
    private IEnumerator HalfGridAttack()
    {
        Debug.Log("적: 광역 공격 준비");
        // 경고 표시
        yield return new WaitForSeconds(2f);
        Debug.Log("적: 광역 공격 발동");
        // 데미지 적용
    }
}
}