using UnityEngine;

public class FallingObjectSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject obstaclePrefab;
    [SerializeField] private GameObject collectiblePrefab;

    [Header("Pool Settings")]
    [SerializeField] private int obstaclePoolSize = 15;
    [SerializeField] private int collectiblePoolSize = 5;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1f;

    [Tooltip("Probabilidad de que salga un obstáculo")]
    [Range(0f, 1f)]
    [SerializeField] private float obstacleChance = 0.2f;

    [Header("Spawn Area")]
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;

    [SerializeField] private float spawnY = 6f;

    private bool isSpawning = true;

    // Pools
    private ObjectPool obstaclePool;
    private ObjectPool collectiblePool;

    private float timer;

    // ------------------ Subscribe Events ------------------
    private void OnEnable()
    {
        ScoreEvents.FinishGame += FinishGame;
    }

    private void OnDisable()
    {
        ScoreEvents.FinishGame -= FinishGame;
    }

    private void Start()
    {
        obstaclePool = new ObjectPool(
            obstaclePrefab,
            obstaclePoolSize,
            new GameObject("ObstaclePool").transform
        );

        collectiblePool = new ObjectPool(
            collectiblePrefab,
            collectiblePoolSize,
            new GameObject("CollectiblePool").transform
        );
    }

    private void Update()
    {
        if (!isSpawning)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObject();
            timer = 0f;
        }
    }

    private void SpawnObject()
    {
        float randomX = Random.Range(minX, maxX);

        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        bool spawnObstacle = Random.value <= obstacleChance;

        GameObject obj;

        if (spawnObstacle)
        {
            obj = obstaclePool.Get();
        }
        else
        {
            obj = collectiblePool.Get();
        }

        obj.transform.position = spawnPosition;
        obj.SetActive(true);
    }

    private void FinishGame()
    {
        isSpawning = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Vector3 leftPoint = new Vector3(minX, spawnY, 0f);
        Vector3 rightPoint = new Vector3(maxX, spawnY, 0f);

        Gizmos.DrawLine(leftPoint, rightPoint);

        Gizmos.DrawSphere(leftPoint, 0.2f);
        Gizmos.DrawSphere(rightPoint, 0.2f);
    }

    public void SetSpawnInterval(float interval)
    {
        spawnInterval = Mathf.Max(0.01f, interval);
    }

    public void SetObstacleChance(float chance)
    {
        obstacleChance = Mathf.Clamp01(chance);
    }
}
