using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class DefaultCharacter : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int enemyCollisionDamage = 10;

    [Header("Gem Settings")]
    [SerializeField] private int modAquireGauge = 0;
    [SerializeField] private int gemValue = 10;

    [Header("UI Elements")]
    [SerializeField] private TMP_Text ammoDisplay;

    private Rigidbody2D rb;
    private Collider2D col;
    private Vector2 movementInput;
    private bool canMove = true;
    private int currentHealth;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        currentHealth = maxHealth;
        ConfigurePhysics();
    }

    private void ConfigurePhysics()
    {
        // Rigidbody configuration
        rb.gravityScale = 0;
        rb.linearDamping = 5;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        // Collider configuration
        col.isTrigger = true;
    }

    private void Update()
    {
        if (canMove) HandleInput();
    }

    private void HandleInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        movementInput = new Vector2(horizontal, vertical).normalized;
    }

    private void FixedUpdate()
    {
        if (canMove) HandleMovementPhysics();
    }

    private void HandleMovementPhysics()
    {
        Vector2 targetVelocity = movementInput * moveSpeed;
        Vector2 velocityDifference = targetVelocity - rb.linearVelocity;
        
        rb.AddForce(velocityDifference * acceleration, ForceMode2D.Force);

        if (movementInput == Vector2.zero)
        {
            rb.linearVelocity = Vector2.MoveTowards(
                rb.linearVelocity,
                Vector2.zero,
                deceleration * Time.fixedDeltaTime
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy")) HandleEnemyCollision();
        else if (other.CompareTag("Gem")) HandleGemCollection(other.gameObject);
    }

    private void HandleEnemyCollision()
    {
        currentHealth -= enemyCollisionDamage;
        Debug.Log($"Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0) Die();
    }

    private void HandleGemCollection(GameObject gem)
    {
        modAquireGauge += gemValue;
        Destroy(gem);
        Debug.Log($"Gem collected! Mod Aquire Gauge: {modAquireGauge}");
    }

    private void Die()
    {
        canMove = false;
        rb.linearVelocity = Vector2.zero;
        Debug.Log("Player defeated!");
        // Add death animation/effects here
    }

    public void SetMovementEnabled(bool state) => canMove = state;
}