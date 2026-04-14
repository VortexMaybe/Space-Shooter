using UnityEngine;
using UnityEngine.UI;

public class BossManager : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 3000f;
    public float currentHealth;

    [Header("UI Reference")]
    public GameObject healthBarCanvas; // Плъзни целия Canvas тук
    public Image fillImage;           // Плъзни червената картинка (Fill) тук

    void Start()
    {
        currentHealth = maxHealth;

        // Настройваме картинката да е пълна
        if (fillImage != null) fillImage.fillAmount = 1f;

        // Скриваме лентата за интрото
        if (healthBarCanvas != null) healthBarCanvas.SetActive(false);
    }

    // Тази функция се вика от Animation Event-а на Боса
    public void ShowHealthBar()
    {
        if (healthBarCanvas != null) healthBarCanvas.SetActive(true);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        // Обновяваме лентата веднага
        if (fillImage != null)
        {
            fillImage.fillAmount = currentHealth / maxHealth;
        }

        if (currentHealth <= 0) Die();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяваме дали ни удря лазер
        if (other.CompareTag("PlayerLaser"))
        {
            TakeDamage(20f); // Колкото демидж искаш
            Destroy(other.gameObject); // Унищожаваме лазера
        }
    }

    void Die()
    {
        Destroy(gameObject);
        Debug.Log("Босът е победен!");
    }
}