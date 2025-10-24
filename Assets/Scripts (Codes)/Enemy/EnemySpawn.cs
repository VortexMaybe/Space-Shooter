using System.Collections;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip deathSound;
    [SerializeField] float speed = 10f;
    [SerializeField] float spawnInvulnerabilityTime = 1.0f;

    bool canBeHit = false;
    [SerializeField] GameManager manager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(EnableHitAfterDelay());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position -= new Vector3(0, speed, 0) * Time.deltaTime;
    }

    IEnumerator EnableHitAfterDelay()
    {
        yield return new WaitForSeconds(spawnInvulnerabilityTime);
        canBeHit = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canBeHit) return;

        if (audioSource != null)
        {
            SoundManager.instance.PlayOneShot(deathSound);
        }

        if (collision.gameObject.tag == "Player")
        {
            GameManager.instance.InitiateGameOver();
        }
        else if (collision.CompareTag("Laser"))
        {
            GameManager.instance.IncreaseScore(10);
        }

        Destroy(gameObject);
        Destroy(collision.gameObject);
    }
}
