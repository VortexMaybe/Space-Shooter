using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip deathSound;
    [SerializeField] float speed = 3f;
    [SerializeField] float spawnInvulnerabilityTime = 1.0f;

    [Header("Shooting Settings")]
    [SerializeField] GameObject enemyLaserPrefab;   // Префаб на вражеския лазер
    [SerializeField] float shootInterval = 2.5f;    // Колко често стреля

    bool canBeHit = false;

    void Start()
    {
        StartCoroutine(EnableHitAfterDelay());
        StartCoroutine(ShootRoutine());
    }

    void Update()
    {
        transform.position -= new Vector3(0, speed, 0) * Time.deltaTime;
    }

    IEnumerator EnableHitAfterDelay()
    {
        yield return new WaitForSeconds(spawnInvulnerabilityTime);
        canBeHit = true;
    }

    IEnumerator ShootRoutine()
    {
        // врагът ще стреля докато е жив
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeHit) return;

        if (collision.CompareTag("Player"))
        {
            GameManager.instance.InitiateGameOver();
        }
        else if (collision.CompareTag("Laser")) // Лазерът на играча
        {
            GameManager.instance.IncreaseScore(10);

            if (audioSource != null && deathSound != null)
            {
                SoundManager.instance.PlayOneShot(deathSound);
            }
        }

        Destroy(collision.gameObject);
        Destroy(gameObject);
        Debug.Log("Enemy fired a laser!");
    }
}
