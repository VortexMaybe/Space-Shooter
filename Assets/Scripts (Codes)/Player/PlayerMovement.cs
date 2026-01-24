using UnityEngine;
using System.Collections; // Трябва за Coroutines (Rapid Fire, Speed Boost)

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    public float speed = 5f;
    [SerializeField] float fireRate = 0.5f;
    float nextFireTime = 0f;
    [SerializeField] GameObject laser;

    // Аудио
    public AudioSource audioSource;
    public AudioClip shootSound;

    public GameObject floatingTextPrefab;
    private float originalSpeed;
    private float currentFireRate = 0.5f;

    // Инициализация на Singleton и оригиналните стойности
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Полезно, ако преминаваш през сцени
        }
        else
        {
            Destroy(gameObject);
        }

        originalSpeed = speed;
        currentFireRate = fireRate;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, moveY, 0f) * speed * Time.deltaTime;
        transform.position += movement;

        Camera cam = Camera.main;
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, cam.transform.position.x - camHalfWidth, cam.transform.position.x + camHalfWidth);
        pos.y = Mathf.Clamp(pos.y, cam.transform.position.y - camHalfHeight, cam.transform.position.y + camHalfHeight);

        transform.position = pos;

        // Логика за стрелба
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Instantiate(laser, transform.position, Quaternion.identity);
            nextFireTime = Time.time + fireRate;

            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }
    }

    void ShowFloatingText(string message, Color textColor)
    {
        // Проверяваме дали префабът за текст е закачен в Инспектора
        if (floatingTextPrefab != null)
        {
            // Създаваме текста на позицията на играча
            GameObject ft = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);

            // Вземаме скрипта на самия текст (предполагам се казва FloatingText)
            FloatingText ftScript = ft.GetComponent<FloatingText>();

            if (ftScript != null)
            {
                // Извикваме неговия метод за инициализация (с твоите параметри)
                ftScript.Initialize(message, textColor);
            }
        }
        else
        {
            Debug.LogWarning("Липсва floatingTextPrefab в Player скрипта!");
        }
    }

    void ApplyFireRateUpgrade(PowerUpTier tier)
    {
        float reduction = 0.05f; // Бронз: -5% време между изстрелите
        if (tier == PowerUpTier.Silver) reduction = 0.12f;
        if (tier == PowerUpTier.Gold) reduction = 0.25f;

        currentFireRate -= currentFireRate * reduction;
        currentFireRate = Mathf.Max(currentFireRate, 0.1f); // Лимит, за да не стане прекалено бързо
    }

    // ====================================================================
    // 4. POWER-UP АКТИВАЦИЯ
    // ====================================================================

    // ВАЖНО: PowerUpType и PowerUpTier трябва да бъдат дефинирани 
    // в PowerUp.cs ИЗВЪН класа, за да могат да се виждат тук.
    public void ActivatePowerUp(PowerUpType type, PowerUpTier tier)
    {
        float effectMultiplier = 1f;

        // Изчисляваме мултипликатора въз основа на качеството (Tier)
        switch (tier)
        {
            case PowerUpTier.Bronze: effectMultiplier = 1.0f; break;
            case PowerUpTier.Silver: effectMultiplier = 1.5f; break;
            case PowerUpTier.Gold: effectMultiplier = 2.0f; break;
        }

        PlayerHealth playerhealth = GetComponent<PlayerHealth>();

        // Прилагаме ефекта
        switch (type)
        {
            case PowerUpType.RapidFire:
                // Увеличава продължителността на ефекта
                StartCoroutine(RapidFireRoutine(5f * effectMultiplier));
                break;

            case PowerUpType.Speed:
                // Увеличава продължителността и силата на ефекта
                StartCoroutine(SpeedBoostRoutine(5f * effectMultiplier, effectMultiplier));
                break;

            case PowerUpType.Recovery:
                int healAmount = Mathf.RoundToInt(1 * effectMultiplier);
                if (playerhealth != null)
                {
                    playerhealth.Heal(healAmount);
                }
                Debug.Log($"Възстановяване на HP: {Mathf.RoundToInt(1 * effectMultiplier)}");
                break;

            case PowerUpType.MegaXP:
                int xpAmount = 0;
                if (tier == PowerUpTier.Bronze) xpAmount = Random.Range(20, 21); // Твърдо 20
                else if (tier == PowerUpTier.Silver) xpAmount = Random.Range(50, 71);
                else if (tier == PowerUpTier.Gold)
                {
                    // 1. Светкавица на екрана (можеш да ползваш бял Image в UI и да го пуснеш за кратко)
                    // 2. Унищожаване на всички врагове
                    EnemyBase[] allEnemies = FindObjectsOfType<EnemyBase>();
                    foreach (EnemyBase enemy in allEnemies)
                    {
                        enemy.Die(); // Това ще ти даде техния score автоматично
                    }
                    xpAmount = 100;
                }

                // Показване на Floating Text (различен цвят)
                ShowFloatingText($"+{xpAmount} XP", Color.yellow);
                ExperienceManager.instance.AddExperience(xpAmount);
                break;

            case PowerUpType.Shield:
                int baseHits = 1;
                int hits = (tier == PowerUpTier.Gold) ? 3 : Mathf.RoundToInt(baseHits * effectMultiplier);
                if (playerhealth != null)
                {
                    playerhealth.ActivateShield(hits, tier);
                }
                PlayerHealth playerHealth = GetComponent<PlayerHealth>();
                break;
        }
    }

    // ====================================================================
    // 5. COROUTINES ЗА ВРЕМЕННИ ЕФЕКТИ
    // ====================================================================

    IEnumerator RapidFireRoutine(float duration)
    {
        // Намаляваме интервала за стрелба (правим го по-бърз)
        // Bronze: 0.5s -> 0.25s, Silver: 0.5s -> 0.16s, Gold: 0.5s -> 0.125s (примерно)
        float rapidFireRate = currentFireRate / 4f;

        // Ако вече е активен, рестартираме Coroutine-а, за да обновим продължителността
        if (fireRate < currentFireRate)
        {
            // Ако вече е в режим RapidFire, просто продължаваме
        }
        else
        {
            fireRate = rapidFireRate; // Активираме Rapid Fire
        }

        yield return new WaitForSeconds(duration);

        // Връщаме оригиналната скорост на стрелба
        fireRate = currentFireRate;
    }

    IEnumerator SpeedBoostRoutine(float duration, float multiplier)
    {
        float speedIncrease = 1.5f; // Повишава скоростта с 50%

        // Временно увеличаваме скоростта
        float newSpeed = originalSpeed * (1f + (speedIncrease * (multiplier / 2f)));
        speed = newSpeed;

        yield return new WaitForSeconds(duration);

        // Връщаме оригиналната скорост
        speed = originalSpeed;
    }
}