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

    //  Трябва да е публична, за да се зададе от EnemyBase при дроп
    [SerializeField] public PowerUpTier tier = PowerUpTier.Bronze;

    void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        if (transform.position.y < -7f)
        {
            Destroy(gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerMovement.instance != null)
            {
                // Изпращаме ТИПА И КАЧЕСТВОТО към PlayerController-а
                PlayerMovement.instance.ActivatePowerUp(type, tier);
            }

            Destroy(gameObject);
        }
    }
}