using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] float spawnRate = 2f;
    [SerializeField] GameObject enemyPrefab;
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

        // Пускаме spawn цикъл
        InvokeRepeating(nameof(SpawnEnemyAndLaser), 1f, spawnRate);
    }

    void SpawnEnemyAndLaser()
    {
        float randX = Random.Range(xMin, xMax);
        Vector3 enemyPos = new Vector3(randX, ySpawn, 0);

        // Spawn-ваме врага
        GameObject enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);

        // Spawn-ваме лазера малко под врага
        if (enemyLaserPrefab != null)
        {
            Vector3 laserPos = enemyPos + Vector3.down * 0.5f;
            Instantiate(enemyLaserPrefab, laserPos, Quaternion.identity);
        }
    }
}
    
