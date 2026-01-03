using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shooting : MonoBehaviour
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

    [Header("Line Shooting")]
    public float maxDistance = 1;

    [Header("Projectile Shooting")]
    public GameObject projectilePrefab;
    public float projectileSpeed = 1;
    public float projectileLife = 1;
    public Transform projectileSpawnPoint;

    [Header("UI")]
    public TextMeshProUGUI ammoText;
    public Slider reloadBar;

    [Header("Audio")]
    public AudioSource sfxSource;
    public AudioClip shootSFX;
    public AudioClip reloadSFX;

    private float fireTimer = 0, reloadTimer = 0;
    private int currentAmmo = 0, reserveAmmo = 0;
    private bool isShooting = false;
    private bool isReloading = false;
    private Camera mainCamera;
    private Image[] reloadBarImages;

    private void Awake()
    {
        mainCamera = Camera.main;
        reloadBarImages = reloadBar.GetComponentsInChildren<Image>();
    }

    private void Start()
    {
        reloadBar.maxValue = reloadTime;
        reloadBar.value = reloadTime;
        fireTimer = fireRate;
        currentAmmo = startingAmmo;
        if (startingAmmo * startingRounds > maxAmmo)
        {
            reserveAmmo = maxAmmo;
        }
        else
        {
            reserveAmmo = startingAmmo * startingRounds;
        }
        if (maxAmmo == 0)
        {
            maxAmmo = reserveAmmo;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        bool isGamePaused = Time.timeScale == 0;
        canShoot = !isGamePaused;
        fireTimer += Time.deltaTime;
        ammoText.enabled = !isGamePaused;
        ToggleSliderImages(!isGamePaused);

        //AimDownSights();

        isShooting = (isAutomatic && Input.GetMouseButton(0)) || (!isAutomatic && Input.GetMouseButtonDown(0));

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < startingAmmo)
        {
            //StartCoroutine(Reload(reloadTime));
            StartCoroutine(Reload(reloadTime));
        }

        if (isReloading)
        {
            if (reloadTimer < reloadTime)
            {
                reloadTimer += Time.deltaTime;
                reloadBar.value -= Time.deltaTime;
            }
            else
            {
                reloadBar.value = reloadTime;
                reloadTimer = 0;
            }
        }

        ammoText.text = $"{currentAmmo} | {reserveAmmo}";

        if (isShooting)
        {
            if (canShoot && fireTimer >= fireRate && !isReloading)
            {
                switch (shootingType)
                {
                    case ShootingType.Projectile:
                        ShootProjectile();
                        break;

                    case ShootingType.Line:
                        ShootLine();
                        break;

                    default:
                        break;
                }
            }
        }
    }

    private void ToggleSliderImages(bool toggle)
    {
        foreach (Image image in reloadBarImages)
        {
            image.enabled = toggle;
        }
    }

    private void ShootProjectile()
    {
        sfxSource.clip = shootSFX;
        if (currentAmmo > 0)
        {
            GameObject projectileClone = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            projectileClone.GetComponent<Rigidbody>().linearVelocity = 100 * projectileSpeed * Time.deltaTime * mainCamera.transform.forward;
            currentAmmo--;
            fireTimer = 0;
            sfxSource.Play();
            Destroy(projectileClone, projectileLife);
        }
    }

    private void ShootLine()
    {
        sfxSource.clip = shootSFX;
        if (currentAmmo > 0)
        {
            //GameObject impact;
            Ray shootDirection = new(mainCamera.transform.position, mainCamera.transform.forward);

            if (Physics.Raycast(shootDirection, out RaycastHit hit, maxDistance))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    hit.collider.GetComponent<Enemy>().TakeDamage(damage);
                    //impact = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal), hit.transform);
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

            sfxSource.Play();
            //muzzleFlash.Play();
            currentAmmo--;
            fireTimer = 0;
        }
    }

    
    public IEnumerator Reload(float duration)
    {
        sfxSource.clip = reloadSFX;
        bool hasReloaded = ((!infiniteAmmo && reserveAmmo > 0) || infiniteAmmo) && currentAmmo < startingAmmo;
        if (hasReloaded)
        {
            isReloading = true;
        }

        yield return new WaitForSeconds(duration);


        if (hasReloaded)
        {
            sfxSource.Play();
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
                int reloadAmount = startingAmmo - currentAmmo;
                reloadAmount = (reserveAmmo - reloadAmount) < 0 ? reserveAmmo : reloadAmount;
                currentAmmo += reloadAmount;
                reserveAmmo -= reloadAmount;
            }
        }

        isReloading = false;
    }

    public enum ShootingType { Projectile, Line }
}