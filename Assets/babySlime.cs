using UnityEngine;

public class babySlime : MonoBehaviour
{
    [Header("Health Attributes")]
    [SerializeField] private int health = 100;
    [SerializeField] private int maxHealth = 100;

    [SerializeField] public GameObject player;

        // boxcast attributes for grounded check
    private Rigidbody2D rb;
    private float speed = 9f;

    public int bounceForce = 5;

    public float bounceRate = 1f;

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
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("Bounce", 0, bounceRate);
        
        
    }


    // Update is called once per frame
    void Update()
    {


        print("i did it");
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

    public void FixedUpdate(){
        // devide veloc9ity by 2 to slow down the slime
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * .9F, rb.linearVelocity.y * .9F);
    }

    private void Bounce(){
        // move toward player
        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.AddForce(direction * speed, ForceMode2D.Impulse);
        Debug.Log("Slime is bouncing towards the player every 3 seconds.");
        bounce++;
    }

    void OnDrawGizmos(){
        
    }
}
