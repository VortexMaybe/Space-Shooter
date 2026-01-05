using UnityEngine;
using System.Collections;

public class Enemy_Turret : EnemyBase
{
    [Header("Turret Specifics")]
    [SerializeField] float turretSpeed = 2f;
    [SerializeField] float trackingSpeed = 5f; // Скорост на ротация
    [SerializeField] GameObject turretLaserPrefab;
    [SerializeField] float turretShootInterval = 2.0f;

    [Header("Dual Shot Settings")]
    [SerializeField] float offsetAngle = 15f; // Ъгъл на разстояние между двата лазера

    protected override void Start()
    {
        base.Start();

        // Извикваме специалната рутина за двойна стрелба
        StartCoroutine(TurretShootRoutine());
    }

    // Извиква двойния изстрел
    IEnumerator TurretShootRoutine()
    {
        yield return new WaitForSeconds(turretShootInterval / 2);

        while (true)
        {
            yield return new WaitForSeconds(turretShootInterval);
            ShootDualLaser(); // Извикваме двойния изстрел
        }
    }

    // Изстрел на два лазера с офсет
    void ShootDualLaser()
    {
        if (turretLaserPrefab != null)
        {
            // Изстрел 1: Лек офсет наляво
            Quaternion rot1 = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + offsetAngle);
            Instantiate(turretLaserPrefab, transform.position, rot1);

            // Изстрел 2: Лек офсет надясно
            Quaternion rot2 = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z - offsetAngle);
            Instantiate(turretLaserPrefab, transform.position, rot2);
        }
    }

    protected override void Update()
    {
        // Бавно движение надолу
        transform.Translate(Vector3.down * turretSpeed * Time.deltaTime);

        // Ротация към играча
        HandleTurretRotation();

        base.Update();
    }

    void HandleTurretRotation()
    {
        if (playerTransform != null) // playerTransform е protected в EnemyBase
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            // Ъгълът за Unity 2D (нагоре е 90)
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90f);

            // Ротация със специфична скорост
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                                                            trackingSpeed * Time.deltaTime);
        }
    }
}