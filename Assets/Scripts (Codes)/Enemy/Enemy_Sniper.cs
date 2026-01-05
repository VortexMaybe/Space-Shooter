using UnityEngine;
using System.Collections;

public class Enemy_Sniper : EnemyBase
{
    [Header("Sniper Specifics")]
    [SerializeField] float sniperSpeed = 0.5f; // Много бавно движение
    [SerializeField] float sniperShootInterval = 5.0f; // Бавна стрелба
    [SerializeField] GameObject fastLaser2DamagePrefab; // Бърз лазер, нанасящ 2 щети

    protected override void Start()
    {
        base.Start();

        // Бавна стрелба с БЪРЗ лазер, нанасящ 2 щети
        StartCoroutine(ShootRoutine(sniperShootInterval, fastLaser2DamagePrefab));
    }

    protected override void Update()
    {
        // Движи се много бавно
        transform.Translate(Vector3.down * sniperSpeed * Time.deltaTime);
        base.Update();
    }
}