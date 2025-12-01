using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawn : MonoBehaviour
{
    // [Звук и Общи]
    public AudioSource audioSource;
    [SerializeField] AudioClip deathSound;
    [SerializeField] float customVolume = 5.6f;

    // [Живот]
    [Header("Health")]
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private float hitFlickerDuration = 0.1f;

    // [Движение]
    [Header("Movement")]
    [SerializeField] float verticalSpeed = 3f;
    [SerializeField] float maxMovementAngle = 35f;
    [SerializeField] float rotationLerpSpeed = 1.5f;
    [SerializeField] float spawnInvulnerabilityTime = 1.0f;

    [Header("Shooting Settings")]
    [SerializeField] GameObject enemyLaserPrefab;
    [SerializeField] float shootInterval = 2.5f;

    [Header("Experience & Score")]
    [SerializeField] private int minExperience = 5;
    [SerializeField] private int maxExperience = 13;
    [SerializeField] private int baseScoreValue = 10;
    [SerializeField] private GameObject floatingTextPrefab;

    // [Вътрешни Променливи]
    private int currentHealth;
    private bool canBeHit = false;
    private SpriteRenderer spriteRenderer;

    // Граници
    private float lowerBoundY;
    private float leftBoundX;
    private float rightBoundX;

    private Vector3 movementDirection;
    private Vector3 targetDirection;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 1. ✅ ИЗЧИСЛЯВАНЕ НА ГРАНИЦИ (КОРИГИРАНО):
        // Долна граница: Остава широка (3.5f) за унищожение
        lowerBoundY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - 3.5f;

        // Лява граница: Намаляваме буфера до 0.1f, за да отскача по-рано.
        leftBoundX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - 0.1f;

        // Дясна граница: Намаляваме буфера до 0.1f, за да отскача по-рано.
        rightBoundX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + 0.1f;

        // 2. ДЕФИНИРАНЕ НА ЦЕЛЕВИЯ ЪГЪЛ
        float randomZRotation = Random.Range(-maxMovementAngle, maxMovementAngle);
        targetDirection = Quaternion.Euler(0, 0, randomZRotation) * Vector3.down;

        // 3. НАЧАЛНА ПОСОКА: ПРАВО НАДОЛУ (0 градуса)
        movementDirection = Vector3.down;
        transform.rotation = Quaternion.identity;

        StartCoroutine(EnableHitAfterDelay());
        StartCoroutine(ShootRoutine());
    }

    void Update()
    {
        // 1. ПЛАВНО ПРЕМИНАВАНЕ КЪМ ЦЕЛЕВИЯ ЪГЪЛ
        movementDirection = Vector3.Lerp(movementDirection, targetDirection, rotationLerpSpeed * Time.deltaTime);

        // 2. КОРЕКЦИЯ НА ПОСОКАТА (ОТБЛЪСКВАНЕ)
        bool hitLeft = transform.position.x < leftBoundX;
        bool hitRight = transform.position.x > rightBoundX;

        if (hitLeft || hitRight)
        {
            // Обръщаме хоризонталната посока
            movementDirection.x = -movementDirection.x;

            // Преместваме врага обратно във видимата зона с един кадър,
            // за да предотвратим засядане.
            if (hitLeft)
            {
                // Позиционираме го точно на границата
                transform.position = new Vector3(leftBoundX, transform.position.y, 0);
            }
            else // hitRight
            {
                // Позиционираме го точно на границата
                transform.position = new Vector3(rightBoundX, transform.position.y, 0);
            }

            // Актуализираме целевата посока (targetDirection)
            targetDirection.x = -targetDirection.x;
        }


        // 3. ЗАВЪРТАНЕ НА СПРАЙТА (плавно)
        if (movementDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            // Адаптираме ъгъла (добавяме 90) за да сочи надолу
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                                                         rotationLerpSpeed * 10f * Time.deltaTime);
        }

        // 4. ДВИЖЕНИЕ В ТЕКУЩАТА ПОСОКА
        transform.position += movementDirection * verticalSpeed * Time.deltaTime;

        // 5. ПРОВЕРКА ЗА УНИЩОЖЕНИЕ (Само отдолу!)
        if (transform.position.y < lowerBoundY)
        {
            Destroy(gameObject);
        }
    }

    // ... (Останалите методи) ...

    IEnumerator EnableHitAfterDelay()
    {
        yield return new WaitForSeconds(spawnInvulnerabilityTime);
        canBeHit = true;
    }

    IEnumerator ShootRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(shootInterval);
            ShootLaser();
        }
    }

    void ShootLaser()
    {
        if (enemyLaserPrefab != null)
        {
            Instantiate(enemyLaserPrefab, transform.position, Quaternion.identity);
        }
    }

    public void EnemyDestroyedByPlayerLaser()
    {
        if (!canBeHit) return;

        currentHealth--;

        if (currentHealth > 0)
        {
            StartCoroutine(FlickerOnHit());

            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(1);
            }
            return;
        }

        // --- ЛОГИКА ЗА СМЪРТ ---

        int scoreAdded = 0;
        if (GameManager.instance != null)
        {
            scoreAdded = GameManager.instance.AddScore(baseScoreValue);
        }

        ExperienceManager expManager = FindAnyObjectByType<ExperienceManager>();
        if (expManager != null)
        {
            int experienceGained = UnityEngine.Random.Range(minExperience, maxExperience);
            expManager.AddExperience(experienceGained);
        }

        if (floatingTextPrefab != null)
        {
            GameObject ft = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            FloatingText ftScript = ft.GetComponent<FloatingText>();

            if (ftScript != null)
            {
                ftScript.Initialize(scoreAdded, Color.yellow);
            }
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, customVolume);
        }

        Destroy(gameObject);
    }

    IEnumerator FlickerOnHit()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.4f);

        yield return new WaitForSeconds(hitFlickerDuration);

        spriteRenderer.color = originalColor;
    }
}