using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float lifeTime = 3f;

    private Vector3 direction;

    void Start()
    {
        // Намираме кораба на играча по Tag. (Изисква Player обектът да има Tag = "Player")
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            // Изчисляваме вектора на посоката към играча
            direction = (player.transform.position - transform.position).normalized;

            // Завъртаме лазера за визуална коректност
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
            
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

            
            if (playerHealth != null)
            {
                
                playerHealth.TakeDamage(1); 
            }

          
            Destroy(gameObject);
        }
        // Можеш да добавиш и други обекти, които унищожават лазера, напр. "Shield"
        // else if (other.CompareTag("Wall")) 
        // {
        //     Destroy(gameObject);
        // }
    }
}