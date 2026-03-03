using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float duration = 0.8f;
    private TextMeshPro textMesh;
    private float timer;

    void Awake() { textMesh = GetComponent<TextMeshPro>(); }

    // Този метод приема всичко и го прави на текст автоматично
    public void Initialize(object message, Color color)
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshPro>();
        if (textMesh != null)
        {
            textMesh.text = (message is int) ? "+" + message.ToString() + " XP" : message.ToString();
            textMesh.color = color;
        }
    }

    void Update()
    {
        transform.position += new Vector3(0, moveSpeed, 0) * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer > duration) Destroy(gameObject);
        else if (textMesh != null)
        {
            float alpha = 1f - (timer / duration);
            Color newColor = textMesh.color;
            newColor.a = alpha;
            textMesh.color = newColor;
        }
    }
}