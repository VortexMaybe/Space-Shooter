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

    // Референция към новия мениджър на кръвта
    private BossManager bossManager;

    void Start()
    {
        startPos = transform.position;

        // Търсим новия скрипт BossManager върху Боса
        bossManager = GetComponent<BossManager>();

        // Изчакваме интрото (можеш да го викаш и от Animation Event ако искаш)
        Invoke("EnableCombat", 2.5f);
    }

    void EnableCombat() => canAttack = true;

    void Update()
    {
        if (!canAttack || bossManager == null) return;

        if (!IsInvoking("ChooseAttack"))
        {
            // Изчисляваме фазата на базата на текущата кръв от BossManager
            int currentPhase = GetCurrentPhase();

            // Във фаза 3 босът атакува по-често
            float cooldown = (currentPhase == 3) ? 0.8f : 2f;
            Invoke("ChooseAttack", cooldown);
        }
    }

    // Помощна функция, която казва в коя фаза сме според BossManager
    int GetCurrentPhase()
    {
        float hpPercent = bossManager.currentHealth / bossManager.maxHealth;
        if (hpPercent <= 0.3f) return 3;
        if (hpPercent <= 0.7f) return 2;
        return 1;
    }

    void ChooseAttack()
    {
        int phase = GetCurrentPhase();
        int chance = Random.Range(0, 100);

        if (phase == 1)
        {
            LaserAttack();
        }
        else if (phase == 2)
        {
            if (chance < 30) StartCoroutine(DashAttack());
            else LaserAttack();
        }
        else // ФАЗА 3
        {
            if (chance < 50) StartCoroutine(DashAttack());
            else TripleLaserAttack();
        }
    }

    void LaserAttack()
    {
        if (shootPoints.Length < 2) return;
        Transform firePoint = shootPoints[currentPointIndex];
        Instantiate(laserPrefab, firePoint.position, Quaternion.identity);
        currentPointIndex = (currentPointIndex == 0) ? 1 : 0;
    }

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
        Vector3 target = new Vector3(transform.position.x, -12f, 0);
        while (Vector3.Distance(transform.position, target) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, dashSpeed * Time.deltaTime);
            yield return null;
        }
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