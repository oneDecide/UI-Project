using UnityEngine;
using UnityEngine.Pool;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private ParticleSystem impactVfx;
    [SerializeField] private GameObject critImpactVfx;
    [SerializeField] private AudioClip impactSound;
    [Range(0, 1)] [SerializeField] private float impactVolume = 0.8f;

    [Header("Movement")]
    [SerializeField] private float speed = 25f;
    [SerializeField] private float maxLifetime = 3f;

    private IObjectPool<Projectile> pool;
    private Rigidbody2D rb;
    private Collider2D col;
    
    // Runtime stats
    private float damage;
    private int remainingPenetrations;
    private bool isCritical;
    private float critMultiplier;
    private Vector2 firePosition;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void Initialize(float damage, int maxPenetrations, bool isCritical, float critMultiplier, Vector2 firePos, IObjectPool<Projectile> projectilePool)
    {
        this.damage = damage;
        this.remainingPenetrations = maxPenetrations;
        this.isCritical = isCritical;
        this.critMultiplier = critMultiplier;
        this.firePosition = firePos;
        this.pool = projectilePool;

        col.enabled = true;
        rb.linearVelocity = transform.right * speed;
        
        if(trail != null)
        {
            trail.Clear();
            trail.emitting = true;
        }

        CancelInvoke();
        Invoke(nameof(ReleaseProjectile), maxLifetime);
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if(other.CompareTag("Player") || other.isTrigger) return;

    //     if(other.TryGetComponent<IDamageable>(out var damageable))
    //     {
    //         float finalDamage = isCritical ? damage * critMultiplier : damage;
    //         damageable.TakeDamage(finalDamage, isCritical);
    //     }

    //     PlayImpactEffects(other.transform.position);
    //     remainingPenetrations--;

    //     if(remainingPenetrations <= 0) ReleaseProjectile();
    // }

    private void PlayImpactEffects(Vector3 position)
    {
        if(impactVfx != null) Instantiate(impactVfx, position, Quaternion.identity).Play();
        if(isCritical && critImpactVfx != null) Instantiate(critImpactVfx, position, Quaternion.identity);
        if(impactSound != null) AudioSource.PlayClipAtPoint(impactSound, position, impactVolume);
    }

    private void ReleaseProjectile()
    {
        if(trail != null) trail.emitting = false;
        col.enabled = false;
        rb.linearVelocity = Vector2.zero;
        pool?.Release(this);
    }
}