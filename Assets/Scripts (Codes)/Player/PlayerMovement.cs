using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    [SerializeField] float fireRate = 0.5f;
    float nextFireTime = 0f;
    [SerializeField] GameObject laser;
    public AudioSource audioSource;
    public AudioClip shootSound;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, moveY, 0f) * speed * Time.deltaTime;
        transform.position += movement;

        Camera cam = Camera.main;
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, cam.transform.position.x - camHalfWidth, cam.transform.position.x + camHalfWidth);
        pos.y = Mathf.Clamp(pos.y, cam.transform.position.y - camHalfHeight, cam.transform.position.y + camHalfHeight);

        transform.position = pos;

        if (Input.GetButton("Fire1") && Time.time >= nextFireTime)
        {
            Instantiate(laser, transform.position, Quaternion.identity);
            nextFireTime = Time.time + fireRate;

            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }

    }
}
