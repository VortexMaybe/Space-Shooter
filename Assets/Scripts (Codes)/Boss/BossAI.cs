using UnityEngine;
using System.Collections;

public class BossAI : MonoBehaviour
{
    [Header("Movement & Dash")]
    public float dashSpeed = 35f;
    public float returnSpeed = 10f;
    private Vector3 startPos;
    private bool canAttack = false;

    [Header("Shooting")]
    public GameObject laserPrefab;
    public Transform[] shootPoints;
    private int currentPointIndex = 0;
    public float fireRate = 1.5f;

    private BossHealth health;

    void Start()
    {
        // Запомня позицията след интрото (Idle позицията)
        startPos = transform.position;
        health = GetComponent<BossHealth>();

        // Изчакваме интрото и смеха да свършат
        Invoke("EnableCombat", 2.5f);
    }

    void EnableCombat() => canAttack = true;

    void Update()
    {
        if (!canAttack) return;

        if (!IsInvoking("ChooseAttack"))
        {
            // Във фаза 3 босът става много по-бърз
            float cooldown = (health.currentPhase == 3) ? 0.8f : 2f;
            Invoke("ChooseAttack", cooldown);
        }
    }

    void ChooseAttack()
    {
        int chance = Random.Range(0, 100);

        if (health.currentPhase == 1)
        {
            LaserAttack(); // Редува ляво и дясно
        }
        else if (health.currentPhase == 2)
        {
            if (chance < 30) StartCoroutine(DashAttack());
            else LaserAttack();
        }
        else // ФАЗА 3
        {
            if (chance < 50) StartCoroutine(DashAttack());
            else TripleLaserAttack(); // Стреля мощно от двете страни
        }
    }

    // Редува стрелбата между лявото и дясното дуло
    void LaserAttack()
    {
        if (shootPoints.Length < 2) return; // Проверка дали си сложил точките в Unity

        Transform firePoint = shootPoints[currentPointIndex];
        Instantiate(laserPrefab, firePoint.position, Quaternion.identity);

        // Сменя за следващия път (0 става 1, 1 става 0)
        currentPointIndex = (currentPointIndex == 0) ? 1 : 0;
    }

    // Във фаза 3 стреля ветрилообразно от двете оръдия едновременно!
    void TripleLaserAttack()
    {
        foreach (Transform p in shootPoints)
        {
            for (int i = -1; i <= 1; i++)
            {
                GameObject laser = Instantiate(laserPrefab, p.position, Quaternion.identity);
                laser.transform.Rotate(0, 0, i * 15f);
            }
        }
    }

    IEnumerator DashAttack()
    {
        canAttack = false;

        // 1. Dash надолу през екрана
        Vector3 target = new Vector3(transform.position.x, -12f, 0);
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);
            yield return null;
        }

        // 2. Връщане нагоре към старта
        while (Vector3.Distance(transform.position, startPos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, returnSpeed * Time.deltaTime);
            yield return null;
        }

        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("БОСЪТ ТЕ УДАРИ ПРИ DASH!");
        }
    }
}