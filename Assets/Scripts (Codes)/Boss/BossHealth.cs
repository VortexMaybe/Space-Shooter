using UnityEngine;
using UnityEngine.UI;

public class BossHealth : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 500f;
    public float currentHealth; // Направих го public, за да го виждаш в Inspector-а
    public int currentPhase = 1;

    [Header("UI Reference")]
    public Slider healthSlider;

    [Header("Effects")]
    public GameObject explosionEffect;

    void Start()
    {
        currentHealth = maxHealth;

        // Настройка на слайдера
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = maxHealth;
            // По подразбиране го държим изключен, докато анимацията не извика ShowHealthBar
            healthSlider.gameObject.SetActive(false);
        }
    }

    // Добавих малко код тук, за да може лентата да следва боса, ако не си я сложил в World Space Canvas
    void Update()
    {
        // Ако лентата е дете на боса в World Space, тя ще се движи сама.
        // Ако босът се върти, можем да "заковем" ротацията на Canvas-а тук, за да е четим.
        if (healthSlider != null && healthSlider.gameObject.activeSelf)
        {
            healthSlider.transform.parent.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        // Обновяване на UI
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        // Логика за смяна на фазите
        UpdatePhases();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void UpdatePhases()
    {
        // ФАЗА 2: Под 70% кръв
        if (currentHealth <= maxHealth * 0.7f && currentPhase == 1)
        {
            currentPhase = 2;
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.6f, 0.6f); // Леко порозовява
            Debug.Log("Влизане във ФАЗА 2!");
        }

        // ФАЗА 3: Под 30% кръв (Лудото състояние)
        if (currentHealth <= maxHealth * 0.3f && currentPhase == 2)
        {
            currentPhase = 3;
            GetComponent<SpriteRenderer>().color = Color.red; // Става чисто червен
            Debug.Log("Влизане във ФАЗА 3!");
        }
    }

    public void ShowHealthBar()
    {
        if (healthSlider != null)
        {
            healthSlider.gameObject.SetActive(true);
        }
    }

    void Die()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        // Скриваме и лентата при смърт
        if (healthSlider != null) healthSlider.gameObject.SetActive(false);

        Destroy(gameObject);
        Debug.Log("Босът е победен!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerLaser"))
        {
            TakeDamage(10f);
            Destroy(other.gameObject);
            // Debug.Log е полезен, но можеш да го махнеш след тестовете, за да не пълни конзолата
        }
    }
}