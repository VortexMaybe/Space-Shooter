using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] float laserSpeed = 1.4f;
    Camera mainCam;
    void Start()
    {
        mainCam = Camera.main;
    }


    void Update()
    {
        transform.position += Vector3.up * laserSpeed * Time.deltaTime;

        Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
        if (viewportPos.y > 1 || viewportPos.y < 0 || viewportPos.x < 0 || viewportPos.x > 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        EnemySpawn enemy = other.GetComponent<EnemySpawn>();

        if (enemy != null)
        {
            enemy.EnemyDestroyedByPlayerLaser();
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("EnemyLaser"))
        {
            Destroy(other.gameObject);

            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Enemy1"))
        {
            Destroy(gameObject);
            return;
        }
    }
}
