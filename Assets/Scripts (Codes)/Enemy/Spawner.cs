using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] float spawnRate = 2f;
    [SerializeField] GameObject enemyPrefab1;

    float xMin;
    float xMax;
    float ySpawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(.1f, 0, 0)).x;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(.9f, 0, 0)).x;
        ySpawn = Camera.main.ViewportToWorldPoint(new Vector3(0, 1.25f, 0)).y;

        InvokeRepeating("SpawnEnemy", 1f, spawnRate);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnEnemy()
    { 
         float randX = Random.Range(xMin, xMax);
         Instantiate(enemyPrefab1, new Vector3(randX, ySpawn, 0), Quaternion.identity);
    }
}
