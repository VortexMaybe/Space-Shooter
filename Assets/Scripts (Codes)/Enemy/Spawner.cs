using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] float spawnRate = 2f;
    [SerializeField] GameObject enemyPrefab;
    // Оставяме го, но не се използва, защото EnemySpawn управлява лазерите
    [SerializeField] GameObject enemyLaserPrefab;

    // ✅ НОВА НАСТРОЙКА: Минимално разстояние между две последователни появявания
    [Header("Spawn Control")]
    [SerializeField] float minDistanceX = 1.5f;

    float xMin;
    float xMax;
    float ySpawn;
    private float lastSpawnX = 0f; // ✅ НОВА: Следи последната X позиция

    void Start()
    {
        // Определяме границите на екрана
        // Използваме 0.1f и 0.9f, за да оставим малко padding
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0, 0)).x;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0, 0)).x;
        ySpawn = Camera.main.ViewportToWorldPoint(new Vector3(0, 1.25f, 0)).y;

        // Задаваме начална стойност на lastSpawnX
        lastSpawnX = (xMin + xMax) / 2f;

        InvokeRepeating(nameof(SpawnEnemy), 1f, spawnRate);
    }

    void SpawnEnemy()
    {
        float randX;
        int maxAttempts = 10; // Предпазен механизъм, за да не зацикли
        int attempt = 0;

        // ✅ КЛЮЧОВА ЛОГИКА: Цикъл, който търси X позиция, достатъчно далеч от последната
        do
        {
            randX = Random.Range(xMin, xMax);
            attempt++;

            // Проверява дали разликата е по-голяма от минималното разстояние
        } while (Mathf.Abs(randX - lastSpawnX) < minDistanceX && attempt < maxAttempts);

        // Ако сме намерили достатъчно отдалечена позиция, или сме стигнали макс. опити,
        // използваме тази позиция и я записваме като последна.
        lastSpawnX = randX;

        Vector3 enemyPos = new Vector3(randX, ySpawn, 0);

        // Spawn-ваме врага на случайна X позиция
        GameObject enemy = Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
    }
}