using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerLifesUI : MonoBehaviour
{
    public Image[] lifeIcons;
    public Sprite fullLifeSprite;
    private RectTransform rectTransform;

    [Header("UI Shake Settings")]
    public float shakeDuration = 0.2f;
    public float shakeMagnitude = 5f;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void UpdateLifes(int currentLifes)
    {
        if (lifeIcons == null || lifeIcons.Length == 0) return;

        bool lostLife = false;

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i < currentLifes)
            {
                // Имаме живот - показваме иконата цяла
                lifeIcons[i].sprite = fullLifeSprite;
                SetLinesActive(lifeIcons[i].transform, false);
            }
            else
            {
                // Нямаме живот - ако иконата е била активна, я "задраскваме"
                GameObject line1 = lifeIcons[i].transform.Find("Line1")?.gameObject;
                if (line1 != null && !line1.activeSelf)
                {
                    StartCoroutine(AnimateLifeLoss(lifeIcons[i].transform));
                    lostLife = true;
                }
            }
        }

        if (lostLife) ShakeUI();
    }

    private void SetLinesActive(Transform icon, bool state)
    {
        icon.Find("Line1")?.gameObject.SetActive(state);
        icon.Find("Line2")?.gameObject.SetActive(state);
    }

    IEnumerator AnimateLifeLoss(Transform lifeIconTransform)
    {
        GameObject line1 = lifeIconTransform.Find("Line1")?.gameObject;
        GameObject line2 = lifeIconTransform.Find("Line2")?.gameObject;

        if (line1 != null) { line1.SetActive(true); yield return new WaitForSeconds(0.1f); }
        if (line2 != null) { line2.SetActive(true); }
    }

    public void ShakeUI()
    {
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        Vector3 originalPos = rectTransform.localPosition;
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            rectTransform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeMagnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.localPosition = originalPos;
    }
}