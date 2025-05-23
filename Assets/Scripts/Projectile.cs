using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;

    [Header("Stats")]
    private float damage;
    private int remainingPenetrations;
    private bool isCritical;
    private float critMultiplier;
    private Vector2 firePosition;
    private IObjectPool<Projectile> pool;

    [Header("Settings")]
    [SerializeField] private float speed = 25f;
    [SerializeField] private float lifetime = 3f;


    public TrailRenderer Trail => trailRenderer;

    public void Initialize(float damage, int maxPenetrations, bool isCritical, 
                         float critMultiplier, Vector2 firePos, IObjectPool<Projectile> pool)
    {
        this.damage = damage;
        this.remainingPenetrations = maxPenetrations;
        this.isCritical = isCritical;
        this.critMultiplier = critMultiplier;
        this.firePosition = firePos;
        this.pool = pool;

        col.enabled = true;
        rb.linearVelocity = transform.right * speed;
        
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.emitting = true;
        }

        CancelInvoke();
        Invoke(nameof(ReleaseProjectile), lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.isTrigger) return;

        if (other.CompareTag("Enemy"))
        {
            
            babySlime babySlime = other.GetComponent<babySlime>();
            Debug.Log("HIT ");
            float finalDamage = isCritical ? damage * critMultiplier : damage;
            Vector2 knockbackDir = new Vector2(0, 0); // Knockback direction (diagonal)
            float knockbackStrength = 10.0f;
            babySlime.TakeDamage(finalDamage, knockbackDir, knockbackStrength);
            ReleaseProjectile();
        }

        remainingPenetrations--;
        if (remainingPenetrations <= 0) ReleaseProjectile();
    }

    private void ReleaseProjectile()
    {
        if (trailRenderer != null)
        {
            trailRenderer.emitting = false;
            trailRenderer.Clear();
        }

        col.enabled = false;
        rb.linearVelocity = Vector2.zero;
        
        if (pool != null)
        {
            pool.Release(this);
            
        }
        else
        {
            Debug.Log("TF");
            Destroy(gameObject);
        }
    }

    public void TakeDamage(float amount, Vector2 knockbackDirection, float knockbackForce){
        Debug.Log("");
    }
}