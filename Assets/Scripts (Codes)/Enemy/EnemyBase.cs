using System.Collections;
using UnityEngine;
using TMPro;
using JetBrains.Annotations;

public class EnemyBase : MonoBehaviour
{
    // *** Общи Променливи (Споделени) ***

    public AudioSource audioSource;
    [SerializeField] AudioClip deathSound;
    [SerializeField] float customVolume = 5.6f;

    [Header("Visual Effects")]
    [SerializeField] protected GameObject deathEffectPrefab; // Тук ще сложиш твоята експлозия

    [Header("Health")]
    [SerializeField] protected int maxHealth = 2; // Използваме 'protected' за достъп от наследниците
    [SerializeField] private float hitFlickerDuration = 0.1f;

    [Header("Movement (Base)")]
    // Запазваме някои общи променливи
    [SerializeField] protected float rotationLerpSpeed = 1.5f;
    [SerializeField] private float spawnInvulnerabilityTime = 1.0f;

    [Header("Experience & Score")]
    [SerializeField] protected int minExperience = 5;
    [SerializeField] protected int maxExperience = 13;
    [SerializeField] protected int baseScoreValue = 10;
    [SerializeField] protected GameObject floatingTextPrefab;

    [Header("XP & Loot Settings")]
    [SerializeField] int baseXPValue = 5;
    [SerializeField] protected float dropChance = 0.2f;
    [SerializeField] GameObject[] powerupPrefabs;

    // [Вътрешни Променливи]
    protected int currentHealth;
    protected Transform playerTransform; // Правим го protected
    private bool canBeHit = false;
    private SpriteRenderer spriteRenderer;

    // Граници (Могат да се ползват от наследниците)
    protected float lowerBoundY;


    protected virtual void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Инициализация на границите
        lowerBoundY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - 3.5f;

        transform.rotation = Quaternion.identity;

        StartCoroutine(EnableHitAfterDelay());

        // ВАЖНО: Тук НЕ стартираме ShootRoutine или Update, тъй като всеки враг ще го прави по различен начин
    }

    // Добавяме празен Update() метод, който наследниците ще заместят (override)
    protected virtual void Update()
    {
        // Всички врагове проверяват дали са излезли от екрана
        if (transform.position.y < lowerBoundY)
        {
            Destroy(gameObject);
        }
    }


    // *** Методи за Унищожение и Щети (Обща Логика) ***

    IEnumerator EnableHitAfterDelay()
    {
        yield return new WaitForSeconds(spawnInvulnerabilityTime);
        canBeHit = true;
    }

    protected IEnumerator ShootRoutine(float interval, GameObject laserPrefab)
    {
        // Изчакваме малко, за да не стрелят веднага
        yield return new WaitForSeconds(interval / 2);

        while (true)
        {
            yield return new WaitForSeconds(interval);
            ShootLaser(laserPrefab);
        }
    }

    protected void ShootLaser(GameObject laserPrefab)
    {
        if (laserPrefab != null)
        {
            // Предполага се, че лазерът се създава с ротацията на кораба
            Instantiate(laserPrefab, transform.position, transform.rotation);
        }
    }

    public void EnemyDestroyedByPlayerLaser()
    {
        if (!canBeHit) return;

        currentHealth--;

        if (currentHealth > 0)
        {
            StartCoroutine(FlickerOnHit());
            return;
        }

        // --- ТОЧКАТА НА СМЪРТТА ---

        // 1. Изчисляваме XP веднъж и го показваме на екрана
        int experienceGained = UnityEngine.Random.Range(minExperience, maxExperience);

        if (ExperienceManager.instance != null)
        {
            ExperienceManager.instance.AddExperience(experienceGained);
        }

        // 2. Показваме текста (Floating Text)
        if (floatingTextPrefab != null)
        {
            GameObject ft = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            FloatingText ftScript = ft.GetComponent<FloatingText>();
            if (ftScript != null) ftScript.Initialize(experienceGained, Color.cyan);
        }

        // 3. Score и Звук
        if (GameManager.instance != null) GameManager.instance.AddScore(baseScoreValue);
        if (deathSound != null) AudioSource.PlayClipAtPoint(deathSound, transform.position, customVolume);

        if (deathEffectPrefab != null)
        {
            GameObject effect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        // 4. Пускаме Loot и унищожаваме врага
        HandlePowerUpDrop();
        Destroy(gameObject);
    }


    public void Die()
    {
        HandlePowerUpDrop();
        Destroy(gameObject);
    }

    void HandlePowerUpDrop()
    {
        Debug.Log("--- Проверка за Power-Up Drop ---"); // 1. Дали методът изобщо се вика?

        if (LevelManager.instance == null)
        {
            Debug.LogError("ГРЕШКА: LevelManager липсва в сцената!");
            return;
        }

        if (powerupPrefabs == null || powerupPrefabs.Length == 0)
        {
            Debug.LogWarning("ПРЕДУПРЕЖДЕНИЕ: Списъкът powerupPrefabs е ПРАЗЕН в Inspector-а!");
            return;
        }

        int pickupsUsed = LevelManager.instance.currentPickupsUsed;
        int maxPickups = LevelManager.instance.maxPickupsPerPhase;

        if (pickupsUsed >= maxPickups)
        {
            Debug.Log("Power-Up НЕ падна: Лимитът за фазата е достигнат (" + pickupsUsed + "/" + maxPickups + ")");
            return;
        }

        float rollChance = Random.value;
        if (rollChance > dropChance)
        {
            Debug.Log("Power-Up НЕ падна: Късметът не проработи. Шанс: " + dropChance + ", Твоето число: " + rollChance);
            return;
        }

        // Ако стигне до тук, значи трябва да се спаунне!
        Debug.Log("УСПЕХ: Спаунваме Power-Up!");

        GameObject chosenPrefab = powerupPrefabs[Random.Range(0, powerupPrefabs.Length)];
        GameObject powerUpInstance = Instantiate(chosenPrefab, transform.position, Quaternion.identity);

        PowerUp pu = powerUpInstance.GetComponent<PowerUp>();
        if (pu != null)
        {
            // Изчисляваме Tier
            float roll = Random.value * 100f;
            if (roll <= 5f) pu.tier = PowerUpTier.Gold;
            else if (roll <= 40f) pu.tier = PowerUpTier.Silver;
            else pu.tier = PowerUpTier.Bronze;

            pu.SetVisuals(pu.tier);
            LevelManager.instance.currentPickupsUsed++;
        }
    }

    protected IEnumerator FlickerOnHit()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.4f);

        yield return new WaitForSeconds(hitFlickerDuration);

        spriteRenderer.color = originalColor;
    }
}
