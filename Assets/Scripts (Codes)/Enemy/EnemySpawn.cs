using System.Collections;
using UnityEngine;
using TMPro;

public class EnemySpawn : MonoBehaviour
{
    public AudioSource audioSource;
    [SerializeField] AudioClip deathSound;
    [SerializeField] float customVolume = 5.6f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 2;
    [SerializeField] private float hitFlickerDuration = 0.1f;

    [Header("Movement")]
    [SerializeField] float verticalSpeed = 3f;
    [SerializeField] float maxMovementAngle = 35f;
    [SerializeField] float rotationLerpSpeed = 1.5f;
    [SerializeField] float spawnInvulnerabilityTime = 1.0f;

    [Header("Wavy Movement")]
    [SerializeField] bool enableWavyMovement = false;
    [SerializeField] float sinFrequency = 1.5f;
    [SerializeField] float sinMagnitude = 0.5f;

    [Header("Shooting Settings")]
    [SerializeField] GameObject enemyLaserPrefab;
    [SerializeField] float shootInterval = 2.5f;

    [Header("Experience & Score")]
    [SerializeField] private int minExperience = 5;
    [SerializeField] private int maxExperience = 13;
    [SerializeField] private int baseScoreValue = 10;
    [SerializeField] private GameObject floatingTextPrefab;

    // [Вътрешни Променливи]
    private int currentHealth;
    private Transform playerTransform;
    private bool canBeHit = false;
    private SpriteRenderer spriteRenderer;
    private float initialX;

    // Граници
    private float lowerBoundY;
    private float leftBoundX;
    private float rightBoundX;

    private Vector3 movementDirection;
    private Vector3 targetDirection;

    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
        }

        initialX = transform.position.x;

        lowerBoundY = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).y - 3.5f;

        leftBoundX = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x - 0.1f;

        rightBoundX = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x + 0.1f;

        float randomZRotation = Random.Range(-maxMovementAngle, maxMovementAngle);
        targetDirection = Quaternion.Euler(0, 0, randomZRotation) * Vector3.down;

        movementDirection = Vector3.down;
        transform.rotation = Quaternion.identity;

        StartCoroutine(EnableHitAfterDelay());
        StartCoroutine(ShootRoutine());
    }

    void Update()
    {
        movementDirection = Vector3.Lerp(movementDirection, targetDirection, rotationLerpSpeed * Time.deltaTime);

        bool hitLeft = transform.position.x < leftBoundX;
        bool hitRight = transform.position.x > rightBoundX;

        if (hitLeft || hitRight)
        {
            movementDirection.x = -movementDirection.x;

            float newXPos = hitLeft ? leftBoundX : rightBoundX;
            transform.position = new Vector3(newXPos, transform.position.y, 0);

            targetDirection.x = -targetDirection.x;

            initialX = transform.position.x;
        }

        if (playerTransform != null)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;

            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90f);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                                                        rotationLerpSpeed * 30f * Time.deltaTime);
        }

        if (enableWavyMovement)
        {
            float sinXOffset = Mathf.Sin(Time.time * sinFrequency) * sinMagnitude;

            transform.position += movementDirection * verticalSpeed * Time.deltaTime;

            transform.position = new Vector3(initialX + sinXOffset, transform.position.y, transform.position.z);
        }
        else
        {
            transform.position += movementDirection * verticalSpeed * Time.deltaTime;
        }

        if (movementDirection != Vector3.zero)
        {
            float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                                                         rotationLerpSpeed * 10f * Time.deltaTime);
        }

        if (transform.position.y < lowerBoundY)
        {
            Destroy(gameObject);
        }
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
            Instantiate(enemyLaserPrefab, transform.position, transform.rotation);
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


        int experienceGained = 0;

        if (GameManager.instance != null)
        {
            experienceGained = GameManager.instance.AddScore(baseScoreValue);
        }

        ExperienceManager expManager = FindAnyObjectByType<ExperienceManager>();
        if (expManager != null)
        {
            experienceGained = UnityEngine.Random.Range(minExperience, maxExperience);
            expManager.AddExperience(experienceGained);
        }

        if (floatingTextPrefab != null)
        {
            GameObject ft = Instantiate(floatingTextPrefab, transform.position, Quaternion.identity);
            FloatingText ftScript = ft.GetComponent<FloatingText>();

            if (ftScript != null)
            {
                ftScript.Initialize(experienceGained, Color.yellow);
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