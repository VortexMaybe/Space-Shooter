using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    // ✅ ПРОМЕНЕНО: Правим скоростта публична, за да може да се задава от Prefab
    [SerializeField] public float speed = 5f;
    [SerializeField] float lifeTime = 3f;

    [Header("Насочване")]
    [SerializeField] float maxAngleDeviation = 45f;

    // ✅ НОВА ПРОМЕНЛИВА: Щетата, която нанася този лазер
    [Header("Нанасяна щета")]
    [SerializeField] public int damageToPlayer = 1; // Default е 1 живот

    private Vector3 direction;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Vector3 targetDirection = (player.transform.position - transform.position).normalized;

            Vector3 downDirection = Vector3.down;

            // Логика за насочване (запазена)
            direction = Vector3.RotateTowards(downDirection, targetDirection,
                                              maxAngleDeviation * Mathf.Deg2Rad, 0f);

            direction.Normalize();

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle + 90);
        }
        else
        {
            direction = Vector3.down;
        }

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // ✅ ИЗПОЛЗВАМЕ ПРОМЕНЛИВАТА speed
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                // ✅ ИЗПОЛЗВАМЕ НОВАТА ПРОМЕНЛИВА ЗА ЩЕТА
                playerHealth.TakeDamage(damageToPlayer);
            }

            Destroy(gameObject);
        }
    }
}