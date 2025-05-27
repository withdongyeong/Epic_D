namespace Game4.Scripts.Sword
{
#pragma warning disable CS0618, CS0612, CS0672
using UnityEngine;
#pragma warning restore CS0618, CS0612, CS0672

public class GroundSword : MonoBehaviour
{
    /// <summary>
    /// 소환할 검 프리팹
    /// </summary>
    public GameObject[] swordPrefabs;
    
    /// <summary>
    /// 이미 수집되었는지 여부
    /// </summary>
    private bool isCollected = false;
    
    /// <summary>
    /// 수집 가능 범위
    /// </summary>
    private float collectionRange = 1f;
    
    /// <summary>
    /// 플레이어 참조
    /// </summary>
    private Transform player;
    
    /// <summary>
    /// 스프라이트 렌더러
    /// </summary>
    private SpriteRenderer spriteRenderer;

    // Properties
    /// <summary>
    /// 소환할 검 프리팹 프로퍼티
    /// </summary>
    public GameObject[] SwordPrefabs { get => swordPrefabs; set => swordPrefabs = value; }
    
    /// <summary>
    /// 수집 가능 범위 프로퍼티
    /// </summary>
    public float CollectionRange { get => collectionRange; set => collectionRange = value; }
    
    /// <summary>
    /// 수집 여부 프로퍼티
    /// </summary>
    public bool IsCollected { get => isCollected; private set => isCollected = value; }

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
        
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        Debug.Log("GroundSword initialized");
    }

    /// <summary>
    /// 매 프레임 업데이트
    /// </summary>
    private void Update()
    {
        if (isCollected || player == null) return;
        
        CheckPlayerDistance();
    }

    /// <summary>
    /// 플레이어와의 거리 확인
    /// </summary>
    private void CheckPlayerDistance()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        
        if (distance <= collectionRange)
        {
            CollectSword();
        }
    }

    /// <summary>
    /// 검 수집 및 소환
    /// </summary>
    private void CollectSword()
    {
        if (isCollected || swordPrefabs == null || swordPrefabs.Length == 0) return;
    
        isCollected = true;
    
        // 랜덤하게 검 종류 선택
        int randomIndex = Random.Range(0, swordPrefabs.Length);
        GameObject selectedSwordPrefab = swordPrefabs[randomIndex];
    
        if (selectedSwordPrefab == null)
        {
            Debug.LogWarning($"Sword prefab at index {randomIndex} is null!");
            return;
        }
    
        // 검 소환
        GameObject newSword = Instantiate(selectedSwordPrefab, transform.position, Quaternion.identity);
        SwordController swordController = newSword.GetComponent<SwordController>();
    
        if (swordController != null)
        {
            // 검을 바로 들어올리기 상태로 전환
            swordController.StartLift();
        }
    
        Debug.Log($"Sword type {randomIndex} collected and summoned!");
    
        // 땅에 박힌 검 오브젝트 제거
        Destroy(gameObject);
    }
    
    /// <summary>
    /// 트리거 충돌 처리 (백업용)
    /// </summary>
    /// <param name="other">충돌한 오브젝트</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            CollectSword();
        }
    }

    /// <summary>
    /// 기즈모 그리기 (수집 범위 표시)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, collectionRange);
    }
}
}