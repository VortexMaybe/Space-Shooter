using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    [SerializeField] GameObject laser;

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

        if(Input.GetButtonDown("Fire1"))
        {
            Instantiate(laser, transform.position, Quaternion.identity);
        }
    }
}
