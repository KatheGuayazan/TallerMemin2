using UnityEngine;

public class FallingObjectSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject obstaclePrefab; // Objeto que el jugador esquiva
    [SerializeField] private GameObject collectiblePrefab; // Objeto que recolecta

    [Header("Pool Settings")]
    [SerializeField] private int obstaclePoolSize = 15;
    [SerializeField] private int collectiblePoolSize = 5;

    [Header("Spawn Settings")]
    [SerializeField] private float spawnInterval = 1f;

    [Tooltip("Probabilidad de que salga el collectible")]
    [Range(0f, 1f)]
    [SerializeField] private float collectibleChance = 0.2f;

    [Header("Spawn Area")]
    [SerializeField] private float minX = -5f;
    [SerializeField] private float maxX = 5f;

    [SerializeField] private float spawnY = 6f;

    // Pools
    private ObjectPool obstaclePool;
    private ObjectPool collectiblePool;

    private float timer;

    // ------------------ Initializes pools ------------------
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

    // ------------------ Handles timed spawning ------------------
    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnObject();
            timer = 0f;
        }
    }

    // ------------------ Spawns either obstacle or collectible ------------------
    private void SpawnObject()
    {
        float randomX = Random.Range(minX, maxX);

        Vector3 spawnPosition = new Vector3(randomX, spawnY, 0f);

        bool spawnCollectible = Random.value <= collectibleChance;

        GameObject obj;

        if (spawnCollectible)
        {
            obj = collectiblePool.Get();
        }
        else
        {
            obj = obstaclePool.Get();
        }

        obj.transform.position = spawnPosition;
    }

    // ------------------ Draws spawn range in Scene view ------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;

        Vector3 leftPoint = new Vector3(minX, spawnY, 0f);
        Vector3 rightPoint = new Vector3(maxX, spawnY, 0f);

        Gizmos.DrawLine(leftPoint, rightPoint);

        Gizmos.DrawSphere(leftPoint, 0.2f);
        Gizmos.DrawSphere(rightPoint, 0.2f);
    }
}