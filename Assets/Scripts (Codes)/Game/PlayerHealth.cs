using System.Collections;
using UnityEngine;
using static PowerUp;

public class PlayerHealth : MonoBehaviour
{
    public int playerLifes = 3;
    public PlayerLifesUI lifesUI;
    public AudioSource audioSource;
    public AudioClip damageSound;
    public AudioClip healSound;
    public float gameOverlayDelay = 3f;

    [Header("Invulnerability VFX")]
    [SerializeField] public float invulnerabilityDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;


    [Header("Collision Layers")]
    [SerializeField] private int invulnerableLayer;
    private int defaultLayer;

    private SpriteRenderer spriteRenderer;

    private bool isInvulnerable = false;

    [Header("Shield Power-Up Visuals")]
    [SerializeField] GameObject shieldVisualBronze; // Визия за 1 удар (Shield I)
    [SerializeField] GameObject shieldVisualSilver; // Визия за 2 удара (Shield II)
    [SerializeField] GameObject shieldVisualGold;   // Визия за 3 удара (Shield III)

    private int shieldHitsRemaining = 0;

    void Start()
    {
        defaultLayer = gameObject.layer;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (lifesUI == null)
        {
            lifesUI = FindAnyObjectByType<PlayerLifesUI>();
        }

        if (lifesUI != null)
        {
            lifesUI.UpdateLifes(playerLifes);
        }
    }

    public void TakeDamage(int amount)
    {
        // 1. ПРОВЕРКА НА ЩИТА
        if (shieldHitsRemaining > 0)
        {
            shieldHitsRemaining -= 1;
            Debug.Log($"Щитът пое удара. Остават {shieldHitsRemaining} удара.");

            if (shieldHitsRemaining <= 0)
            {
                // Щитът е унищожен, деактивираме всички визии
                if (shieldVisualBronze != null) shieldVisualBronze.SetActive(false);
                if (shieldVisualSilver != null) shieldVisualSilver.SetActive(false);
                if (shieldVisualGold != null) shieldVisualGold.SetActive(false);
            }

            // Възпроизвеждане на звук за удар по щита
            if (audioSource != null && damageSound != null)
            {
                audioSource.PlayOneShot(damageSound);
            }

            return;
        }

        if (isInvulnerable) return;

        isInvulnerable = true;

        if (CameraShake.instance != null)
        {
            CameraShake.instance.Shake(0.2f, 0.3f);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetCombo();
        }

        playerLifes -= amount;

        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (lifesUI != null)
        {
            lifesUI.UpdateLifes(playerLifes);
        }

        if (playerLifes <= 0)
        {
            StopAllCoroutines();
            StartCoroutine(HandleDelayedDeath());
        }
        else
        {
            StartCoroutine(InvulnerabilityFlicker());
        }
    }

    public void ActivateShield(int maxHits, PowerUpTier tier)
    {
        shieldHitsRemaining = maxHits;

        shieldVisualBronze.SetActive(false);
        shieldVisualSilver.SetActive(false);
        shieldVisualGold.SetActive(false);

        switch (tier)
        {
            case PowerUpTier.Bronze:
                if (shieldVisualBronze != null) shieldVisualBronze.SetActive(true);
                break;
            case PowerUpTier.Silver:
                if (shieldVisualSilver != null) shieldVisualSilver.SetActive(true);
                break;
            case PowerUpTier.Gold:
                if (shieldVisualGold != null) shieldVisualGold.SetActive(true);
                break;
        }

        Debug.Log($"Щит {tier} активиран! Остават {shieldHitsRemaining} удара.");
    }

    public void Heal(int amount)
    {
        playerLifes += amount;

        playerLifes = Mathf.Min(playerLifes, 3);

        if (lifesUI != null)
        {
            lifesUI.UpdateLifes(playerLifes);
        }

        if (healSound != null)
        {
            AudioSource.PlayClipAtPoint(healSound, Camera.main.transform.position, 1f);
        }

        Debug.Log($"Животът е възстановен с {amount}. Текущ живот: {playerLifes}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy1") || collision.CompareTag("EnemyLaser"))
        {
            if (isInvulnerable) return;
            Destroy(collision.gameObject);
            TakeDamage(1);
        }
    }

    public IEnumerator InvulnerabilityFlicker()
    {
        float startTime = Time.time;

        gameObject.layer = invulnerableLayer;
        try
        {
            while (Time.time < startTime + invulnerabilityDuration)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled;
                yield return new WaitForSeconds(blinkInterval);
            }
        }
        finally
        {
            gameObject.layer = defaultLayer;
            spriteRenderer.enabled = true;
            isInvulnerable = false;
        }
    }

    IEnumerator HandleDelayedDeath()
    {
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(gameOverlayDelay);

        if (GameManager.instance != null)
        {
            GameManager.instance.InitiateGameOver();
        }

        Destroy(gameObject);
    }
}