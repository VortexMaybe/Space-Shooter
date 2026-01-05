using UnityEngine;
using System.Collections;

public class Enemy_Standard : EnemyBase
{
    [Header("Movement & Tracking")]
    [SerializeField] float standardSpeed = 3.0f; // Основна скорост надолу
    [SerializeField] float wiggleMagnitude = 0.5f; // Сила на клатушкане по X
    [SerializeField] float trackingSpeed = 5f; // Скорост на ротация към играча (Turret-а)

    [Header("Shooting")]
    [SerializeField] float standardShootInterval = 3.0f;
    [SerializeField] GameObject standardLaserPrefab;

    private float initialX; // Записва началната X позиция за клатушкане

    protected override void Start()
    {
        base.Start();
        initialX = transform.position.x; // Инициализираме началната X позиция

        // Стартираме рутината за стандартна стрелба
        StartCoroutine(ShootRoutine(standardShootInterval, standardLaserPrefab));
    }

    protected override void Update()
    {
        // 1. ДВИЖЕНИЕ (Wiggling Movement):
        HandleMovement();

        // 2. ПРОСЛЕДЯВАНЕ (Tracking):
        HandleTrackingRotation();

        base.Update(); // Извикваме логиката на базата (CheckBounds)
    }

    void HandleMovement()
    {
        // Изчисляваме плавното клатушкане (Wiggle)
        float sinXOffset = Mathf.Sin(Time.time * 3f) * wiggleMagnitude;

        // Изчисляваме новата Y позиция (движение надолу)
        Vector3 newPosition = transform.position + Vector3.down * standardSpeed * Time.deltaTime;

        // Прилагаме клатушкането върху началната X позиция, комбинирано с новата Y позиция
        transform.position = new Vector3(initialX + sinXOffset, newPosition.y, 0);
    }

    void HandleTrackingRotation()
    {
        if (playerTransform != null) // playerTransform се инициализира в EnemyBase
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            // Ъгълът за Unity 2D (нагоре е 90 градуса)
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90f);

            // Плавно завъртане към целта
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                                                            trackingSpeed * Time.deltaTime);
        }
    }
}