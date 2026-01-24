using UnityEngine;

// Дефинираме качеството на Power-Up-а
public enum PowerUpTier
{
    Bronze, // Най-чест, най-слаб ефект (напр. +1%)
    Silver, // Среден, умерен ефект (напр. +2.5%)
    Gold    // Най-рядък, най-силен ефект (напр. +5%)
}

public enum PowerUpType
{
    None,
    RapidFire,  // По-бърза стрелба
    Recovery,   // Възстановяване на здраве
    Shield,     // Временен щит
    Speed,      // Увеличение на скоростта
    MegaXP      // Голям XP бонус
}

public class PowerUp : MonoBehaviour
{
    [Header("Power Up Settings")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] public PowerUpType type; // Тип на ефекта

    [Header("Pickup Effects")]
    [SerializeField] private AudioClip bronzeSound;
    [SerializeField] private AudioClip silverSound;
    [SerializeField] private AudioClip goldSound;
    [SerializeField] public PowerUpTier tier = PowerUpTier.Bronze;

    [Header("Visual Effects")]
    public GameObject pickupEffectPrefab;
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

        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
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
        Debug.Log("Power-Up се докосна до: " + other.name);

        if (other.CompareTag("Player"))
        {
            if (PlayerMovement.instance != null)
            {
                // Изпращаме ТИПА И КАЧЕСТВОТО към PlayerController-а
                PlayerMovement.instance.ActivatePowerUp(type, tier);
            }

            AudioClip soundToPlay = null;

            switch (tier)
            {
                case PowerUpTier.Bronze: soundToPlay = bronzeSound; break;
                case PowerUpTier.Silver: soundToPlay = silverSound; break;
                case PowerUpTier.Gold: soundToPlay = goldSound; break;
            }

            if (soundToPlay != null)
            {
                AudioSource.PlayClipAtPoint(soundToPlay, Camera.main.transform.position, 1f);
            }

            if (pickupEffectPrefab != null)
            {
                GameObject effect = Instantiate(pickupEffectPrefab, transform.position, Quaternion.identity);

                Destroy(effect, 1f);

                var main = effect.GetComponent<ParticleSystem>().main;
                if (tier == PowerUpTier.Gold) main.startColor = Color.yellow;
                else if (tier == PowerUpTier.Silver) main.startColor = Color.white;
                else main.startColor = new Color(0.8f, 0.5f, 0.2f);
            }

            Destroy(gameObject);
        }
    }
}