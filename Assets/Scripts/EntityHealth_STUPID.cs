using UnityEngine;

public class EntityHealth_STUPID : MonoBehaviour
{
    [Header("Entity Health Settings")]
    [SerializeField] protected float maxHealth = 100;
    [SerializeField] public float currentHealth = 100;
    [SerializeField] public GameObject gemPrefab; // Changed variable name for clarity
    [SerializeField] [Range(0, 1)] float gemDropChance = 1f; // Optional drop chance

    void Start()
    {
        currentHealth = maxHealth; // Initialize health at start
    }

    public void RecieveDamage(float amount)
    {
        currentHealth -= amount;
        Debug.Log("Enemy recieved damage");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        TrySpawnGem();
        Destroy(gameObject);
    }

    private void TrySpawnGem()
    {
        if (gemPrefab != null && Random.value <= gemDropChance)
        {
            Instantiate(gemPrefab, transform.position, Quaternion.identity);
        }
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        Debug.Log($"Healed: {amount}. Current health: {currentHealth}");
    }

    public float GetHP() => currentHealth;
    public float GetMaxHP() => maxHealth;
}