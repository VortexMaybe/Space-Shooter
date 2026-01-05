using UnityEngine;
using System.Collections;

public class Enemy_Fast : EnemyBase
{
    [Header("Fast Specifics")]
    [SerializeField] float slowFastSpeed = 4f; // Базова скорост
    [SerializeField] float wiggleMagnitude = 1.5f; // Сила на клатушкане
    [SerializeField] GameObject fastLaserPrefab;
    [SerializeField] float slowerFastShootInterval = 1.6f; // Интервал на стрелба (средно-бавен)

    private float initialX;

    protected override void Start()
    {
        base.Start();
        initialX = transform.position.x; // Записваме началната X позиция

        // Стартираме рутината за стрелба
        StartCoroutine(ShootRoutine(slowerFastShootInterval, fastLaserPrefab));
    }

    protected override void Update()
    {
        // Клатушкане с по-бавна основна скорост
        float sinXOffset = Mathf.Sin(Time.time * 5f) * wiggleMagnitude;
        Vector3 newPosition = transform.position + Vector3.down * slowFastSpeed * Time.deltaTime;

        // Прилагаме клатушкането върху началната X позиция
        transform.position = new Vector3(initialX + sinXOffset, newPosition.y, 0);

        base.Update();
    }
}