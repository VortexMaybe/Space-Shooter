using System.Collections;
using UnityEngine;
using TMPro; // Запазваме го за Floating Text

public class EnemyBase : MonoBehaviour
{
    // *** Общи Променливи (Споделени) ***

    public AudioSource audioSource;
    [SerializeField] AudioClip deathSound;
    [SerializeField] float customVolume = 5.6f;

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

    // [Вътрешни Променливи]
    protected int currentHealth;
    protected Transform playerTransform; // Правим го protected
    private bool canBeHit = false;
    private SpriteRenderer spriteRenderer;

    // Граници (Могат да се ползват от наследниците)
    protected float lowerBoundY;

    // --------------------------------------------------------------------------------

    // ✅ Започваме с 'protected virtual Start()'
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

    // ✅ Добавяме празен Update() метод, който наследниците ще заместят (override)
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

    public void EnemyDestroyedByPlayerLaser()
    {
        if (!canBeHit) return;

        currentHealth--;

        if (currentHealth > 0)
        {
            StartCoroutine(FlickerOnHit());
            return;
        }

        // Logic за Score и XP (запазена)
        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(baseScoreValue);
        }

        ExperienceManager expManager = FindAnyObjectByType<ExperienceManager>();
        if (expManager != null)
        {
            int experienceGained = UnityEngine.Random.Range(minExperience, maxExperience);
            expManager.AddExperience(experienceGained);

            if (floatingTextPrefab != null)
            {
                GameObject ft = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
                FloatingText ftScript = ft.GetComponent<FloatingText>();

                if (ftScript != null)
                {
                    ftScript.Initialize(experienceGained, Color.cyan);
                }
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