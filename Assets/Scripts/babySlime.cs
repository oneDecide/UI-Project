using UnityEngine;

public class babySlime : MonoBehaviour
{
    //[Header("Health Attributes")]

    // script component for health attributes
    private EntityHealth healthScript; 
    [SerializeField] public GameObject player;

        // boxcast attributes for grounded check
    private Rigidbody2D rb;
    // private float speed = 9f;

    public int bounceForce = 5;

    public float bounceRate = 1f;

    public int bounce = 0;

    [SerializeField] public GameObject gem = null;

    [SerializeField] public GameObject heart = null;

    public bool hasAttacked = false;

    [SerializeField] public int slimeDamage = 10;

    [Header("Slime Hitbox Stuff")]

    [SerializeField] public Vector2 hitboxSize = new Vector2(1f, 1f);
    [SerializeField] public float hitboxDistanceHorizontal = 0f;
    [SerializeField] public float hitboxDistanceVertical = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }

        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("Bounce", 0, bounceRate);
        
        // Get the EntityHealth component from the GameObject
        healthScript = GetComponent<EntityHealth>();
        if (healthScript == null)
        {
            //debug.logError("EntityHealth component not found on this GameObject.");
        }
    }


    // Update is called once per frame
    void Update()
    {

        
        
        // Flip sprite based on movement direction
        if (rb.linearVelocity.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x)*-1, transform.localScale.y, transform.localScale.z);
        }
        else if (rb.linearVelocity.x < 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }

        if(hasAttacked == false){
            doAttacks();
        }


    }

    public void FixedUpdate(){
        // devide veloc9ity by 2 to slow down the slime
        rb.linearVelocity = new Vector2(rb.linearVelocity.x * .9F, rb.linearVelocity.y * .9F);
    }

    private void Bounce(){
        // move toward player

        // get random number between .5 and 1.5
        float random = Random.Range(.7f, 1.7f);


        Vector2 direction = (player.transform.position - transform.position).normalized;
        rb.AddForce(direction * bounceForce * random, ForceMode2D.Impulse);
        //debug.log("Slime is bouncing towards the player every 3 seconds.");
        bounce++;
        //take tamage per bounce
        if (bounce >= 3){
            
            bounce = 0;
        }

        hasAttacked = false;
        // healthScript.takeDamage(1);
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + new Vector3(hitboxDistanceHorizontal, hitboxDistanceVertical), hitboxSize);
    }

    public void TakeDamage(float amount, Vector2 knockbackDirection, float knockbackForce){
        healthScript.takeDamage(amount);
        rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode2D.Impulse);
        //debug.log("Took damage: " + amount + ". Current health: " + healthScript.getHP());
        if(healthScript.getHP() <= 0){
            // spawn gem
            if (gem != null){
                Instantiate(gem, transform.position, Quaternion.identity);
            }
            // spawn heart 1/3 chance
            float random = Random.Range(0f, 1f);
            if (random <= .33f && heart != null){
                Instantiate(heart, transform.position, Quaternion.identity);
            }


            Destroy(gameObject);
        }
    }

    public void doAttacks(){

        Debug.Log("In attack method");
        RaycastHit2D[] hits = Physics2D.BoxCastAll(transform.position + new Vector3(hitboxDistanceHorizontal, hitboxDistanceVertical), hitboxSize, 0f, Vector2.zero, 0f, LayerMask.GetMask("PlayerHitbox"));
        foreach (RaycastHit2D hit in hits){
            if(hit.collider.gameObject.transform.parent != this.gameObject && hit.collider.gameObject.transform.parent.GetComponent<characterController>())
            {
                Debug.Log("HIT PLAYER");
                characterController player = hit.collider.gameObject.transform.parent.GetComponent<characterController>();
                player.TakeDamage(slimeDamage);
                hasAttacked = true;
            }
            if(hit.collider.gameObject.transform.parent != this.gameObject && hit.collider.gameObject.transform.parent.GetComponent<DefaultCharacter>())
            {
                Debug.Log("HIT PLAYER");
                DefaultCharacter playerDC = hit.collider.gameObject.transform.parent.GetComponent<DefaultCharacter>();
                playerDC.TakeDamage(slimeDamage);
                hasAttacked = true;
            }
        }
    }
}
