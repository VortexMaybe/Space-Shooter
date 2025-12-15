using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] float spawnRate = 3f;
    [SerializeField] GameObject enemyPrefab;
    [SerializeField] GameObject enemyLaserPrefab;

    [Header("Spawn Control")]
    [SerializeField] float minDistanceX = 2f;

    float xMin;
    float xMax;
    float ySpawn;
    private float lastSpawnX = 0f;

    void Start()
    {
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0, 0)).x;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0, 0)).x;
        ySpawn = Camera.main.ViewportToWorldPoint(new Vector3(0, 1.25f, 0)).y;

        lastSpawnX = (xMin + xMax) / 2f;

        InvokeRepeating(nameof(SpawnEnemy), 1.5f, spawnRate);
    }

    void SpawnEnemy()
    {
        float randX;
        int maxAttempts = 10;
        int attempt = 0;

        do
        {
            randX = Random.Range(xMin, xMax);
            attempt++;

        } while (Mathf.Abs(randX - lastSpawnX) < minDistanceX && attempt < maxAttempts);

        lastSpawnX = randX;

        Vector3 enemyPos = new Vector3(randX, ySpawn, 0);

        GameObject enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
    }
}