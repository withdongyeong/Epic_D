using Game4.Scripts.Character.Enemy;

namespace Game4.Scripts.Core
{
using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    /// <summary>
    /// 몬스터 타입
    /// </summary>
    public enum MonsterType
    {
        Basic,   // 기본 몬스터
        Ranged,  // 원거리 몬스터
        Fast,    // 빠른 몬스터
        Boss     // 보스 몬스터
    }
    
    /// <summary>
    /// 몬스터 프리팹
    /// </summary>
    public GameObject monsterPrefab;
    
    /// <summary>
    /// 스폰할 몬스터 수
    /// </summary>
    private int monsterCount = 10;
    
    /// <summary>
    /// 스폰 영역의 중심점
    /// </summary>
    public GameObject spawnCenter;
    
    /// <summary>
    /// 스폰 영역의 크기 (가로)
    /// </summary>
    private float spawnAreaWidth = 50f;
    
    /// <summary>
    /// 스폰 영역의 크기 (세로)
    /// </summary>
    private float spawnAreaHeight = 50f;
    
    /// <summary>
    /// 플레이어와의 최소 거리
    /// </summary>
    private float minDistanceFromPlayer = 10f;
    
    /// <summary>
    /// 스폰된 몬스터들의 리스트
    /// </summary>
    private List<GameObject> spawnedMonsters = new List<GameObject>();
    
    /// <summary>
    /// 플레이어 참조
    /// </summary>
    private Transform player;

    // Properties
    /// <summary>
    /// 몬스터 프리팹 프로퍼티
    /// </summary>
    public GameObject MonsterPrefab { get => monsterPrefab; set => monsterPrefab = value; }
    
    /// <summary>
    /// 스폰할 몬스터 수 프로퍼티
    /// </summary>
    public int MonsterCount { get => monsterCount; set => monsterCount = value; }
    
    /// <summary>
    /// 스폰 영역 중심점 프로퍼티
    /// </summary>
    public GameObject SpawnCenter { get => spawnCenter; set => spawnCenter = value; }
    
    /// <summary>
    /// 스폰 영역 가로 크기 프로퍼티
    /// </summary>
    public float SpawnAreaWidth { get => spawnAreaWidth; set => spawnAreaWidth = value; }
    
    /// <summary>
    /// 스폰 영역 세로 크기 프로퍼티
    /// </summary>
    public float SpawnAreaHeight { get => spawnAreaHeight; set => spawnAreaHeight = value; }
    
    /// <summary>
    /// 플레이어와의 최소 거리 프로퍼티
    /// </summary>
    public float MinDistanceFromPlayer { get => minDistanceFromPlayer; set => minDistanceFromPlayer = value; }
    
    /// <summary>
    /// 스폰된 몬스터 리스트 프로퍼티
    /// </summary>
    public List<GameObject> SpawnedMonsters { get => spawnedMonsters; private set => spawnedMonsters = value; }

    /// <summary>
    /// 땅에 박힌 검 프리팹
    /// </summary>
    public GameObject groundSwordPrefab;

    /// <summary>
    /// 스폰할 땅에 박힌 검 개수
    /// </summary>
    private int groundSwordCount = 5;

    /// <summary>
    /// 스폰된 땅에 박힌 검들의 리스트
    /// </summary>
    private List<GameObject> spawnedGroundSwords = new List<GameObject>();

    /// <summary>
    /// 땅에 박힌 검 프리팹 프로퍼티
    /// </summary>
    public GameObject GroundSwordPrefab { get => groundSwordPrefab; set => groundSwordPrefab = value; }

    /// <summary>
    /// 땅에 박힌 검 개수 프로퍼티
    /// </summary>
    public int GroundSwordCount { get => groundSwordCount; set => groundSwordCount = value; }
    
    /// <summary>
    /// 땅에 박힌 검 스폰 영역 크기 (가로) - 몬스터보다 넓음
    /// </summary>
    private float groundSwordSpawnAreaWidth = 50f;

    /// <summary>
    /// 땅에 박힌 검 스폰 영역 크기 (세로) - 몬스터보다 넓음
    /// </summary>
    private float groundSwordSpawnAreaHeight = 50f;

    /// <summary>
    /// 땅에 박힌 검 스폰 영역 크기 (가로) 프로퍼티
    /// </summary>
    public float GroundSwordSpawnAreaWidth { get => groundSwordSpawnAreaWidth; set => groundSwordSpawnAreaWidth = value; }

    /// <summary>
    /// 땅에 박힌 검 스폰 영역 크기 (세로) 프로퍼티
    /// </summary>
    public float GroundSwordSpawnAreaHeight { get => groundSwordSpawnAreaHeight; set => groundSwordSpawnAreaHeight = value; }
    
    /// <summary>
    /// 스폰 중심을 플레이어로 할지 여부
    /// </summary>
    private bool usePlayerAsSpawnCenter = true;

    /// <summary>
    /// 스폰 중심을 플레이어로 할지 여부 프로퍼티
    /// </summary>
    public bool UsePlayerAsSpawnCenter { get => usePlayerAsSpawnCenter; set => usePlayerAsSpawnCenter = value; }
    
    private int totalMonsterCount = 100;
    private int waveCount = 5;
    private int currentWave = 0;
    private float waveInterval = 12f;
    private float lastWaveSpawnTime = 0f;
    private float groundSwordSpawnInterval = 8f;
    private float lastGroundSwordSpawnTime = 0f;
    private bool isWaveActive = false;
    
    /// <summary>
    /// 총 몬스터 수 프로퍼티
    /// </summary>
    public int TotalMonsterCount { get => totalMonsterCount; set => totalMonsterCount = value; }

    /// <summary>
    /// 웨이브 수 프로퍼티
    /// </summary>
    public int WaveCount { get => waveCount; set => waveCount = value; }

    /// <summary>
    /// 현재 웨이브 프로퍼티
    /// </summary>
    public int CurrentWave { get => currentWave; private set => currentWave = value; }
    
    /// <summary>
    /// 몬스터 타입별 프리팹들
    /// </summary>
    public GameObject basicMonsterPrefab;     // 기본 몬스터
    public GameObject rangedMonsterPrefab;    // 원거리 몬스터  
    public GameObject fastMonsterPrefab;      // 빠른 몬스터
    public GameObject bossMonsterPrefab;      // 보스 몬스터

    /// <summary>
    /// 보스 등장 조건 (총 처치 몬스터 수)
    /// </summary>
    private int bossSpawnThreshold = 90;

    /// <summary>
    /// 현재까지 처치한 몬스터 수
    /// </summary>
    private int totalKilledMonsters = 0;

    /// <summary>
    /// 보스가 스폰되었는지 여부
    /// </summary>
    private bool bossSpawned = false;

    /// <summary>
    /// 웨이브별 몬스터 구성 비율 (5웨이브용)
    /// </summary>
    private float[] basicMonsterRatio = { 1.0f, 0.8f, 0.6f, 0.4f, 0.2f };
    private float[] rangedMonsterRatio = { 0.0f, 0.2f, 0.3f, 0.3f, 0.3f };
    private float[] fastMonsterRatio = { 0.0f, 0.0f, 0.1f, 0.3f, 0.5f };
    /// <summary>
    /// 보스가 소환되었는지 여부 프로퍼티
    /// </summary>
    public bool BossSpawned { get => bossSpawned; private set => bossSpawned = value; }
    /// <summary>
    /// 보스 등장 조건 프로퍼티
    /// </summary>
    public int BossSpawnThreshold { get => bossSpawnThreshold; set => bossSpawnThreshold = value; }

    /// <summary>
    /// 총 처치 몬스터 수 프로퍼티
    /// </summary>
    public int TotalKilledMonsters { get => totalKilledMonsters; set => totalKilledMonsters = value; }
    
    /// <summary>
    /// 초기화
    /// </summary>
    private void Start()
    {
        // 플레이어 찾기
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        
        Debug.Log("SpawnManager initialized");
    }
    
    private void Update()
    {
        if (isWaveActive)
        {
            HandleWaveSpawning();
            HandleGroundSwordSpawning();
        }
    }
    
    public void StopWaves()
    {
        isWaveActive = false;
        currentWave = 0;
    
        Debug.Log("Wave system stopped");
    }
    
    public void StartWaves()
    {
        ClearSpawnedMonsters();
        ClearSpawnedGroundSwords();

        currentWave = 0;
        totalKilledMonsters = 0;
        bossSpawned = false;
        isWaveActive = true;
        lastWaveSpawnTime = Time.time - waveInterval;

        // 초기 검 3개 스폰
        for (int i = 0; i < 3; i++)
        {
            SpawnGroundSword();
        }
    
        Debug.Log("Wave system started - Boss will spawn after killing " + bossSpawnThreshold + " monsters");
    }

    private void HandleWaveSpawning()
    {
        if (currentWave >= waveCount) return;
    
        if (Time.time - lastWaveSpawnTime >= waveInterval)
        {
            SpawnWave();
            lastWaveSpawnTime = Time.time;
        }
    }

/// <summary>
/// 단일 웨이브 스폰
/// </summary>
private void SpawnWave()
{
    if (basicMonsterPrefab == null) return;
    
    currentWave++;
    
    // 이번 웨이브에서 스폰할 몬스터 수
    int monstersToSpawn = totalMonsterCount / waveCount;
    
    // 마지막 웨이브의 경우 남은 몬스터 모두 스폰
    if (currentWave == waveCount)
    {
        int remainingMonsters = totalMonsterCount - (totalMonsterCount / waveCount * (waveCount - 1));
        monstersToSpawn = remainingMonsters;
    }
    
    // 웨이브에 따른 몬스터 타입 결정 및 스폰
    SpawnMonstersForWave(monstersToSpawn, currentWave);
    
    Debug.Log($"Wave {currentWave}/{waveCount} spawned - {monstersToSpawn} monsters");
    
    // 보스 스폰 체크 (마지막 웨이브이고 보스가 아직 안 나왔으면)
    if (currentWave == waveCount && !bossSpawned && totalKilledMonsters >= bossSpawnThreshold)
    {
        SpawnBoss();
    }
}

/// <summary>
/// 웨이브별 몬스터 구성에 따라 스폰
/// </summary>
/// <param name="totalCount">스폰할 총 몬스터 수</param>
/// <param name="wave">현재 웨이브</param>
private void SpawnMonstersForWave(int totalCount, int wave)
{
    int waveIndex = Mathf.Clamp(wave - 1, 0, basicMonsterRatio.Length - 1);
    
    // 각 타입별 스폰 수 계산
    int basicCount = Mathf.RoundToInt(totalCount * basicMonsterRatio[waveIndex]);
    int rangedCount = Mathf.RoundToInt(totalCount * rangedMonsterRatio[waveIndex]);
    int fastCount = totalCount - basicCount - rangedCount; // 나머지는 빠른 몬스터
    
    // 각 타입별 스폰
    for (int i = 0; i < basicCount; i++)
    {
        SpawnMonsterOfType(MonsterType.Basic);
    }
    
    for (int i = 0; i < rangedCount; i++)
    {
        SpawnMonsterOfType(MonsterType.Ranged);
    }
    
    for (int i = 0; i < fastCount; i++)
    {
        SpawnMonsterOfType(MonsterType.Fast);
    }
    
    Debug.Log($"Wave {wave}: Basic({basicCount}), Ranged({rangedCount}), Fast({fastCount})");
}

/// <summary>
/// 몬스터 타입별 스폰
/// </summary>
/// <param name="type">몬스터 타입</param>
/// <returns>스폰된 몬스터</returns>
public GameObject SpawnMonsterOfType(MonsterType type)
{
    GameObject prefabToSpawn = null;
    
    switch (type)
    {
        case MonsterType.Basic:
            prefabToSpawn = basicMonsterPrefab;
            break;
        case MonsterType.Ranged:
            prefabToSpawn = rangedMonsterPrefab;
            break;
        case MonsterType.Fast:
            prefabToSpawn = fastMonsterPrefab;
            break;
        case MonsterType.Boss:
            prefabToSpawn = bossMonsterPrefab;
            break;
    }
    
    if (prefabToSpawn == null)
    {
        Debug.LogWarning($"Monster prefab for type {type} is not assigned!");
        return null;
    }
    
    Vector2 spawnPosition = GetRandomSpawnPosition();
    GameObject monster = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    
    spawnedMonsters.Add(monster);
    
    // 모든 몬스터 타입에 대해 사망 이벤트 연결
    IMonster monsterInterface = monster.GetComponent<IMonster>();
    if (monsterInterface != null)
    {
        monsterInterface.OnMonsterDeath += () => OnMonsterKilled(monster);
    }
    
    return monster;
}

/// <summary>
/// 보스 스폰
/// </summary>
private void SpawnBoss()
{
    if (bossSpawned || bossMonsterPrefab == null) return;
    
    GameObject boss = SpawnMonsterOfType(MonsterType.Boss);
    bossSpawned = true;
    
    // GameManager에 보스 소환 알림
    if (GameManager.Instance != null)
    {
        GameManager.Instance.BossSpawned(boss);
    }
    
    Debug.Log("Boss Monster Spawned!");
}

/// <summary>
/// 몬스터 처치 시 호출
/// </summary>
/// <param name="monster">처치된 몬스터</param>
private void OnMonsterKilled(GameObject monster)
{
    RemoveMonster(monster);
    totalKilledMonsters++;
    
    Debug.Log($"Monster killed! Total killed: {totalKilledMonsters}");
    
    // 80마리 처치하면 즉시 보스 소환 (웨이브 상관없이)
    if (totalKilledMonsters >= bossSpawnThreshold && !bossSpawned)
    {
        SpawnBoss();
    }
}

    private void HandleGroundSwordSpawning()
    {
        if (Time.time - lastGroundSwordSpawnTime >= groundSwordSpawnInterval)
        {
            SpawnGroundSword();
            lastGroundSwordSpawnTime = Time.time;
        }
    }

    /// <summary>
    /// 모든 몬스터 스폰
    /// </summary>
    public void SpawnAllMonsters()
    {
        if (monsterPrefab == null)
        {
            Debug.LogError("Monster prefab is not assigned!");
            return;
        }
    
        // 기존 몬스터들 정리
        ClearSpawnedMonsters();
    
        // 기존 땅에 박힌 검들 정리
        ClearSpawnedGroundSwords();
    
        // 몬스터 스폰
        for (int i = 0; i < monsterCount; i++)
        {
            SpawnMonster();
        }
    
        // 땅에 박힌 검 스폰
        SpawnAllGroundSwords();
    
        Debug.Log($"Spawned {spawnedMonsters.Count} monsters and {spawnedGroundSwords.Count} ground swords");
    }

    /// <summary>
    /// 단일 몬스터 스폰
    /// </summary>
    /// <returns>스폰된 몬스터 GameObject</returns>
    public GameObject SpawnMonster()
    {
        Vector2 spawnPosition = GetRandomSpawnPosition();
        GameObject monster = Instantiate(monsterPrefab, spawnPosition, Quaternion.identity);
        
        // 스폰된 몬스터 리스트에 추가
        spawnedMonsters.Add(monster);
        
        // 몬스터 사망 시 리스트에서 제거하도록 이벤트 연결
        MonsterController monsterController = monster.GetComponent<MonsterController>();
        if (monsterController != null)
        {
            // 몬스터가 파괴될 때 리스트에서 제거하는 로직 추가 필요
            monster.GetComponent<MonsterController>().OnMonsterDeath += () => RemoveMonster(monster);
        }
        
        return monster;
    }

    /// <summary>
    /// 랜덤 스폰 위치 생성
    /// </summary>
    /// <returns>스폰 위치</returns>
    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 actualSpawnCenter = usePlayerAsSpawnCenter && player != null ? 
            (Vector2)player.position : spawnCenter.transform.position;
    
        Vector2 randomPosition;
        int attempts = 0;
        int maxAttempts = 50;
    
        do
        {
            // 스폰 영역 내 랜덤 위치 생성
            float randomX = Random.Range(actualSpawnCenter.x - spawnAreaWidth / 2f, actualSpawnCenter.x + spawnAreaWidth / 2f);
            float randomY = Random.Range(actualSpawnCenter.y - spawnAreaHeight / 2f, actualSpawnCenter.y + spawnAreaHeight / 2f);
            randomPosition = new Vector2(randomX, randomY);
        
            attempts++;
        
            // 최대 시도 횟수 초과 시 그냥 반환
            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("Could not find suitable spawn position, using last attempted position");
                break;
            }
        
        } while (IsPositionTooCloseToPlayer(randomPosition));
    
        return randomPosition;
    }

    /// <summary>
    /// 위치가 플레이어와 너무 가까운지 확인
    /// </summary>
    /// <param name="position">확인할 위치</param>
    /// <returns>너무 가까우면 true</returns>
    private bool IsPositionTooCloseToPlayer(Vector2 position)
    {
        if (player == null) return false;
        
        float distance = Vector2.Distance(position, player.position);
        return distance < minDistanceFromPlayer;
    }

    /// <summary>
    /// 몬스터를 리스트에서 제거
    /// </summary>
    /// <param name="monster">제거할 몬스터</param>
    public void RemoveMonster(GameObject monster)
    {
        if (spawnedMonsters.Contains(monster))
        {
            spawnedMonsters.Remove(monster);
        }
    }

    /// <summary>
    /// 스폰된 모든 몬스터 제거
    /// </summary>
    public void ClearSpawnedMonsters()
    {
        foreach (GameObject monster in spawnedMonsters)
        {
            if (monster != null)
            {
                Destroy(monster);
            }
        }
        spawnedMonsters.Clear();
    }

    /// <summary>
    /// 특정 위치에 몬스터 스폰
    /// </summary>
    /// <param name="position">스폰 위치</param>
    /// <returns>스폰된 몬스터</returns>
    public GameObject SpawnMonsterAt(Vector2 position)
    {
        if (monsterPrefab == null) return null;
        
        GameObject monster = Instantiate(monsterPrefab, position, Quaternion.identity);
        spawnedMonsters.Add(monster);
        
        return monster;
    }
    
    /// <summary>
    /// 모든 땅에 박힌 검 스폰
    /// </summary>
    public void SpawnAllGroundSwords()
    {
        if (groundSwordPrefab == null)
        {
            Debug.LogWarning("Ground sword prefab is not assigned!");
            return;
        }
    
        for (int i = 0; i < groundSwordCount; i++)
        {
            SpawnGroundSword();
        }
    }

    /// <summary>
    /// 단일 땅에 박힌 검 스폰
    /// </summary>
    /// <returns>스폰된 땅에 박힌 검</returns>
    public GameObject SpawnGroundSword()
    {
        Vector2 spawnPosition = GetRandomGroundSwordSpawnPosition();
        GameObject groundSword = Instantiate(groundSwordPrefab, spawnPosition, Quaternion.identity);
    
        spawnedGroundSwords.Add(groundSword);
    
        return groundSword;
    }

    /// <summary>
    /// 땅에 박힌 검용 랜덤 스폰 위치 생성 (넓은 범위)
    /// </summary>
    /// <returns>스폰 위치</returns>
    private Vector2 GetRandomGroundSwordSpawnPosition()
    {
        Vector2 actualSpawnCenter = usePlayerAsSpawnCenter && player != null ? 
            (Vector2)player.position : spawnCenter.transform.position;
    
        // 넓은 범위에서 랜덤 위치 생성
        float randomX = Random.Range(actualSpawnCenter.x - groundSwordSpawnAreaWidth / 2f, actualSpawnCenter.x + groundSwordSpawnAreaWidth / 2f);
        float randomY = Random.Range(actualSpawnCenter.y - groundSwordSpawnAreaHeight / 2f, actualSpawnCenter.y + groundSwordSpawnAreaHeight / 2f);
    
        return new Vector2(randomX, randomY);
    }

    /// <summary>
    /// 스폰된 모든 땅에 박힌 검 제거
    /// </summary>
    public void ClearSpawnedGroundSwords()
    {
        foreach (GameObject groundSword in spawnedGroundSwords)
        {
            if (groundSword != null)
            {
                Destroy(groundSword);
            }
        }
        spawnedGroundSwords.Clear();
    }

    
    /// <summary>
    /// 스폰 영역 시각화 (Scene 뷰에서 확인용)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Vector2 actualSpawnCenter = usePlayerAsSpawnCenter && player != null ? 
            (Vector2)player.position : spawnCenter.transform.position;
    
        // 몬스터 스폰 영역 그리기
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(actualSpawnCenter, new Vector3(spawnAreaWidth, spawnAreaHeight, 0));
    
        // 땅에 박힌 검 스폰 영역 그리기 (더 넓음)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(actualSpawnCenter, new Vector3(groundSwordSpawnAreaWidth, groundSwordSpawnAreaHeight, 0));
    
        // 플레이어 주변 최소 거리 영역 그리기
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }
}
}