using Game3.Scripts.Core;

namespace Game3.Scripts.Character.Enemy
{
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    /// <summary>
    /// 몬스터 프리팹
    /// </summary>
    [SerializeField] private GameObject enemyPrefab;
    
    /// <summary>
    /// 소환 주기
    /// </summary>
    [SerializeField] private float spawnInterval = 3f;
    
    /// <summary>
    /// 한 번에 소환할 몬스터 수
    /// </summary>
    [SerializeField] private int spawnCount = 2;
    
    /// <summary>
    /// 플레이어로부터의 최소 소환 거리
    /// </summary>
    [SerializeField] private float minDistanceFromPlayer = 5f;
    
    /// <summary>
    /// 소환 범위 (플레이어 중심 반경)
    /// </summary>
    [SerializeField] private float spawnRadius = 15f;
    
    /// <summary>
    /// 소환할 최대 몬스터 수 (클리어 조건)
    /// </summary>
    [SerializeField] private int maxEnemies = 20;
    
    /// <summary>
    /// 현재까지 소환된 몬스터 수
    /// </summary>
    private int enemiesSpawned = 0;
    
    /// <summary>
    /// 플레이어 트랜스폼 참조
    /// </summary>
    private Transform playerTransform;
    
    /// <summary>
    /// 소환 활성화 여부
    /// </summary>
    private bool isSpawningActive = true;
    
    /// <summary>
    /// 게임 매니저 참조
    /// </summary>
    private GameManager gameManager;
    
    /// <summary>
    /// 소환된 몬스터 수 프로퍼티
    /// </summary>
    public int EnemiesSpawned { get => enemiesSpawned; private set => enemiesSpawned = value; }
    
    /// <summary>
    /// 최대 몬스터 수 프로퍼티
    /// </summary>
    public int MaxEnemies { get => maxEnemies; set => maxEnemies = value; }

    private void Start()
    {
        // 플레이어 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        
        gameManager = GameManager.Instance;
        
        // 소환 시작
        StartCoroutine(SpawnEnemies());
    }

    /// <summary>
    /// 일정 주기마다 몬스터를 소환하는 코루틴
    /// </summary>
    private IEnumerator SpawnEnemies()
    {
        while (isSpawningActive && enemiesSpawned < maxEnemies)
        {
            for (int i = 0; i < spawnCount; i++)
            {
                if (enemiesSpawned >= maxEnemies)
                {
                    break;
                }
                
                SpawnEnemy();
                enemiesSpawned++;
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
        
        // 모든 몬스터가 소환된 후 체크 시작
        StartCoroutine(CheckAllEnemiesDefeated());
    }

    /// <summary>
    /// 개별 몬스터 소환 메서드
    /// </summary>
    private void SpawnEnemy()
    {
        if (enemyPrefab != null && playerTransform != null)
        {
            Vector2 spawnPosition = GetRandomSpawnPosition();
            Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
        }
    }

    /// <summary>
    /// 플레이어로부터 적절한 거리의 랜덤 위치 반환
    /// </summary>
    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 randomPosition;
        float distanceFromPlayer;
        int attempts = 0;
        int maxAttempts = 30;
        
        do
        {
            // 플레이어 중심으로 랜덤한 방향의 위치 생성
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float distance = Random.Range(minDistanceFromPlayer, spawnRadius);
            
            float x = playerTransform.position.x + distance * Mathf.Cos(angle);
            float y = playerTransform.position.y + distance * Mathf.Sin(angle);
            
            randomPosition = new Vector2(x, y);
            distanceFromPlayer = Vector2.Distance(randomPosition, playerTransform.position);
            
            attempts++;
        }
        while (distanceFromPlayer < minDistanceFromPlayer && attempts < maxAttempts);
        
        return randomPosition;
    }

    /// <summary>
    /// 모든 몬스터가 처치되었는지 확인하는 코루틴
    /// </summary>
    private IEnumerator CheckAllEnemiesDefeated()
    {
        // 모든 몬스터가 소환될 때까지 대기
        while (enemiesSpawned < maxEnemies)
        {
            yield return new WaitForSeconds(1f);
        }
        
        while (true)
        {
            // 씬에 남아있는 몬스터 수 확인
            GameObject[] remainingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
            
            // 남은 몬스터가 없으면 게임 클리어
            if (remainingEnemies.Length == 0)
            {
                if (gameManager != null)
                {
                    gameManager.GameCleared();
                }
                break;
            }
            
            yield return new WaitForSeconds(1f);
        }
    }

    /// <summary>
    /// 소환 활성화/비활성화 토글 메서드
    /// </summary>
    /// <param name="active">활성화 여부</param>
    public void SetSpawningActive(bool active)
    {
        isSpawningActive = active;
    }
}
}