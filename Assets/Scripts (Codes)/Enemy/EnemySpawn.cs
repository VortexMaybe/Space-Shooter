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
    [SerializeField] float sinFrequency = 1.5f;
    [SerializeField] float sinMagnitude = 1.0f;
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
    private float initialX;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        currentHealth = maxHealth;
        initialX = transform.position.x;
        // КРИТИЧНО: Взимаме SpriteRenderer за примигване. Увери се, че врагът има такъв компонент!
        spriteRenderer = GetComponent<SpriteRenderer>();

        StartCoroutine(EnableHitAfterDelay());
        StartCoroutine(ShootRoutine());
    }

    void Update()
    {
        // 1. Вертикално Движение
        transform.position -= new Vector3(0, verticalSpeed, 0) * Time.deltaTime;

        // 2. Синусоидално (Вълнообразно) Движение
        float newX = initialX + Mathf.Sin(Time.time * sinFrequency) * sinMagnitude;
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
    }

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
            // ВРАГЪТ Е РАНЕН: Примигва и дава малко точки
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

        int experienceGained = 0;
        ExperienceManager expManager = FindAnyObjectByType<ExperienceManager>();
        if (expManager != null)
        {
            experienceGained = UnityEngine.Random.Range(minExperience, maxExperience);
            expManager.AddExperience(experienceGained);
        }

        if (floatingTextPrefab != null)
        {
            GameObject ft = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            FloatingText ftScript = ft.GetComponent<FloatingText>();

            if (ftScript != null)
            {
                ftScript.Initialize(experienceGained, Color.magenta);
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