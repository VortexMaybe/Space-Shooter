using UnityEngine;

public class SimpleBackgroundScroll : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 0.5f;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        Vector2 offset = new Vector2(0, Time.time * scrollSpeed);
        meshRenderer.material.mainTextureOffset = offset;
    }
}