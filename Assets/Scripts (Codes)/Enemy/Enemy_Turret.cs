using UnityEngine;
using System.Collections;

public class Enemy_Turret : EnemyBase
{
    [Header("Turret Specifics")]
    [SerializeField] float turretSpeed = 2f;
    [SerializeField] float trackingSpeed = 5f;
    [SerializeField] GameObject turretLaserPrefab; 

    protected override void Start()
    {
        base.Start();
        StartCoroutine(ShootRoutine(2.0f, turretLaserPrefab)); 
    }

    protected override void Update()
    {
      
        transform.Translate(Vector3.down * turretSpeed * Time.deltaTime);

       
        HandleTurretRotation();

        base.Update();
    }

    void HandleTurretRotation()
    {
        if (playerTransform != null)
        {
            Vector3 directionToPlayer = playerTransform.position - transform.position;
            float angle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;

            Quaternion targetRotation = Quaternion.Euler(0, 0, angle + 90f);

            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation,
                                                            trackingSpeed * Time.deltaTime);
        }
    }

    protected IEnumerator ShootRoutine(float interval, GameObject laserPrefab)
    {
        yield break; 
    }
}