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

        int newlyLostLifeIndex = -1;

        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (i < currentLifes)
            {
                lifeIcons[i].sprite = fullLifeSprite;
                lifeIcons[i].transform.Find("Line1")?.gameObject.SetActive(false);
                lifeIcons[i].transform.Find("Line2")?.gameObject.SetActive(false);
            }
            else
            {
                if (i == currentLifes)
                {
                    newlyLostLifeIndex = i;
                }
            }
        }
        if (newlyLostLifeIndex != -1)
        {
            ShakeUI();
            StartCoroutine(AnimateLifeLoss(lifeIcons[newlyLostLifeIndex].transform));
        }
    }
    IEnumerator AnimateLifeLoss(Transform lifeIconTransform)
    {
        GameObject line1 = lifeIconTransform.Find("Line1")?.gameObject;
        GameObject line2 = lifeIconTransform.Find("Line2")?.gameObject;

        if (line1 == null || line2 == null) yield break;

        line1.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        line2.SetActive(true);
    }

    public void ShakeUI()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        Vector3 originalPos = rectTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;

            rectTransform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.localPosition = originalPos;
    }
}