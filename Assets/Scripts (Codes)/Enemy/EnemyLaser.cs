using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
    }
}