using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public static PlayerMovement instance;

    [Header("Movement & Shooting")]
    public float speed = 5f;
    [SerializeField] float baseFireRate = 0.5f;
    private float fireRateModifier = 1f;
    float nextFireTime = 0f;
    [SerializeField] GameObject laser;

    [Header("Visual Effects")]
    public GameObject floatingTextPrefab;
    [SerializeField] GameObject overchargeEffectPrefab; // Сложи ЧЕРВЕНИЯ ефект тук в Инспектора
    private GameObject activeOverchargeEffect;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;

    private float originalSpeed;
    private float currentFireRate;
    private float activeFireRate;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        originalSpeed = speed;
        currentFireRate = baseFireRate;
        activeFireRate = baseFireRate;
    }

    void Update()
    {
        // Движение
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveX, moveY, 0f) * speed * Time.deltaTime;
        transform.position += movement;

        // Ограничение на екрана
        Camera cam = Camera.main;
        if (cam != null)
        {
            float camHalfHeight = cam.orthographicSize;
            float camHalfWidth = camHalfHeight * cam.aspect;
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(pos.x, cam.transform.position.x - camHalfWidth, cam.transform.position.x + camHalfWidth);
            pos.y = Mathf.Clamp(pos.y, cam.transform.position.y - camHalfHeight, cam.transform.position.y + camHalfHeight);
            transform.position = pos;
        }

        // Логика за стрелба
        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Instantiate(laser, transform.position, Quaternion.identity);
            nextFireTime = Time.time + activeFireRate;

            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }
    }

    void ApplyFireRateUpgrade(PowerUpTier tier)
    {
        float bonus = 0.15f;
        if (tier == PowerUpTier.Silver) bonus = 0.25f;
        if (tier == PowerUpTier.Gold) bonus = 0.35f;

        fireRateModifier += bonus;
        currentFireRate = baseFireRate / fireRateModifier;
        activeFireRate = currentFireRate;

        ShowFloatingText($"Fire Rate +{bonus * 100}%", Color.cyan);
    }

    public void ActivatePowerUp(PowerUpType type, PowerUpTier tier)
    {
        float effectMultiplier = 1f;
        switch (tier)
        {
            case PowerUpTier.Bronze: effectMultiplier = 1.0f; break;
            case PowerUpTier.Silver: effectMultiplier = 1.5f; break;
            case PowerUpTier.Gold: effectMultiplier = 2.0f; break;
        }

        PlayerHealth playerhealth = GetComponent<PlayerHealth>();

        switch (type)
        {
            case PowerUpType.RapidFire:
                ApplyFireRateUpgrade(tier);
                StartCoroutine(RapidFireRoutine(5f * effectMultiplier));
                break;

            case PowerUpType.Speed:
                StartCoroutine(SpeedBoostRoutine(5f * effectMultiplier, effectMultiplier));
                break;

            case PowerUpType.Recovery:
                if (tier == PowerUpTier.Gold) // ЧЕРВЕНО ХАПЧЕ
                {
                    if (playerhealth != null) playerhealth.Heal(1);
                    StartCoroutine(UltimateGoldRoutine(10f));
                    ShowFloatingText("ULTIMATE MODE!", Color.red);
                }
                else if (tier == PowerUpTier.Silver) // СРЕБЪРНО (Score)
                {
                    StartCoroutine(ScoreMultiplierRoutine(10f));
                    ShowFloatingText("SCORE X2!", Color.white);
                }
                else if (tier == PowerUpTier.Bronze) // БРОНЗ (Heal)
                {
                    if (playerhealth != null) playerhealth.Heal(1);
                    ShowFloatingText("+1 HP", Color.green);
                }
                break;

            case PowerUpType.MegaXP:
                int xpAmount = (tier == PowerUpTier.Gold) ? 100 : Random.Range(20, 50);
                if (tier == PowerUpTier.Gold)
                {
                    EnemyBase[] allEnemies = FindObjectsOfType<EnemyBase>();
                    foreach (EnemyBase enemy in allEnemies) enemy.Die();
                }
                ShowFloatingText($"+{xpAmount} XP", Color.yellow);
                ExperienceManager.instance.AddExperience(xpAmount);
                break;

            case PowerUpType.Shield:
                if (playerhealth != null) playerhealth.ActivateShield((tier == PowerUpTier.Gold) ? 3 : 1, tier);
                break;
        }
    }

    // --- COROUTINES ---

    IEnumerator RapidFireRoutine(float duration)
    {
        activeFireRate = currentFireRate / 2f;
        yield return new WaitForSeconds(duration);
        activeFireRate = currentFireRate;
    }

    IEnumerator UltimateGoldRoutine(float duration)
    {
        // Активираме всичко
        activeFireRate = currentFireRate / 2f;
        speed = originalSpeed * 1.5f;

        if (overchargeEffectPrefab != null && activeOverchargeEffect == null)
        {
            activeOverchargeEffect = Instantiate(overchargeEffectPrefab, transform.position, Quaternion.identity);
            activeOverchargeEffect.transform.SetParent(this.transform);

            // Фикс мащаб за твоя кораб (4.31, 4.12)
            activeOverchargeEffect.transform.localScale = new Vector3(1f / transform.localScale.x, 1f / transform.localScale.y, 1f);
            activeOverchargeEffect.transform.localPosition = new Vector3(0, -0.2f, 0);
        }

        yield return new WaitForSeconds(duration);

        // Връщаме всичко в норма
        activeFireRate = currentFireRate;
        speed = originalSpeed;
        if (activeOverchargeEffect != null)
        {
            Destroy(activeOverchargeEffect);
            activeOverchargeEffect = null;
        }
    }

    IEnumerator ScoreMultiplierRoutine(float duration)
    {
        // Тук сложи логика за Score Multiplier, ако имаш ScoreManager
        Debug.Log("Score Multiplier Active!");
        yield return new WaitForSeconds(duration);
        Debug.Log("Score Multiplier Ended.");
    }

    IEnumerator SpeedBoostRoutine(float duration, float multiplier)
    {
        speed = originalSpeed * (1f + (0.5f * multiplier));
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
    }

    void ShowFloatingText(string message, Color textColor)
    {
        if (floatingTextPrefab != null)
        {
            GameObject ft = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            FloatingText ftScript = ft.GetComponent<FloatingText>();
            if (ftScript != null) ftScript.Initialize(message, textColor);
        }
    }
}