using UnityEngine;

public enum PowerUpTier { Bronze, Silver, Gold }
public enum PowerUpType { None, RapidFire, Recovery, Shield, Speed, MegaXP }
public class PowerUp : MonoBehaviour
{
    [Header("Power Up Settings")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] public PowerUpType type;
    [SerializeField] public PowerUpTier tier = PowerUpTier.Bronze;

    [Header("Audio Effects")]
    [SerializeField] private AudioClip bronzeSound;
    [SerializeField] private AudioClip silverSound;
    [SerializeField] private AudioClip goldSound;

    [Header("Visual Effects (Prefabs)")]
    public GameObject bronzePickupEffect;
    public GameObject silverPickupEffect;
    public GameObject goldPickupEffect;

    [Header("Sprites")]
    private SpriteRenderer sr;
    public Sprite bronzeSprite;
    public Sprite silverSprite;
    public Sprite goldSprite;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        if (transform.position.y < -7f) Destroy(gameObject);
    }

    public void SetVisuals(PowerUpTier tier)
    {
        if (sr == null) sr = GetComponent<SpriteRenderer>();
        switch (tier)
        {
            case PowerUpTier.Bronze: sr.sprite = bronzeSprite; break;
            case PowerUpTier.Silver: sr.sprite = silverSprite; break;
            case PowerUpTier.Gold: sr.sprite = goldSprite; break;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerMovement.instance != null)
            {
                PlayerMovement.instance.ActivatePowerUp(type, tier);
            }

            HandleEffects(); // Извикваме ефектите тук
            Destroy(gameObject);
        }
    }

    private void HandleEffects()
    {
        AudioClip soundToPlay = null;
        GameObject effectToSpawn = null;

        // Избираме звук и ефект според Tier-а
        switch (tier)
        {
            case PowerUpTier.Bronze:
                soundToPlay = bronzeSound;
                effectToSpawn = bronzePickupEffect;
                break;
            case PowerUpTier.Silver:
                soundToPlay = silverSound;
                effectToSpawn = silverPickupEffect;
                break;
            case PowerUpTier.Gold:
                soundToPlay = goldSound;
                effectToSpawn = goldPickupEffect;
                break;
        }

        // Пускаме звука
        if (soundToPlay != null)
        {
            AudioSource.PlayClipAtPoint(soundToPlay, Camera.main.transform.position, 1f);
        }

        // Създаваме ефекта
        if (effectToSpawn != null)
        {
            Instantiate(effectToSpawn, transform.position, Quaternion.identity);
            // Забележка: Повечето Cartoon FX префаби имат скрипт (CFX_AutoDestruct), 
            // който ги трие сам, така че няма нужда от Destroy(effect) тук.
        }
    }
}