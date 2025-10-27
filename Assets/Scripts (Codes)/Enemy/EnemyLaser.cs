using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField] float speed = 8f;
    [SerializeField] float lifeTime = 5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 🟢 Вместо директно GameOver, викаме LoseLife()
            GameManager.instance.LoseLife();

            Destroy(gameObject);
        }
    }
}
