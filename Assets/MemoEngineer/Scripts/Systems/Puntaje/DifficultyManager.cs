using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [Header("Referencia")]
    [SerializeField] private FallingObjectSpawner spawner;

    [Header("Frecuencia (spawnInterval)")]
    [SerializeField] private float initialSpawnInterval = 1f;
    [SerializeField] private float minSpawnInterval = 0.3f;
    [SerializeField] private float spawnIntervalDecreasePerScore = 0.02f;

    [Header("Probabilidad obstáculo (obstacleChance)")]
    [Range(0f, 1f)]
    [SerializeField] private float obstacleChanceBase = 0.2f;
    [SerializeField] private float obstacleChanceMax = 0.6f;
    [SerializeField] private float obstacleChanceIncreasePerScore = 0.005f;

    private void OnEnable()
    {
        ScoreEvents.ScoreChanged += OnScoreChanged;
    }

    private void OnDisable()
    {
        ScoreEvents.ScoreChanged -= OnScoreChanged;
    }

    private void Start()
    {
        if (spawner != null)
        {
            spawner.SetSpawnInterval(initialSpawnInterval);
            spawner.SetObstacleChance(obstacleChanceBase);
        }
    }

    private void OnScoreChanged(int score)
    {
        if (spawner == null) return;

        float newInterval = Mathf.Max(minSpawnInterval, initialSpawnInterval - score * spawnIntervalDecreasePerScore);

        float newChance = Mathf.Min(obstacleChanceMax, obstacleChanceBase + score * obstacleChanceIncreasePerScore);

        spawner.SetSpawnInterval(newInterval);
        spawner.SetObstacleChance(newChance);
    }
}
