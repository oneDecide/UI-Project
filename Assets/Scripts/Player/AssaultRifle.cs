using UnityEngine;
using UnityEngine.Pool;
using System.Collections.Generic;
using System.Collections;

public class AssaultRifle : MonoBehaviour
{
    [Header("Base Stats")]
    public int damage = 15;
    public float fireRate = 10f;
    public int punchThrough = 1;
    public float multishot = 1f;
    public float critRate = 0.2f;
    public float critMultiplier = 1.5f;
    public int magazineCapacity = 30;
    public float reloadTime = 4f;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private ObjectPool<Projectile> projectilePool;

    [Header("Mod System")]
    [SerializeField] private List<WeaponMod> activeMods = new List<WeaponMod>();
    private const int MAX_MODS = 4;

    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;

    private void Awake()
    {
        // projectilePool = new ObjectPool<Projectile>(
        //     () => Instantiate(projectilePrefab),
        //     projectile => projectile.gameObject.SetActive(true),
        //     projectile => projectile.gameObject.SetActive(false),
        //     Destroy,
        //     30
        // );
    }

    private void Update()
    {
        if(Input.GetButton("Fire1") && CanFire()) Fire();
        if(Input.GetKeyDown(KeyCode.R) && !isReloading) StartCoroutine(Reload());
    }

    private bool CanFire() => Time.time >= nextFireTime && currentAmmo > 0 && !isReloading;

    private void Fire()
    {
        nextFireTime = Time.time + 1f / fireRate;
        currentAmmo--;

        int shotsToFire = CalculateMultishot();
        for(int i = 0; i < shotsToFire; i++)
        {
            float spread = CalculateSpread(i, shotsToFire);
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, spread);
            
            var projectile = projectilePool.Get();
            projectile.Initialize(
                damage, 
                punchThrough, 
                Random.value <= critRate, 
                critMultiplier,
                firePoint.position,
                projectilePool
            );
            projectile.transform.SetPositionAndRotation(firePoint.position, rotation);
        }
    }

    private int CalculateMultishot()
    {
        int guaranteedShots = Mathf.FloorToInt(multishot);
        float extraShotChance = multishot - guaranteedShots;
        return guaranteedShots + (Random.value < extraShotChance ? 1 : 0);
    }

    private float CalculateSpread(int shotIndex, int totalShots)
    {
        if(totalShots <= 1) return 0f;
        float baseSpread = 5f;
        return Mathf.Lerp(-baseSpread, baseSpread, (float)shotIndex / (totalShots - 1));
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = magazineCapacity;
        isReloading = false;
    }

    public bool TryAddMod(WeaponMod mod)
    {
        if(activeMods.Count >= MAX_MODS) return false;
        
        activeMods.Add(mod);
        mod.ApplyMod(this);
        return true;
    }

    public void RemoveMod(WeaponMod mod)
    {
        if(activeMods.Remove(mod))
        {
            mod.RemoveMod(this);
            RecalculateStats();
        }
    }

    private void RecalculateStats()
    {
        // Reset to base values first
        // Then reapply all mods
        foreach(var mod in activeMods)
        {
            mod.ApplyMod(this);
        }
    }
}

// 3. WEAPON MOD BASE CLASS
public abstract class WeaponMod : ScriptableObject
{
    public string modName;
    public Sprite icon;
    [TextArea] public string description;

    public abstract void ApplyMod(AssaultRifle weapon);
    public abstract void RemoveMod(AssaultRifle weapon);
}