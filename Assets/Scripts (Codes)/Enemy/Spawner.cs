using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] float spawnRate = 2f;
    [SerializeField] GameObject enemyPrefab;
    // Оставяме го, но не се използва, защото EnemySpawn управлява лазерите
    [SerializeField] GameObject enemyLaserPrefab;

    float xMin;
    float xMax;
    float ySpawn;

    void Start()
    {
        // Определяме границите на екрана
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0, 0)).x;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0, 0)).x;
        ySpawn = Camera.main.ViewportToWorldPoint(new Vector3(0, 1.25f, 0)).y;

        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnRate);
    }

    void SpawnEnemy()
    {
        float randX = Random.Range(xMin, xMax);
        Vector3 enemyPos = new Vector3(randX, ySpawn, 0);

        // Spawn-ваме врага на случайна X позиция
        GameObject enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
    }
}