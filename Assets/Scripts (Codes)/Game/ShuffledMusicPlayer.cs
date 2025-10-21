using UnityEngine;
using System.Collections;

public class ShuffledMusicPlayer : MonoBehaviour
{
    [Header("Audio Setup")]
    public AudioSource audioSource;   // Трябва да е свързан в Inspector
    public AudioClip[] clips;         // Постави всичките 4 песни тук

    [Header("Delay Settings")]
    public float minDelay = 2f;
    public float maxDelay = 3f;

    [Header("Loop")]
    public bool loopPlaylist = true;

    void Start()
    {
        if(audioSource == null)
        {
            Debug.LogError("AudioSource не е свързан в Inspector!");
            return;
        }

        if(clips == null || clips.Length == 0)
        {
            Debug.LogError("Музикалните клипове не са зададени!");
            return;
        }

        StartCoroutine(PlayShuffled());
    }

    IEnumerator PlayShuffled()
    {
        // Първоначално изчакване 2–3 секунди
        yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));

        while(true)
        {
            // Избор на случайна песен
            int index = Random.Range(0, clips.Length);

            // Поставяме в AudioSource и пускаме
            audioSource.clip = clips[index];
            audioSource.Play();

            // Изчакваме края на песента + случайно забавяне
            yield return new WaitForSeconds(audioSource.clip.length + Random.Range(minDelay, maxDelay));

            if(!loopPlaylist)
                break;
        }
    }
}
