using UnityEngine;

public class DefaultCharacter : CharacterBlueprint
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private PersonalHealthBar healthBar;
    
    private int currentHealth;

    protected override void Awake()
    {
        base.Awake();
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }


    private void Die()
    {
        // Add death effects/animation
        Debug.Log("Player Died");
        gameObject.SetActive(false);
        
        // Trigger game over or respawn logic
    }

    protected override void Update()
    {
        base.Update();
    }

}