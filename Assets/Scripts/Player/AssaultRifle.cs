using UnityEngine;
using UnityEngine.Pool;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class AssaultRifle : MonoBehaviour
{
    [Header("Base Stats")]
    public int damage = 15;
    public float fireRate = 10f;
    public int punchThrough = 1;
    public float multishot = 1f;
    public float critChance = 0.2f;
    public float critMultiplier = 1.5f;
    public int magazineCapacity = 30;
    public float reloadTime = 4f;

    [Header("References")]
    [SerializeField] private Transform firePoint;
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private AudioClip fireSound;
    [SerializeField] private AudioClip reloadSound;
    [Range(0, 1)] [SerializeField] private float volume = 0.8f;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;

    [Header("Positioning")]
    [SerializeField] private Transform weaponPivot;
    [SerializeField] private float orbitRadius = 0.5f;
    [SerializeField] private float rotationSpeed = 15f;
    [SerializeField] private float orbitSmoothSpeed = 10f;
    [SerializeField] private bool forceLeftSide = true;
    [SerializeField] private bool instantAim = false;

    [Header("Recoil")]
    [SerializeField] private float positionalRecoilDistance = 0.1f;
    [SerializeField] private float rotationalRecoilAngle = 3f;
    [SerializeField] private float recoilRecoverySpeed = 8f;
    [SerializeField] private Vector2 recoilRandomRange = new Vector2(-15f, 15f);
    [SerializeField] private TMP_Text ammoText;
    [SerializeField] private TMP_Text ammoWarnText;
    private const int MAX_MODS = 4;
    private List<WeaponMod> activeMods = new List<WeaponMod>();
    private ObjectPool<Projectile> projectilePool;
    private int currentAmmo;
    private float nextFireTime;
    private bool isReloading;
    private AudioSource audioSource;
    private Camera mainCamera;
    private bool isFiring;
    private Vector3 recoilOffset = Vector3.zero;
    private Vector3 targetOrbitPosition;
    private bool isOnLeftSide;
    private Quaternion targetRotation;
    private Quaternion recoilRotationOffset = Quaternion.identity;

    private void Awake()
    {
        ammoWarnText.enabled = false;
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }

        if (weaponSpriteRenderer == null)
        {
            weaponSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        mainCamera = Camera.main;
        InitializePool();
        currentAmmo = magazineCapacity;
        
        if (weaponPivot != null)
        {
            transform.position = weaponPivot.position + Vector3.left * orbitRadius;
        }
    }

    private void InitializePool()
    {
        projectilePool = new ObjectPool<Projectile>(
            createFunc: () => Instantiate(projectilePrefab),
            actionOnGet: (projectile) => {
                projectile.transform.SetPositionAndRotation(firePoint.position, firePoint.rotation);
                projectile.gameObject.SetActive(true);
            },
            actionOnRelease: (projectile) => {
                if (projectile.Trail != null) projectile.Trail.Clear();
                projectile.gameObject.SetActive(false);
            },
            actionOnDestroy: (projectile) => Destroy(projectile.gameObject),
            collectionCheck: true,
            defaultCapacity: 20,
            maxSize: 50
        );
    }

    private void Update()
    {
        if (weaponPivot == null) return;

        CalculateTargetPosition();
        HandleAiming();
        HandleInput();
        HandleRecoil();
        UpdateSpriteFlip();
        UpdateAmmoUI();
    }

    private void CalculateTargetPosition()
    {
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0;

        Vector3 directionToMouse = (mousePosition - weaponPivot.position).normalized;
        if (directionToMouse == Vector3.zero) directionToMouse = Vector3.left;

        isOnLeftSide = directionToMouse.x < 0;
        if (forceLeftSide && !isOnLeftSide)
        {
            directionToMouse = -directionToMouse;
            isOnLeftSide = true;
        }

        targetOrbitPosition = weaponPivot.position + (directionToMouse * orbitRadius);
        float angle = Mathf.Atan2(directionToMouse.y, directionToMouse.x) * Mathf.Rad2Deg;
        targetRotation = Quaternion.Euler(0, 0, angle);
    }

    private void HandleAiming()
    {
        // Calculate base position without recoil
        Vector3 basePosition = Vector3.Lerp(transform.position - recoilOffset, targetOrbitPosition, orbitSmoothSpeed * Time.deltaTime);
        Vector3 directionFromPivot = (basePosition - weaponPivot.position).normalized;
        
        // Apply exact radius then add recoil
        transform.position = weaponPivot.position + (directionFromPivot * orbitRadius) + recoilOffset;

        // Apply rotation with recoil
        Quaternion baseRotation = instantAim ? targetRotation : 
            Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        
        transform.rotation = baseRotation * recoilRotationOffset;
    }

    private void UpdateSpriteFlip()
    {
        if (weaponSpriteRenderer != null)
        {
            weaponSpriteRenderer.flipY = isOnLeftSide;
        }
    }

    private void HandleInput()
    {
        if (Input.GetMouseButtonDown(0)) isFiring = true;
        if (Input.GetMouseButtonUp(0)) isFiring = false;

        if (isFiring) TryFire();
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < magazineCapacity) StartCoroutine(Reload());
    }

    private void HandleRecoil()
    {
        // Smoothly recover both types of recoil
        recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, recoilRecoverySpeed * Time.deltaTime);
        recoilRotationOffset = Quaternion.Slerp(recoilRotationOffset, Quaternion.identity, recoilRecoverySpeed * Time.deltaTime);
    }

    private void TryFire()
    {
        if (CanFire()) Fire();
    }

    private bool CanFire()
    {
        return Time.time >= nextFireTime && currentAmmo > 0 && !isReloading;
    }

    private void Fire()
    {
        nextFireTime = Time.time + 1f/fireRate;
        currentAmmo--;

        ApplyRecoil();
        //muzzleFlash.Play();
        audioSource.PlayOneShot(fireSound, volume);

        int shotsToFire = CalculateMultishot();
        for (int i = 0; i < shotsToFire; i++)
        {
            float spread = CalculateSpread(i, shotsToFire);
            Quaternion rotation = firePoint.rotation * Quaternion.Euler(0, 0, spread);

            Projectile projectile = projectilePool.Get();
            projectile.Initialize(
                damage,
                punchThrough,
                Random.value <= critChance,
                critMultiplier,
                firePoint.position,
                projectilePool
            );
            projectile.transform.rotation = rotation * recoilRotationOffset;
        }
    }

    private void ApplyRecoil()
    {
        // Positional recoil (push back)
        float randomAngle = Random.Range(recoilRandomRange.x, recoilRandomRange.y);
        Vector3 recoilDirection = Quaternion.Euler(0, 0, randomAngle) * -firePoint.right;
        recoilOffset = recoilDirection * positionalRecoilDistance;

        // Rotational recoil (kick up)
        float kickAngle = Random.Range(-rotationalRecoilAngle, rotationalRecoilAngle);
        recoilRotationOffset = Quaternion.Euler(0, 0, kickAngle);
    }

    private int CalculateMultishot()
    {
        int guaranteedShots = Mathf.FloorToInt(multishot);
        float extraShotChance = multishot - guaranteedShots;
        return guaranteedShots + (Random.value < extraShotChance ? 1 : 0);
    }

    private float CalculateSpread(int shotIndex, int totalShots)
    {
        if (totalShots <= 1) return 0f;
        
        float baseAngle = 5f;
        bool hasFractionalShots = (multishot % 1) > 0.1f;
        float additionalSpread = hasFractionalShots ? 2.5f : 0f;
        
        return Mathf.Lerp(
            -baseAngle - additionalSpread, 
            baseAngle + additionalSpread, 
            (float)shotIndex / (totalShots - 1)
        );
    }

public float ReloadTime { get; private set; }
public float ReloadStartTime { get; private set; }

private IEnumerator Reload()
{
    isReloading = true;
    ReloadStartTime = Time.time;
    ReloadTime = reloadTime;
    
    audioSource.PlayOneShot(reloadSound, volume);
    yield return new WaitForSeconds(reloadTime);
    
    currentAmmo = magazineCapacity;
    isReloading = false;
}
    public bool TryAddMod(WeaponMod mod)
    {
        if (activeMods.Count >= MAX_MODS || activeMods.Contains(mod))
            return false;

        activeMods.Add(mod);
        mod.ApplyMod(this);
        return true;
    }

    public bool RemoveMod(WeaponMod mod)
    {
        if (!activeMods.Contains(mod))
            return false;

        activeMods.Remove(mod);
        mod.RemoveMod(this);
        RecalculateStats();
        return true;
    }

    private void RecalculateStats()
    {
        // Reset to base values
        damage = 15;
        fireRate = 10f;
        punchThrough = 1;
        multishot = 1f;
        critChance = 0.2f;
        critMultiplier = 1.5f;
        magazineCapacity = 30;
        reloadTime = 4f;
        instantAim = false;

        // Reapply all mods
        foreach (var mod in activeMods)
        {
            mod.ApplyMod(this);
        }
    }

     private void UpdateAmmoUI()
    {
        if(currentAmmo < magazineCapacity*.2){
            ammoWarnText.enabled = true;
        }
        else{
            ammoWarnText.enabled = false;
        }
        if (IsReloading)
        {
            // Calculate remaining reload time
            float reloadProgress = ReloadTime - (Time.time - ReloadStartTime);
            ammoText.text = "Ammo: Reloading: " + reloadProgress.ToString("F1");
        }
        else
        {
            // Show ammo count
            ammoText.text = $"Ammo: {CurrentAmmo} / {MagazineSize}";
        }
    }

    public int CurrentAmmo => currentAmmo;
    public int MagazineSize => magazineCapacity;
    public bool IsReloading => isReloading;
    public IReadOnlyList<WeaponMod> ActiveMods => activeMods.AsReadOnly();
}