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
    [SerializeField] public float invulnerabilityDuration = 1.5f;
    [SerializeField] private float blinkInterval = 0.1f;

    [Header("Collision Layers")]
    [SerializeField] private int invulnerableLayer;
    private int defaultLayer;

    private SpriteRenderer spriteRenderer;

    private bool isInvulnerable = false;

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
       
        if (isInvulnerable) return;

        
        isInvulnerable = true;

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