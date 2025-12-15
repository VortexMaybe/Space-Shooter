using UnityEngine;
using System.Collections;

public class Enemy_Sniper : EnemyBase
{
    [Header("Sniper Specifics")]
    [SerializeField] float sniperShootInterval = 5.0f; 
    [SerializeField] GameObject fastLaserPrefab;      

    protected override void Start()
    {
        base.Start();
        StartCoroutine(ShootRoutine(sniperShootInterval, fastLaserPrefab)); 
    }

    protected override void Update()
    {
        
        transform.Translate(Vector3.down * 0.5f * Time.deltaTime);
        base.Update();
    }

    protected IEnumerator ShootRoutine(float interval, GameObject laserPrefab)
    {
        yield break; 
    }
}