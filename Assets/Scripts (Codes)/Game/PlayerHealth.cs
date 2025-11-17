using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int playerLifes = 3;
    public PlayerLifesUI lifesUI;
    public AudioSource audioSource;
    public AudioClip damageSound;
    public float gameOverlayDelay = 3f;

    [Header("Invulnerability VFX")]
    [SerializeField] public float invulnerabilityDuration = 1f;
    [SerializeField] private float blinkInterval = 0.2f;

    private SpriteRenderer spriteRenderer;

    private bool isInvulnerable = false;

    void Start()
    {
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
        if (isInvulnerable) return;

        isInvulnerable = true;

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetCombo();
        }
        if (playerLifes <= 0) return;

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
            StartCoroutine(HandleDelayedDeath());
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.ResetCombo();
        }
        else
        {
            StartCoroutine(InvulnerabilityFlicker());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy1") || collision.CompareTag("EnemyLaser"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    public IEnumerator InvulnerabilityFlicker()
    {
        float startTime = Time.time;

        while (Time.time < startTime + invulnerabilityDuration)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(blinkInterval);
        }

        spriteRenderer.enabled = true;
        isInvulnerable = false;

    }

    IEnumerator HandleDelayedDeath()
    {
        spriteRenderer.enabled = false;

        yield return new WaitForSeconds(gameOverlayDelay);

        GameManager gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager != null)
        {
            GameManager.instance.InitiateGameOver();
        }

        Destroy(gameObject);
    }
}