using UnityEngine;
using UnityEngine.UI; // Necesar pentru componenta Slider

public class BossHealth : MonoBehaviour
{
    [Header("Setări Viață")]
    public int maxHealth = 10; // Câte obiecte trebuie să înghită pentru a muri
    private int currentHealth;

    [Header("Referințe UI")]
    public Slider healthBar; // Trage bara ta de viață (BossHealthBar) aici în Inspector

    [Header("Efecte la Moarte")]
    [Tooltip("Trage aici Prefab-ul cu explozia finală, mai mare!")]
    public GameObject deathExplosionPrefab;

    void Start()
    {
        currentHealth = maxHealth;
        
        // Sincronizăm bara vizuală cu viața reală
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
        }
    }

    // Funcția apelată când este lovit de un obiect
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        
        if (healthBar != null)
        {
            healthBar.value = currentHealth;
        }

        Debug.Log("Boss-ul a fost lovit! Viață rămasă: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("🎉 Boss-ul a fost ÎNVINS!");

        // --- COD NOU: EXPLODĂM LA MOARTE ---
        if (deathExplosionPrefab != null)
        {
            // Instanțiem explozia uriașă la poziția boss-ului.
            // Folosim Quaternion.identity pentru rotație zero.
            GameObject explosion = Instantiate(deathExplosionPrefab, transform.position, Quaternion.identity);

            // Ne asigurăm că efectul se distruge după ce termină (ex: 3 secunde pentru particule)
            Destroy(explosion, 3f);
        }
        // -----------------------------------
        
        // Ascundem bara de viață
        if (healthBar != null) healthBar.gameObject.SetActive(false);
        
        // Momentan îl distrugem. Mai târziu aici vom pune logica de Win / Trecere la alt nivel
        Destroy(gameObject);
    }
}