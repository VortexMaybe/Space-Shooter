using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float lifeTime = 3f;

    // ✅ НОВА ПРОМЕНЛИВА: Максимален ъгъл на отклонение от вертикалата
    [Header("Насочване")]
    [SerializeField] float maxAngleDeviation = 45f; // Препоръчителна стойност: 30 до 50 градуса

    private Vector3 direction;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // 1. Изчисляваме вектора на **директната** посока към играча
            Vector3 targetDirection = (player.transform.position - transform.position).normalized;

            // 2. Определяме вертикалната посока
            Vector3 downDirection = Vector3.down;

            // 3. ✅ КЛЮЧОВА ПРОМЯНА: Ограничаваме ъгъла на насочване.
            // Vector3.RotateTowards завърта вектора надолу (downDirection) към targetDirection,
            // но само до максимално допустимия ъгъл (maxAngleDeviation).
            direction = Vector3.RotateTowards(downDirection, targetDirection,
                                             maxAngleDeviation * Mathf.Deg2Rad, 0f);

            // Нормализираме резултата, за да сме сигурни, че дължината е 1
            direction.Normalize();

            // 4. Завъртаме лазера за визуална коректност
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        }
        else
        {
            // Ако играчът не е намерен, стреля само надолу (безопасна стойност)
            direction = Vector3.down;
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // Движим лазера в изчислената посока
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Намираме PlayerHealth, за да нанесем щета
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // Предполагам, че методът ти за нанасяне на щета се казва TakeDamage
                playerHealth.TakeDamage(1);
            }

            // Унищожаваме лазера, тъй като е ударил целта си
            Destroy(gameObject);
        }
        // ... (друга логика за сблъсък) ...
    }
}