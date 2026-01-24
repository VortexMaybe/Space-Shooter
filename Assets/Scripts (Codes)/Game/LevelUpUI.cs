using UnityEngine;
using TMPro;
using System.Collections;

public class LevelUpUI : MonoBehaviour
{
    [SerializeField] private GameObject levelUpTextObject;

    [Header("Animation Settings")]
    [SerializeField] private float moveSpeed = 100f;      // Скорост на издигане
    [SerializeField] private float displayDuration = 2f;  // Колко време общо да се вижда
    [SerializeField] private float blinkSpeed = 0.2f;     // Скорост на мигане

    private TextMeshProUGUI textComponent;
    private RectTransform rectTransform;
    private Vector2 startPosition; // За да връщаме текста в центъра всеки път

    void OnEnable()
    {
        ExperienceManager.OnLevelUp += ShowLevelUpEffect;
    }

    void OnDisable()
    {
        ExperienceManager.OnLevelUp -= ShowLevelUpEffect;
    }

    void Start()
    {
        if (levelUpTextObject != null)
        {
            textComponent = levelUpTextObject.GetComponent<TextMeshProUGUI>();
            rectTransform = levelUpTextObject.GetComponent<RectTransform>();
            startPosition = rectTransform.anchoredPosition; // Запомняме центъра
            levelUpTextObject.SetActive(false);
        }
    }

    void ShowLevelUpEffect()
    {
        StopAllCoroutines();
        StartCoroutine(LevelUpRoutine());
    }

    IEnumerator LevelUpRoutine()
    {
        // 1. Рестартираме позицията и прозрачността
        rectTransform.anchoredPosition = startPosition;
        textComponent.canvasRenderer.SetAlpha(1f);
        levelUpTextObject.SetActive(true);

        float elapsed = 0f;

        // 2. Анимация: Движение нагоре + Мигане в началото
        while (elapsed < displayDuration)
        {
            // Движим нагоре
            rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

            // Мигане през първата 1 секунда
            if (elapsed < 1.0f)
            {
                float alpha = (Mathf.Sin(elapsed * (1 / blinkSpeed) * Mathf.PI) > 0) ? 1f : 0.5f;
                textComponent.canvasRenderer.SetAlpha(alpha);
            }
            else
            {
                // Плавно изчезване (Fade Out) след мигането
                float fadeProgress = (elapsed - 1.0f) / (displayDuration - 1.0f);
                textComponent.canvasRenderer.SetAlpha(1f - fadeProgress);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        levelUpTextObject.SetActive(false);
    }
}