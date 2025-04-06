using UnityEngine;

public abstract class CharacterBlueprint : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float health = 100f;
    
    protected Rigidbody2D rb;
    protected Vector2 movementInput;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        HandleInput();
    }

    protected virtual void FixedUpdate()
    {
        HandleMovement();
    }

    protected virtual void HandleInput()
    {
        movementInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;
    }

    protected virtual void HandleMovement()
    {
        rb.linearVelocity = movementInput * moveSpeed;
    }
}