using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float lifeTime = 3f;

    [Header("Насочване")]
    [SerializeField] float maxAngleDeviation = 45f;

    private Vector3 direction;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {

            Vector3 targetDirection = (player.transform.position - transform.position).normalized;

            Vector3 downDirection = Vector3.down;

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
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }

            Destroy(gameObject);
        }
    }
}