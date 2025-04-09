using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DefaultCharacter : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 15f;
    
    private Rigidbody2D rb;
    private Vector2 movementInput;
    private Vector2 currentVelocity;
    private bool canMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ConfigureRigidbody();
    }

    private void ConfigureRigidbody()
    {
        rb.gravityScale = 0;
        rb.linearDamping = 5;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Update()
    {
        if(canMove)
        {
            HandleInput();
        }
    }

    private void HandleInput()
    {
        // Get raw input for instant response
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        // Normalize diagonal movement
        movementInput = new Vector2(horizontal, vertical).normalized;
    }

    private void FixedUpdate()
    {
        if(canMove)
        {
            HandleMovementPhysics();
        }
    }

    private void HandleMovementPhysics()
    {
        Vector2 targetVelocity = movementInput * moveSpeed;
        Vector2 velocityDifference = targetVelocity - rb.linearVelocity;
        
        // Apply acceleration force
        Vector2 force = velocityDifference * acceleration;
        rb.AddForce(force, ForceMode2D.Force);
        
        // Manual deceleration when no input
        if(movementInput == Vector2.zero)
        {
            rb.linearVelocity = Vector2.MoveTowards(
                rb.linearVelocity, 
                Vector2.zero, 
                deceleration * Time.fixedDeltaTime
            );
        }
    }

    public void SetMovementEnabled(bool state)
    {
        canMove = state;
        if(!state) rb.linearVelocity = Vector2.zero;
    }
}