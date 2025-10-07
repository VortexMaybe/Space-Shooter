using UnityEngine;

public class RandomCameraStart : MonoBehaviour
{
    public Transform background;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        Vector2 bgSize = sr.bounds.size;

        float camHeight = mainCamera.orthographicSize * 2f;
        float camWidth = camHeight * mainCamera.aspect;

        float posX = Random.Range(-bgSize.x / 2 + camWidth / 2, bgSize.x / 2 - camWidth / 2);
        float posY = Random.Range(-bgSize.y / 2 + camHeight / 2, bgSize.y / 2 - camHeight / 2);

        mainCamera.transform.position = new Vector3(posX, posY, mainCamera.transform.position.z);
    }
}
