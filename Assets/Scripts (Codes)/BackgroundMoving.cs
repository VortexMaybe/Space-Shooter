using UnityEngine;

public class RandomBackgroundMotion : MonoBehaviour
{
    public float speed = 0.5f;       
    public float changeTime = 3f;    

    private Vector3 targetDir;
    private float timer;

    void Start()
    {
        PickNewDirection();
    }

    void Update()
    {
        transform.position += targetDir * speed * Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            PickNewDirection();
        }
    }

    void PickNewDirection()
    {
        float dirX = Random.Range(-1f, 1f);
        float dirY = Random.Range(-1f, 1f);

        targetDir = new Vector3(dirX, dirY, 0f).normalized;

        timer = changeTime + Random.Range(-1f, 1f);
    }
}

