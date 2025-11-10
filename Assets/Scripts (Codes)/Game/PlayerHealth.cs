using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int playerLifes = 3;
    public PlayerLifesUI lifesUI;
    public AudioSource audioSource;
    public AudioClip damageSound;
    public float gameOverlayDelay = 3f;

    void Start()
    {
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
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy1") || collision.CompareTag("EnemyLaser"))
        {
            TakeDamage(1);
            Destroy(collision.gameObject);
        }
    }

    IEnumerator HandleDelayedDeath()
    {
        yield return new WaitForSeconds(gameOverlayDelay);

        GameManager gameManager = FindAnyObjectByType<GameManager>();

        if (gameManager != null)
        {
            GameManager.instance.InitiateGameOver();
        }

        Destroy(gameObject);
    }
}