using UnityEngine;
using TMPro;
using System;

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
            Debug.LogError("FloatingText.cs �� ������ TextMeshPro ��������� �� ����� �����!");
        }
    }

    public void Initialize(int value, Color color)
    {
        if (textMesh != null)
        {
            textMesh.text = $"+{value} XP";
            textMesh.color = color;
        }
        else
        {
            Debug.LogError("�� ���� �� �� ������������ Floating Text, TextMeshPro � null.");
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

    internal void Initialize(string message, Color textColor)
    {
        throw new NotImplementedException();
    }
}