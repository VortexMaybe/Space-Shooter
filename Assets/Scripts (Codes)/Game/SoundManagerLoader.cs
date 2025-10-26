using UnityEngine;

public class SoundManagerLoader : MonoBehaviour
{
    [SerializeField] GameObject soundManagerPrefab;

    void Awake()
    {
        if (SoundManager.instance == null)
        {
            Instantiate(soundManagerPrefab);
        }
    }
}
