using UnityEngine;

public class BS_OLD : MonoBehaviour
{
    [Header("Health Attributes")]
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;

    [SerializeField] private GameObject player;

        // boxcast attributes for grounded check
    [Header("Grounded Check Raycast")]
    [SerializeField] public Vector2 boxSize;
    [SerializeField] public float castDistance;
    [SerializeField] public LayerMask groundLayer;
    private Rigidbody2D rb;
    private float speed = 2f;

    public int bounceForce = 5;

    public int bounce = 0;
    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health < 0)
        {
            health = 0;
        }
    }
    public void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Add force to move left
        
    }

    // Update is called once per frame
    void Update()
    {

        if(isGrounded()){
            //add bounce force 
            
            if (rb.linearVelocity.y <= 0)
            {
                rb.AddForce(new Vector2(0, bounceForce), ForceMode2D.Impulse);

                // add horizontal force based on bounce count
                if (bounce <= 3)
                {
                rb.AddForce(new Vector2(-speed, 0), ForceMode2D.Impulse);
                }
                else if (bounce <= 6)
                {
                rb.AddForce(new Vector2(speed, 0), ForceMode2D.Impulse);
                }
                else
                {
                bounce = 0;
                }
                Debug.Log("Grounded: " + isGrounded() + " Bounce Count: " + bounce);
                bounce++;
            }
        }

        //death
        if (health <= 0)
        {
            Destroy(gameObject);
        }


        
        // Flip sprite based on movement direction
        if (rb.linearVelocity.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x)*-1, transform.localScale.y, transform.localScale.z);
        }
        else if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }

    public bool isGrounded(){
        if(Physics2D.BoxCast(transform.position, boxSize, 0, -transform.up, castDistance, groundLayer)){
            return true;
        } else {
            return false;
        }
    }

    void OnDrawGizmos(){
        Gizmos.DrawWireCube(transform.position - transform.up * castDistance, boxSize);
    }
}
