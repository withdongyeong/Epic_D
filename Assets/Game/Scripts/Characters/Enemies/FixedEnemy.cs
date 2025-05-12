using System.Collections;
using System.Collections.Generic;
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
        // 기존 변수들 유지
        private int _currentPattern;
        private GridSystem _gridSystem;
        private PlayerController _player;
        private PlayerHealth _playerHealth;
        private float _patternCooldown = 0.6f; // 패턴 간 딜레이
        
        public GameObject projectilePrefab;
        public GameObject warningTilePrefab;
        public GameObject explosionEffectPrefab; // 폭발 이펙트 프리팹
        
        // 메테오 프리팹 추가
        public GameObject meteorPrefab;
        public float meteorHoverHeight = 2f; // 몬스터 머리 위에 떠있는 높이
        public float meteorTravelDuration = 1.5f; // 날아가는 시간

        public GameObject magicSwordPrefab;
        public float swordTravelDuration = 1f;
        
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
            yield return new WaitForSeconds(1f); // 초반 딜레이 감소
        
            while (!IsDead)
            {
                ExecutePattern();
                yield return new WaitForSeconds(_patternCooldown); // 패턴 간 딜레이 감소
            }
        }
        
        /// <summary>
        /// 현재 패턴 실행
        /// </summary>
        private void ExecutePattern()
        {
            switch (Random.Range(0, 7)) // 7개 패턴
            {
                case 0:
                    StartCoroutine(AreaAttack());
                    break;
                case 1:
                    StartCoroutine(HalfGridAttack());
                    break;
                case 2:
                    StartCoroutine(RapidFirePattern());
                    break;
                case 3:
                    StartCoroutine(CrossAttackPattern());
                    break;
                case 4:
                    StartCoroutine(MultiAreaAttack());
                    break;
                case 5:
                    StartCoroutine(DiagonalAttackPattern());
                    break;
                case 6:
                    StartCoroutine(DiagonalCrossPattern());
                    break;
            }
        }
        
        /// <summary>
        /// 데미지 효과 생성
        /// </summary>
        private void CreateDamageEffect(Vector3 position)
        {
            if (explosionEffectPrefab != null)
            {
                GameObject effect = Instantiate(explosionEffectPrefab, position, Quaternion.identity);
                Destroy(effect, 0.1f);
            }
        }
        
        /// <summary>
        /// 플레이어에게 데미지 적용 및 이펙트 표시
        /// </summary>
        private void ApplyDamageWithEffect(int damage)
        {
            if (_playerHealth != null)
            {
                _playerHealth.TakeDamage(damage);
                CreateDamageEffect(_player.transform.position);
            }
        }
        
        /// <summary>
        /// 패턴 2: 플레이어 위치 범위 공격 (매직 소드 애니메이션 추가)
        /// </summary>
        private IEnumerator AreaAttack()
        {
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
                        warningTiles[index] = Instantiate(warningTilePrefab, tilePos, Quaternion.identity);
                        index++;
                    }
                }
            }
    
            // 타겟 영역 중앙 위치 계산
            Vector3 targetCenter = _gridSystem.GetWorldPosition(playerX, playerY);
    
            // 매직 소드 애니메이션 시작 - 1.5초 대기 시간에 맞춤
            if (magicSwordPrefab != null)
            {
                StartCoroutine(MagicSwordAnimation(targetCenter, 1.5f));
            }
    
            // 경고 대기
            yield return new WaitForSeconds(1.5f);
    
            // 플레이어가 영역 내에 있는지 확인
            _gridSystem.GetXY(_player.transform.position, out int currentX, out int currentY);
            if (Mathf.Abs(currentX - playerX) <= 1 && Mathf.Abs(currentY - playerY) <= 1)
            {
                ApplyDamageWithEffect(15);
            }
    
            // 공격 영역에 폭발 이펙트 생성
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int tileX = playerX + x;
                    int tileY = playerY + y;
    
                    if (_gridSystem.IsValidPosition(tileX, tileY))
                    {
                        Vector3 tilePos = _gridSystem.GetWorldPosition(tileX, tileY);
                        CreateDamageEffect(tilePos);
                    }
                }
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
        /// 패턴 3: 그리드 절반 공격 (메테오 애니메이션 추가)
        /// </summary>
        private IEnumerator HalfGridAttack()
        {
            // 공격할 절반 결정 (가로 또는 세로)
            bool isHorizontal = Random.value > 0.5f;
            int splitValue = _gridSystem.Height / 2; 
            
            // 경고 타일 생성
            List<Vector3> attackPositions = new List<Vector3>();
            GameObject[] warningTiles = new GameObject[_gridSystem.Width * _gridSystem.Height / 2];
            int index = 0;
            
            // 절반 그리드의 중앙 계산
            Vector3 targetCenter = Vector3.zero;
            int positionCount = 0;
            
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                for (int y = 0; y < _gridSystem.Height; y++)
                {
                    bool shouldAttack = isHorizontal ? y < splitValue : x < splitValue;
                    
                    if (shouldAttack)
                    {
                        Vector3 tilePos = _gridSystem.GetWorldPosition(x, y);
                        attackPositions.Add(tilePos);
                        warningTiles[index] = Instantiate(warningTilePrefab, tilePos, Quaternion.identity);
                        index++;
                        
                        // 중앙 계산용
                        targetCenter += tilePos;
                        positionCount++;
                    }
                }
            }
            
            // 절반 그리드의 중앙 위치 계산
            targetCenter /= positionCount;
            
            // 메테오 애니메이션 시작
            if (meteorPrefab != null)
            {
                StartCoroutine(MeteorAnimation(targetCenter, meteorTravelDuration));
            }
            
            // 경고 대기
            yield return new WaitForSeconds(2f);
            
            // 플레이어 위치 확인
            _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
            bool isPlayerInArea = isHorizontal ? playerY < splitValue : playerX < splitValue;
            
            if (isPlayerInArea)
            {
                ApplyDamageWithEffect(20);
            }
            
            // 공격 영역에 폭발 이펙트 생성
            foreach (Vector3 pos in attackPositions)
            {
                CreateDamageEffect(pos);
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
        /// 패턴 4 : 연속 투사체 발사
        /// </summary>
        private IEnumerator RapidFirePattern()
        {
            for (int i = 0; i < 5; i++)
            {
                Vector3 direction = (_player.transform.position - transform.position).normalized;
                GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
                Projectile projectile = projectileObj.GetComponent<Projectile>();
                projectile.Initialize(direction, Projectile.ProjectileTeam.Enemy);
            
                yield return new WaitForSeconds(0.3f);
            }
        }
    
        /// <summary>
        /// 패턴 5 : 십자 공격
        /// </summary>
        private IEnumerator CrossAttackPattern()
        {
            // 플레이어 위치
            _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
        
            // 십자 경고 생성
            List<GameObject> warningTiles = new List<GameObject>();
            List<Vector3> attackPositions = new List<Vector3>();
        
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                Vector3 pos = _gridSystem.GetWorldPosition(x, playerY);
                attackPositions.Add(pos);
                warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
            }
        
            for (int y = 0; y < _gridSystem.Height; y++)
            {
                if (y != playerY) // 중복 방지
                {
                    Vector3 pos = _gridSystem.GetWorldPosition(playerX, y);
                    attackPositions.Add(pos);
                    warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
                }
            }
        
            yield return new WaitForSeconds(0.8f);
        
            // 공격 실행
            _gridSystem.GetXY(_player.transform.position, out int currentX, out int currentY);
            if (currentX == playerX || currentY == playerY)
            {
                ApplyDamageWithEffect(15);
            }
            
            // 공격 영역에 폭발 이펙트 생성
            foreach (Vector3 pos in attackPositions)
            {
                CreateDamageEffect(pos);
            }
        
            // 제거
            foreach (GameObject tile in warningTiles)
            {
                Destroy(tile);
            }
        }
        
/// <summary>
/// 패턴 6 : 연속 영역 공격 (매직 소드 애니메이션 추가)
/// </summary>
private IEnumerator MultiAreaAttack()
{
    for (int i = 0; i < 3; i++)
    {
        // 랜덤 위치 선택
        int targetX = Random.Range(0, _gridSystem.Width);
        int targetY = Random.Range(0, _gridSystem.Height);

        // 경고 타일 생성 (2x2 영역)
        List<GameObject> warningTiles = new List<GameObject>();
        List<Vector3> attackPositions = new List<Vector3>();

        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                int tileX = targetX + x;
                int tileY = targetY + y;
        
                if (_gridSystem.IsValidPosition(tileX, tileY))
                {
                    Vector3 tilePos = _gridSystem.GetWorldPosition(tileX, tileY);
                    attackPositions.Add(tilePos);
                    warningTiles.Add(Instantiate(warningTilePrefab, tilePos, Quaternion.identity));
                }
            }
        }
        
        // 타겟 영역 중앙 위치 계산 (2x2 영역의 중심)
        Vector3 targetCenter = _gridSystem.GetWorldPosition(targetX, targetY) + 
                              new Vector3(_gridSystem.CellSize / 2, _gridSystem.CellSize / 2, 0);
        
        // 매직 소드 애니메이션 시작 - 0.6초 대기 시간에 맞춤
        if (magicSwordPrefab != null)
        {
            StartCoroutine(MagicSwordAnimation(targetCenter, 0.6f));
        }

        yield return new WaitForSeconds(0.6f);

        // 공격 실행
        _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
        bool isHit = playerX >= targetX && playerX < targetX + 2 && 
                     playerY >= targetY && playerY < targetY + 2;

        if (isHit)
        {
            ApplyDamageWithEffect(10);
        }
        
        // 공격 영역에 폭발 이펙트 생성
        foreach (Vector3 pos in attackPositions)
        {
            CreateDamageEffect(pos);
        }

        // 경고 타일 제거
        foreach (GameObject tile in warningTiles)
        {
            Destroy(tile);
        }

        yield return new WaitForSeconds(0.3f);
    }
}
        
        /// <summary>
        /// 패턴 7 : 대각선 공격
        /// </summary>
        private IEnumerator DiagonalAttackPattern()
        {
            // 플레이어 위치
            _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
            
            // 대각선 경고 생성
            List<GameObject> warningTiles = new List<GameObject>();
            List<Vector3> attackPositions = new List<Vector3>();
            
            // 우상단-좌하단 대각선
            int offset = playerY - playerX;
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                int y = x + offset;
                if (_gridSystem.IsValidPosition(x, y))
                {
                    Vector3 pos = _gridSystem.GetWorldPosition(x, y);
                    attackPositions.Add(pos);
                    warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
                }
            }
            
            // 좌상단-우하단 대각선
            offset = playerY + playerX;
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                int y = offset - x;
                if (_gridSystem.IsValidPosition(x, y))
                {
                    Vector3 pos = _gridSystem.GetWorldPosition(x, y);
                    attackPositions.Add(pos);
                    warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
                }
            }
            
            yield return new WaitForSeconds(0.8f);
            
            // 공격 실행
            _gridSystem.GetXY(_player.transform.position, out int currentX, out int currentY);
            
            // 우상단-좌하단 대각선 검사
            bool isOnDiagonal1 = (currentY - currentX) == (playerY - playerX);
            
            // 좌상단-우하단 대각선 검사
            bool isOnDiagonal2 = (currentY + currentX) == (playerY + playerX);
            
            if (isOnDiagonal1 || isOnDiagonal2)
            {
                ApplyDamageWithEffect(15);
            }
            
            // 공격 영역에 폭발 이펙트 생성
            foreach (Vector3 pos in attackPositions)
            {
                CreateDamageEffect(pos);
            }
            
            // 제거
            foreach (GameObject tile in warningTiles)
            {
                Destroy(tile);
            }
        }
        
        /// <summary>
        /// 패턴 8 : 대각선 후 십자 공격 패턴
        /// </summary>
        private IEnumerator DiagonalCrossPattern()
        {
            // 플레이어 위치
            _gridSystem.GetXY(_player.transform.position, out int playerX, out int playerY);
            
            // 1단계: 대각선 경고 생성
            List<GameObject> warningTiles = new List<GameObject>();
            List<Vector3> attackPositions = new List<Vector3>();
            
            // 우상단-좌하단 대각선
            int offset = playerY - playerX;
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                int y = x + offset;
                if (_gridSystem.IsValidPosition(x, y))
                {
                    Vector3 pos = _gridSystem.GetWorldPosition(x, y);
                    attackPositions.Add(pos);
                    warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
                }
            }
            
            yield return new WaitForSeconds(0.8f);
            
            // 대각선 공격 실행
            _gridSystem.GetXY(_player.transform.position, out int currentX, out int currentY);
            bool isOnDiagonal = (currentY - currentX) == (playerY - playerX);
            
            if (isOnDiagonal)
            {
                ApplyDamageWithEffect(10);
            }
            
            // 공격 영역에 폭발 이펙트 생성
            foreach (Vector3 pos in attackPositions)
            {
                CreateDamageEffect(pos);
            }
            
            // 경고 타일 제거
            foreach (GameObject tile in warningTiles)
            {
                Destroy(tile);
            }
            
            // 새로운 플레이어 위치 가져오기
            _gridSystem.GetXY(_player.transform.position, out playerX, out playerY);
            
            // 2단계: 십자 경고 생성
            warningTiles = new List<GameObject>();
            attackPositions = new List<Vector3>();
            
            yield return new WaitForSeconds(0.3f);
            
            // 가로 방향
            for (int x = 0; x < _gridSystem.Width; x++)
            {
                Vector3 pos = _gridSystem.GetWorldPosition(x, playerY);
                attackPositions.Add(pos);
                warningTiles.Add(Instantiate(warningTilePrefab, pos, Quaternion.identity));
            }
            
            yield return new WaitForSeconds(0.8f);
            
            // 십자 공격 실행
            _gridSystem.GetXY(_player.transform.position, out currentX, out currentY);
            if (currentY == playerY)
            {
                ApplyDamageWithEffect(15);
            }
            
            // 공격 영역에 폭발 이펙트 생성
            foreach (Vector3 pos in attackPositions)
            {
                CreateDamageEffect(pos);
            }
            
            // 타일 제거
            foreach (GameObject tile in warningTiles)
            {
                Destroy(tile);
            }
        }
        
        /// <summary>
        /// 메테오 애니메이션 처리
        /// </summary>
        private IEnumerator MeteorAnimation(Vector3 targetPosition, float duration)
        {
            // 메테오 생성 (몬스터 머리 위에 생성)
            Vector3 startPosition = transform.position + Vector3.up * meteorHoverHeight;
            GameObject meteor = Instantiate(meteorPrefab, startPosition, Quaternion.identity);
    
            // 1단계: 위로 높이 올라가는 단계
            float riseHeight = 5f;
            Vector3 risePosition = startPosition + Vector3.up * riseHeight;
            float riseTime = duration * 0.2f;
    
            float elapsed = 0f;
            while (elapsed < riseTime)
            {
                meteor.transform.position = Vector3.Lerp(startPosition, risePosition, elapsed / riseTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
    
            // 2단계: 잠시 멈추는 단계
            float hoverTime = duration * 0.3f;
            yield return new WaitForSeconds(hoverTime);
    
            // 3단계: 목표 위치로 직선으로 날아가는 단계
            float fallTime = duration * 0.5f;
            elapsed = 0f;
            Vector3 initialScale = meteor.transform.localScale;
            Vector3 finalScale = initialScale * 1.5f; // 날아가면서 약간 커짐
    
            while (elapsed < fallTime)
            {
                float t = elapsed / fallTime;
        
                // 직선 이동
                meteor.transform.position = Vector3.Lerp(risePosition, targetPosition, t);
        
                // 크기 보간 (날아가면서 커짐)
                meteor.transform.localScale = Vector3.Lerp(initialScale, finalScale, t);
        
                // 메테오 회전 효과
                meteor.transform.Rotate(Vector3.forward, 360f * Time.deltaTime);
        
                elapsed += Time.deltaTime;
                yield return null;
            }
    
            // 메테오 파괴
            Destroy(meteor);
        }
        /// <summary>
        /// 매직 소드 애니메이션 처리
        /// </summary>
        /// <summary>
        /// 매직 소드 애니메이션 처리
        /// </summary>
        private IEnumerator MagicSwordAnimation(Vector3 targetPosition, float waitTime)
        {
            // 매직 소드 생성 (몬스터 머리 위에 생성)
            Vector3 startPosition = transform.position + Vector3.up * meteorHoverHeight;
            GameObject sword = Instantiate(magicSwordPrefab, startPosition, Quaternion.identity);
    
            // 타겟 방향으로 회전 (시작부터 설정)
            Vector3 direction = (targetPosition - startPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            sword.transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    
            // 총 애니메이션 시간을 waitTime에 맞춤
            float totalAnimTime = waitTime;
    
            // 1단계: 위로 올라가는 시간 (총 시간의 20%)
            float riseTime = totalAnimTime * 0.2f;
            float riseHeight = 4f;
            Vector3 risePosition = startPosition + Vector3.up * riseHeight;
    
            float elapsed = 0f;
            while (elapsed < riseTime)
            {
                sword.transform.position = Vector3.Lerp(startPosition, risePosition, elapsed / riseTime);
                elapsed += Time.deltaTime;
                yield return null;
            }
    
            // 2단계: 정지 단계 (총 시간의 50%)
            float hoverTime = totalAnimTime * 0.5f;
            yield return new WaitForSeconds(hoverTime);
    
            // 3단계: 타겟까지 빠르게 이동 (총 시간의 30%)
            float fallTime = totalAnimTime * 0.3f;
            elapsed = 0f;
    
            while (elapsed < fallTime)
            {
                float t = elapsed / fallTime;
        
                // 이징 적용 (가속도 효과)
                float easedT = 1f - Mathf.Pow(1f - t, 2f);
        
                // 직선 이동
                sword.transform.position = Vector3.Lerp(risePosition, targetPosition, easedT);
        
                elapsed += Time.deltaTime;
                yield return null;
            }
    
            // 최종 위치로 설정
            sword.transform.position = targetPosition;
            
            Destroy(sword);
        }
    }
}