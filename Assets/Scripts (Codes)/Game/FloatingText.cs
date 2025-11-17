using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;     
    [SerializeField] private float duration = 0.8f;      

    private TextMeshPro textMesh;
    private float timer;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();

        if (textMesh == null)
        {
            Debug.LogError("FloatingText.cs не намери TextMeshPro компонент на същия обект!");
        }
    }

    public void Initialize(int score, Color color)
    {
        if (textMesh != null)
        {
            textMesh.text = $"+{score}";
            textMesh.color = color;
        }
        else
        {
            Debug.LogError("Не може да се инициализира Floating Text, TextMeshPro е null.");
        }
    }

    void Update()
    {
        transform.position += new Vector3(0, moveSpeed, 0) * Time.deltaTime;

        timer += Time.deltaTime;

        if (timer > duration)
        {
            Destroy(gameObject);
        }
        else if (textMesh != null)
        {
            float alpha = 1f - (timer / duration);
            Color newColor = textMesh.color;
            newColor.a = alpha;
            textMesh.color = newColor;
        }
    }
}