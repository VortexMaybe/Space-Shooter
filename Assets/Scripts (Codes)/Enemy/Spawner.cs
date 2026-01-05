using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    // Вече е деклариран, добавяме логика в Awake()
    public static Spawner instance;

    [Header("Spawn Rate Control")]
    [SerializeField] float initialSpawnRate = 3f; // Става initial
    private float currentSpawnRate; //  Променя се в движение
    private int currentWorldLevel = 1; //  Следи нивото на трудност

    [Header("Enemy Prefabs")]
    [SerializeField] GameObject defaultEnemyPrefab;
    [SerializeField] GameObject tankEnemyPrefab;
    [SerializeField] GameObject sniperEnemyPrefab;
    [SerializeField] GameObject fastEnemyPrefab;
    [SerializeField] GameObject turretEnemyPrefab;

    [Header("Spawn Control")]
    [SerializeField] float minDistanceX = 2f;

    [Header("Enemy Distribution")]
    [Range(0.0f, 1.0f)]
    [SerializeField] float maxSpecialEnemyChance = 0.5f;

    float xMin;
    float xMax;
    float ySpawn;
    private float lastSpawnX = 0f;

    // Добавяме Awake() за Singleton инициализация
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Инициализация на границите
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(0.1f, 0, 0)).x;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(0.9f, 0, 0)).x;
        ySpawn = Camera.main.ViewportToWorldPoint(new Vector3(0, 1.25f, 0)).y;

        lastSpawnX = (xMin + xMax) / 2f;

        currentSpawnRate = initialSpawnRate; // Задаваме началната скорост

        // Използваме Coroutine вместо InvokeRepeating
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// Извиква се от LevelManager, за да увеличи трудността.
    /// </summary>
    public void UpdateDifficulty(int newWorldLevel)
    {
        currentWorldLevel = newWorldLevel;

        // Намаляваме интервала за спаунване (Играта става по-интензивна)
        // Интервалът не трябва да пада под 0.5 секунди
        currentSpawnRate = Mathf.Max(0.5f, initialSpawnRate - (currentWorldLevel * 0.25f));

        Debug.Log($"[Spawner] Трудност актуализирана: World Level {currentWorldLevel}. Spawn Rate: {currentSpawnRate:F2}s");
    }

    // Замества InvokeRepeating, за да можем да променяме времето в движение
    IEnumerator SpawnRoutine()
    {
        // Изчакваме първоначалния старт
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            // Изчакваме текущия интервал (който UpdateDifficulty променя)
            yield return new WaitForSeconds(currentSpawnRate);

            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        // ... (Твоята логика за избор на X позиция) ...
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

        // Избор на враг според World Level
        GameObject enemyToInstantiate = SelectEnemyByDifficulty();

        if (enemyToInstantiate != null)
        {
            Instantiate(enemyToInstantiate, enemyPos, Quaternion.identity);
        }
    }

    // Избира кой враг да спаунне
    GameObject SelectEnemyByDifficulty()
    {
        float progressionFactor = Mathf.Min(1f, (float)currentWorldLevel / 10f);
        float specialEnemyChance = progressionFactor * maxSpecialEnemyChance;

        // Ако не сме достигнали ниво, на което да пускаме специални врагове, или шансът не е сработил
        if (currentWorldLevel < 3)
        {
            specialEnemyChance = 0f;
        }

        if (Random.value < specialEnemyChance)
        {
            // Шансът е сработил: Пускаме един от 4-те специални врага

            // ВАЖНО: Тук ще добавим логика за да се уверим, че поне един от Prefab-ите не е null
            if (tankEnemyPrefab == null && sniperEnemyPrefab == null && fastEnemyPrefab == null && turretEnemyPrefab == null)
            {
                Debug.LogError("Няма зададени специални врагове в Spawner Prefabs! Връщаме Default Enemy.");
                return defaultEnemyPrefab;
            }

            int randomIndex = Random.Range(0, 4);

            switch (randomIndex)
            {
                case 0: return tankEnemyPrefab;
                case 1: return sniperEnemyPrefab;
                case 2: return fastEnemyPrefab;
                case 3: return turretEnemyPrefab;
                default: return defaultEnemyPrefab;
            }
        }

        return defaultEnemyPrefab;
    }
}