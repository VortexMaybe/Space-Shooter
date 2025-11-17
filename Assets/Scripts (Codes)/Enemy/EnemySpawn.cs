using System.Collections;
using UnityEditor.EditorTools;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public AudioSource audioSource;

    float customVolume = 5.6f;
    [SerializeField] AudioClip deathSound;
    [SerializeField] float speed = 3f;
    [SerializeField] float spawnInvulnerabilityTime = 1.0f;

    [Header("Shooting Settings")]
    [SerializeField] GameObject enemyLaserPrefab;
    [SerializeField] float shootInterval = 2.5f;

    [Header("Experience")]
    [Tooltip("Най-малкото XP, който врагът да дава")]
    [SerializeField] private int minExperience = 5;
    [Tooltip("Най-много XP, който врагът да дава")]
    [SerializeField] private int maxExperience = 13;

    [Header("Score")]
    [SerializeField] private int baseScoreValue = 10;

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

        GameManager.instance.AddScore(10);

        ExperienceManager expManager = FindAnyObjectByType<ExperienceManager>();

        if (expManager != null)
        {
            int experienceGained = Random.Range(minExperience, maxExperience);

            expManager.AddExperience(experienceGained);
        }

        if (deathSound != null)
        {
            AudioSource.PlayClipAtPoint(deathSound, transform.position, customVolume);
        }

        if (GameManager.instance != null)
        {
            GameManager.instance.AddScore(baseScoreValue);
        }

        Destroy(gameObject);

    }
}
