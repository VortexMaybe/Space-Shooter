using UnityEngine;
using System.Collections;

public class Enemy_Tank : EnemyBase
{
    [Header("Tank Specifics")]
    [SerializeField] float tankSpeed = 1.5f;
    [SerializeField] float tankShootInterval = 4.0f;
    [SerializeField] GameObject heavyBulletPrefab;



    protected override void Start()
    {
        base.Start();

        StartCoroutine(ShootRoutine(tankShootInterval, heavyBulletPrefab));
    }

    protected override void Update()
    {

        transform.Translate(Vector3.down * tankSpeed * Time.deltaTime);


        base.Update();
    }
}