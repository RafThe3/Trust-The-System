using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : UnityEngine.MonoBehaviour
{
    [Header("Settings")]
    public bool canShoot = true;
    public bool isAutomatic = false;
    public bool infiniteAmmo = false;
    public ShootingType shootingType = ShootingType.Line;

    [Header("Stats")]
    [Min(0)] public int startingAmmo = 30;
    [Min(0)] public int maxAmmo = 150;
    [Min(0)] public int startingRounds = 1;
    [Min(0)] public float damage = 1;
    [Min(0)] public float fireRate = 1;
    [Min(0)] public float reloadTime = 1;

    [Header("ADS & Bloom")]
    public bool isAiming = false;
    public float adsFOV = 40f;
    public float adsSpeed = 10f;
    public float bloomRate = 0.02f; // How fast spread increases while firing
    // public float bloomRecovery = 0.1f; // How fast it shrinks back
    public float adsBloomRecovery = 15f;
    public float hipBloomRecovery = 5f;

    [Header("Hipfire & Recoil")]
    public float hipfireSpread = 0.05f;
    public float adsSpread = 0.01f;
    public float maxSpread = 0.2f;
    public Vector3 recoilRotation = new(2f, 1f, 0f); // X = Up/Down, Y = Side
    public float adsRecoilMultiplier = 0.5f; // 0.5 means 50% less recoil when aiming
    public float recoilSnappiness = 10f;
    public float adsRecoilSnappiness = 0.8f;
    public float recoilReturnSpeed = 5f;

    [Header("Line Shooting")]
    public float maxDistance = 1;

    [Header("Projectile Shooting")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 1;
    public float projectileLife = 1;
    public Transform projectileSpawnPoint;

    [Header("UI")]
    public TextMeshProUGUI ammoText;
    //public GameObject impactEffect;
    //public GameObject bloodEffect;
    public ParticleSystem muzzleFlash;
    public RectTransform crosshairUI; // Assign a UI Image (e.g. 4 lines)
    public float crosshairScaleMultiplier = 500f;
    public GameObject tracerPrefab;
    public float tracerSpeed = 500f;
    public float tracerLifetime = 0.1f;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip shootSFX;
    public AudioClip reloadSFX;

    private class RayBullet
    {
        public Vector3 currentPosition;
        public Vector3 previousPosition; // Needed to draw the line segment
        public Vector3 velocity;
        public float timeRemaining;
        public LineRenderer line;

        public RayBullet(Vector3 pos, Vector3 vel, float life, LineRenderer lr)
        {
            currentPosition = pos;
            previousPosition = pos;
            velocity = vel;
            timeRemaining = life;
            line = lr;
        }
    }
    private readonly List<RayBullet> activeBullets = new();

    // Weapon variables
    private float fireTimer = 0;
    private int currentAmmo = 0, reserveAmmo = 0;
    public float currentSpread = 0; // How much the bullet deviates
    private bool isShooting = false;
    private bool isReloading = false;

    // UI variables
    private Camera mainCamera;
    private float hipFOV = 0;

    // Recoil calculation variables
    private Vector3 targetRotation;
    private Vector3 currentRotation;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        fireTimer = fireRate;
        currentAmmo = startingAmmo;
        reserveAmmo = Mathf.Min(maxAmmo, startingAmmo * startingRounds);
        if (maxAmmo == 0) maxAmmo = reserveAmmo;
        hipFOV = mainCamera.fieldOfView;
        currentSpread = hipfireSpread;
    }

    private void Update()
    {
        UpdateVariables();
        HandleRecoilMath();
        HandleADS();
        HandleBloom();
        HandleReloading();
        HandleShooting();
        for (int i = activeBullets.Count - 1; i >= 0; i--)
        {
            RayBullet b = activeBullets[i];

            // Save the current position as the new 'tail' for this frame
            b.previousPosition = b.currentPosition;

            Vector3 step = b.velocity * Time.deltaTime;

            // Raycast Check
            if (Physics.Raycast(b.currentPosition, b.velocity.normalized, out RaycastHit hit, step.magnitude))
            {
                if (hit.collider.TryGetComponent(out Enemy enemy)) enemy.TakeDamage(damage);
                CleanupBullet(i);
                continue;
            }

            // Update Physics Data
            b.currentPosition += step;
            b.velocity += Physics.gravity * Time.deltaTime;
            b.timeRemaining -= Time.deltaTime;

            // --- VISUALIZATION ---
            if (b.line != null)
            {
                // Point 0 is the back of the bullet
                b.line.SetPosition(0, b.previousPosition);
                // Point 1 is the front of the bullet
                b.line.SetPosition(1, b.currentPosition);
            }

            if (b.timeRemaining <= 0) CleanupBullet(i);
        }
    }

    private void LateUpdate()
    {
        UpdateCrosshair();
    }

    private void CleanupBullet(int index)
    {
        if (activeBullets[index].line != null)
        {
            Destroy(activeBullets[index].line.gameObject);
        }
        activeBullets.RemoveAt(index);
    }

    // Handles the shooting timing
    private void HandleShooting()
    {
        isShooting = ((isAutomatic && Input.GetMouseButton(0)) || (!isAutomatic && Input.GetMouseButtonDown(0))) && fireTimer >= fireRate && !isReloading;

        if (isShooting && canShoot)
        {

            switch (shootingType)
            {
                case ShootingType.Projectile:
                    ShootProjectile();
                    break;

                case ShootingType.Line:
                    ShootLine();
                    break;

                case ShootingType.RayProjectile:
                    ShootRayProjectile();
                    break;

                default:
                    break;
            }
        }
    }

    // Handles the reloading timing
    private void HandleReloading()
    {
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < startingAmmo)
        {
            StartCoroutine(Reload(reloadTime));
        }

        ammoText.text = $"{currentAmmo} | {reserveAmmo}";
    }

    // Updates the crosshair with respect to the bullet spread
    private void UpdateCrosshair()
    {
        if (crosshairUI)
        {
            // Adjust UI size based on current spread

            // 1. Calculate size (make sure multiplier is high, e.g. 1000)
            float size = currentSpread * crosshairScaleMultiplier;

            // 2. Clamp it so it doesn't disappear or fill the whole screen
            size = Mathf.Clamp(size, currentSpread * 100, maxSpread * 1000);

            // 3. Apply directly to width and height
            crosshairUI.sizeDelta = new Vector2(size, size);
        }
    }

    // Updates the UI elements and other variables
    private void UpdateVariables()
    {
        bool isGamePaused = Time.timeScale == 0;
        canShoot = !isGamePaused;
        fireTimer += Time.deltaTime;
        ammoText.enabled = !isGamePaused;
    }

    // Handles the ADS timing and mechanic
    private void HandleADS()
    {
        isAiming = Input.GetMouseButton(1); // Right-click to aim

        hipFOV = PlayerPrefs.GetFloat("FOV");

        // Smoothly transition FOV
        float targetFOV = isAiming ? hipFOV - adsFOV : hipFOV;
        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * adsSpeed);
    }

    // Handles the bloom of the bullets
    private void HandleBloom()
    {
        // Shrink spread over time to the base value (ADS or Hipfire)
        float baseSpread = isAiming ? adsSpread : hipfireSpread;

        if (currentSpread > baseSpread)
        {
            float currentRecoverySpeed = isAiming ? adsBloomRecovery : hipBloomRecovery;
            currentSpread = Mathf.Lerp(currentSpread, baseSpread, Time.deltaTime * currentRecoverySpeed);
        }
        else
        {
            currentSpread = Mathf.Lerp(currentSpread, baseSpread, Time.deltaTime * adsSpeed);
        }

        // Clamps the spread
        currentSpread = Mathf.Clamp(currentSpread, adsSpread, maxSpread);
    }

    // This handles the smooth movement of the camera kick
    private void HandleRecoilMath()
    {
        float currentSnappiness = isAiming ? recoilSnappiness * adsRecoilSnappiness : recoilSnappiness;

        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, recoilReturnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, currentSnappiness * Time.fixedDeltaTime);
        mainCamera.transform.localRotation = Quaternion.Euler(currentRotation);
    }

    private void ApplyRecoil()
    {
        // Add random horizontal kick and consistent vertical kick
        //targetRotation += new Vector3(-recoilRotation.x, Random.Range(-recoilRotation.y, recoilRotation.y), 0);

        // Determine the strength of the kick
        float multiplier = isAiming ? adsRecoilMultiplier : 1f;

        // Apply scaled recoil
        float xRecoil = -recoilRotation.x * multiplier;
        float yRecoil = Random.Range(-recoilRotation.y, recoilRotation.y) * multiplier;

        targetRotation += new Vector3(xRecoil, yRecoil, 0);

        // Add "Bloom" every time we fire
        currentSpread += bloomRate;
    }

    private IEnumerator SpawnTracer(Vector3 targetPos)
    {
        GameObject tracer = Instantiate(tracerPrefab, projectileSpawnPoint.position, Quaternion.identity);
        tracer.GetComponent<TrailRenderer>().Clear();
        float distance = Vector3.Distance(tracer.transform.position, targetPos);
        float remainingDistance = distance;

        while (remainingDistance > Mathf.Epsilon)
        {
            tracer.transform.position = Vector3.MoveTowards(tracer.transform.position, targetPos, tracerSpeed * Time.deltaTime);
            remainingDistance -= tracerSpeed * Time.deltaTime;
            yield return null;
        }

        tracer.transform.position = targetPos;
        Destroy(tracer, tracerLifetime); // Brief delay for the trail to catch up
    }

    // Helper to get a direction with spread
    private Vector3 GetDirectionWithSpread()
    {
        // 1. Get a random point inside a circle (X and Y offset)
        // This creates a circular "cone" of fire rather than a square box
        Vector2 randomPoint = Random.insideUnitCircle * currentSpread;

        // 2. Get the camera's local axes
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;
        Vector3 up = mainCamera.transform.up;

        // 3. Combine the forward vector with the random offsets
        // This pushes the direction slightly to the side and slightly up/down
        Vector3 shotDirection = forward + (right * randomPoint.x) + (up * randomPoint.y);

        return shotDirection.normalized;
    }

    // Shoots a projectile
    private void ShootProjectile()
    {
        sfxSource.clip = shootSFX;
        if (currentAmmo <= 0) return;

        // 1. Get direction including your existing Spread logic
        Vector3 shootDir = GetDirectionWithSpread();

        // 2. Spawn the bullet
        GameObject bullet = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);

        // 3. Apply Velocity
        if (bullet.TryGetComponent(out Rigidbody rb))
        {
            // Physics logic: Velocity = Direction * Speed
            // Note: linearVelocity is the 2026 standard for Unity 6+
            rb.linearVelocity = (shootDir * projectileSpeed) + GameObject.FindWithTag("Player").GetComponent<CharacterController>().velocity;
        }

        // 4. Existing logic
        currentAmmo--;
        fireTimer = 0;
        sfxSource.Play();
        Destroy(bullet, projectileLife);
    }

    // Shoots a Ray
    private void ShootLine()
    {
        sfxSource.clip = shootSFX;

        if (currentAmmo > 0)
        {
            sfxSource.Play();
            muzzleFlash.Play();
            Vector3 bulletSpread = GetDirectionWithSpread();
            Ray shootDirection = new(mainCamera.transform.position, bulletSpread);
            StartCoroutine(SpawnTracer(shootDirection.origin + (bulletSpread * maxDistance)));
            ApplyRecoil();
            //GameObject impact;

            if (Physics.Raycast(shootDirection, out RaycastHit hit, maxDistance))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    Debug.Log("Hit enemy...");
                    //impact = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
                    hit.collider.GetComponent<Enemy>().TakeDamage(damage);
                }
                else
                {
                    //impact = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    if (hit.rigidbody)
                    {
                        hit.rigidbody.AddForce(-hit.normal * 60);
                    }
                }

                //Destroy(impact, 1);
            }

            currentAmmo--;
            fireTimer = 0;
        }
    }

    private void ShootRayProjectile()
    {
        sfxSource.clip = shootSFX;
        if (currentAmmo <= 0) return;

        GameObject tracerObj = Instantiate(tracerPrefab, projectileSpawnPoint.position, Quaternion.identity);
        LineRenderer lr = tracerObj.GetComponent<LineRenderer>();

        RayBullet newBullet = new(
            projectileSpawnPoint.position,
            GetDirectionWithSpread() * projectileSpeed,
            projectileLife,
            lr
        );

        activeBullets.Add(newBullet);
        ApplyRecoil();

        currentAmmo--;
        fireTimer = 0;
        sfxSource.Play();
    }


    // Reloads the gun
    public IEnumerator Reload(float duration)
    {
        bool hasReloaded = ((!infiniteAmmo && reserveAmmo > 0) || infiniteAmmo) && currentAmmo < startingAmmo;

        if (hasReloaded)
        {
            isReloading = true;
        }
        else
        {
            yield break;
        }

        yield return new WaitForSeconds(duration);

        if (hasReloaded)
        {
            sfxSource.PlayOneShot(reloadSFX);
        }


        if (infiniteAmmo)
        {
            if (currentAmmo < startingAmmo)
            {
                currentAmmo = startingAmmo;
            }
        }
        else
        {
            if (currentAmmo < startingAmmo && reserveAmmo > 0)
            {
                int reloadAmount = Mathf.Min(startingAmmo - currentAmmo, reserveAmmo);
                currentAmmo += reloadAmount;
                reserveAmmo -= reloadAmount;
            }
        }

        isReloading = false;
    }

    // Adds ammo into the gun's reserve ammo pocket
    public void AddAmmo(int amount)
    {
        if (reserveAmmo < maxAmmo)
        {
            reserveAmmo += amount;
        }
        else if (reserveAmmo + amount > maxAmmo)
        {
            reserveAmmo += maxAmmo - reserveAmmo;
        }
    }

    public enum ShootingType { Projectile, Line, RayProjectile }

    public int CurrentAmmo => currentAmmo;

    public int ReserveAmmo => reserveAmmo;
}