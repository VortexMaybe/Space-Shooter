using UnityEngine;

public class CameraClampDynamic : MonoBehaviour
{
    public Transform background; 
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        float camHalfHeight = cam.orthographicSize;
        float camHalfWidth = camHalfHeight * cam.aspect;

        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        Vector2 bgSize = sr.bounds.size;
        Vector3 bgPos = background.position;

        float minX = bgPos.x - bgSize.x / 2 + camHalfWidth;
        float maxX = bgPos.x + bgSize.x / 2 - camHalfWidth;
        float minY = bgPos.y - bgSize.y / 2 + camHalfHeight;
        float maxY = bgPos.y + bgSize.y / 2 - camHalfHeight;

        Vector3 pos = cam.transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        cam.transform.position = pos;
    }
}
