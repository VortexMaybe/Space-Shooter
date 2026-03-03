using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float scrollSpeed = 0.5f;
    [SerializeField] private float resetPosition;
    [SerializeField] private float startPosition;

    void Start()
    {
        startPosition = transform.position.y;
    }

    void Update()
    {
        transform.Translate(Vector3.down * scrollSpeed * Time.deltaTime);

        if (transform.position.y < startPosition - resetPosition)
        {
            transform.position = new Vector3(transform.position.x, startPosition, transform.position.z);
        }
    }
}