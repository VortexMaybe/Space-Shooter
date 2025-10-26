using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(PlayClickSound);
    }

    private void PlayClickSound()
    {
        if (SoundManager.instance != null)
        {
            SoundManager.instance.PlayButtonClick();
        }
        else
        {
            Debug.LogWarning("SoundManager не е намерен в сцената!");
        }
    }
}
