using System.Collections;
using UnityEngine;
using Game.Scripts.Characters.Player;
using Game.Scripts.Core;
using Game.Scripts.Projectiles;

namespace Game.Scripts.Characters.Enemies
{
    /// <summary>
/// 고정 위치 적 캐릭터 구현
/// </summary>
public class FixedEnemy : BaseEnemy
{
    private float _attackCooldown = 3f;
    private int _currentPattern;
    private GridSystem _gridSystem;
    private PlayerController _player;
    private PlayerHealth _playerHealth;
    
    public GameObject projectilePrefab;
    public GameObject warningTilePrefab;
    
    public float AttackCooldown { get => _attackCooldown; set => _attackCooldown = value; }
    
    void Start()
    {
        _gridSystem = FindAnyObjectByType<GridSystem>();
        _player = FindAnyObjectByType<PlayerController>();
        _playerHealth = _player.GetComponent<PlayerHealth>();
        
        StartCoroutine(AttackRoutine());
    }
    
    /// <summary>
    /// 공격 패턴 실행 루틴
    /// </summary>
    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(2f); // 초반 대기
        
        while (!IsDead)
        {
            ExecutePattern();
            _currentPattern = (_currentPattern + 1) % 3;
            
            yield return new WaitForSeconds(_attackCooldown);
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
        
        if (projectilePrefab != null && _player != null)
        {
            Vector3 direction = (_player.transform.position - transform.position).normalized;
            GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectile.Initialize(direction);
        }
        
        yield return null;
    }
    
    /// <summary>
    /// 패턴 2: 플레이어 위치 범위 공격
    /// </summary>
    private IEnumerator AreaAttack()
    {
        Debug.Log("적: 영역 공격 준비");
        
        // 플레이어 위치 가져오기
        int playerX, playerY;
        _gridSystem.GetXY(_player.transform.position, out playerX, out playerY);
        
        // 경고 타일 표시 (3x3 영역)
        GameObject[] warningTiles = new GameObject[9]; // 3x3 = 9
        int index = 0;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                int tileX = playerX + x;
                int tileY = playerY + y;
        
                if (_gridSystem.IsValidPosition(tileX, tileY))
                {
                    Vector3 tilePos = _gridSystem.GetWorldPosition(tileX, tileY);
                    warningTiles[index++] = Instantiate(warningTilePrefab, tilePos, Quaternion.identity);
                }
            }
        }
        
        // 경고 대기
        yield return new WaitForSeconds(1.5f);
        
        // 공격 실행
        Debug.Log("적: 영역 공격 발동");
        
        // 플레이어가 영역 내에 있는지 확인
        _gridSystem.GetXY(_player.transform.position, out int currentX, out int currentY);
        if (Mathf.Abs(currentX - playerX) <= 1 && Mathf.Abs(currentY - playerY) <= 1)
        {
            _playerHealth.TakeDamage(15);
        }
        
        // 경고 타일 제거
        foreach (GameObject tile in warningTiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
    }
    
    /// <summary>
    /// 패턴 3: 그리드 절반 공격
    /// </summary>
    private IEnumerator HalfGridAttack()
    {
        Debug.Log("적: 광역 공격 준비");
        
        // 공격할 절반 결정 (가로 또는 세로)
        bool isHorizontal = Random.value > 0.5f;
        int splitValue = _gridSystem.Height / 2; 
        
        // 경고 타일 생성
        GameObject[] warningTiles = new GameObject[_gridSystem.Width * _gridSystem.Height / 2];
        int index = 0;
        
        for (int x = 0; x < _gridSystem.Width; x++)
        {
            for (int y = 0; y < _gridSystem.Height; y++)
            {
                bool shouldAttack = isHorizontal ? y < splitValue : x < splitValue;
                
                if (shouldAttack)
                {
                    Vector3 tilePos = _gridSystem.GetWorldPosition(x, y);
                    warningTiles[index] = Instantiate(warningTilePrefab, tilePos, Quaternion.identity);
                    index++;
                }
            }
        }
        
        // 경고 대기
        yield return new WaitForSeconds(2f);
        
        // 공격 실행
        Debug.Log("적: 광역 공격 발동");
        
        // 플레이어 위치 확인
        _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
        bool isPlayerInArea = isHorizontal ? playerY < splitValue : playerX < splitValue;
        
        if (isPlayerInArea)
        {
            _playerHealth.TakeDamage(20);
        }
        
        // 경고 타일 제거
        foreach (GameObject tile in warningTiles)
        {
            if (tile != null)
            {
                Destroy(tile);
            }
        }
    }
}
}