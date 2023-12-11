using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("References")]
    public Image frontHealthBar;
    public Image backHealthBar;
    public Transform armorBar;
    public Image frontArmorBar;
    public Image backArmorBar;
    public Ragdoll ragdoll;
    public Transform inventoryUI;
    public Camera deathCamera;
    public Material vignetteMaterial;
    private AudioSource heartbeatSound;
    private AudioSource deathSound;
    private AudioSource impactSound;
    public AudioClip[] impactClips;

    [Header("Health")]
    public float currentHealth;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public bool isAlive = true;

    [Header("Armor")]
    public float currentArmor = 0;
    public float maxArmor = 100f;

    [Header("Heartbeat")]
    private float heartbeatMultiplier = 1.2f;
    private float initialMultiplier = 0.05f;

    private void Start()
    {
        currentHealth = maxHealth;
        heartbeatSound = transform.Find("Sounds/Heartbeat").GetComponent<AudioSource>();
        deathSound = transform.Find("Sounds/Death").GetComponent<AudioSource>();
        impactSound = transform.Find("Sounds/Impact").GetComponent<AudioSource>();
        ragdoll = GetComponent<Ragdoll>();
        var rigidBodies = GetComponentsInChildren<Rigidbody>();

        foreach (var rigidBody in rigidBodies)
        {
            HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
            hitBox.playerHealth = this;

            if (hitBox.gameObject != gameObject)
                hitBox.gameObject.layer = LayerMask.NameToLayer("Hitbox");
        }

        vignetteMaterial.SetFloat("_VoronoiIntensity", 0);
        vignetteMaterial.SetFloat("_VignetteRadiusPower", 10);
    }
    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentArmor = Mathf.Clamp(currentArmor, 0, maxArmor);
        UpdateHealthUI();
        float healthPercentage = currentHealth / maxHealth;

        if (healthPercentage <= 0.3f && isAlive == true)
        {
            if (!heartbeatSound.isPlaying)
                heartbeatSound.Play();

            float pitchJumpMultiplier = Mathf.Lerp(initialMultiplier, 1f, 1f - healthPercentage);
            float volumeMultiplier = Mathf.Lerp(initialMultiplier, 1f, 1f - healthPercentage);
            heartbeatSound.volume = Mathf.Lerp(0.5f, 0.75f, 1f - healthPercentage) * heartbeatMultiplier * volumeMultiplier;
            heartbeatSound.pitch = Mathf.Lerp(1f, 1.5f, 1f - healthPercentage) * heartbeatMultiplier * pitchJumpMultiplier;
        }
        else
            heartbeatSound.Stop();
    }
    public void UpdateHealthUI()
    {
        float fillF = frontHealthBar.fillAmount;
        float fillB = backHealthBar.fillAmount;
        float hFraction = currentHealth / maxHealth;

        if (fillB > hFraction)
        {
            frontHealthBar.fillAmount = hFraction;
            backHealthBar.color = Color.red;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillB, hFraction, percentComplete);
        }
        if (fillF < hFraction)
        {
            backHealthBar.color = Color.green;
            backHealthBar.fillAmount = hFraction;
            lerpTimer += Time.deltaTime;
            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
        }
        if (!isAlive)
        {
            armorBar.gameObject.SetActive(false);
            backArmorBar.fillAmount = 0;
            return;
        }

        float fillAF = frontArmorBar.fillAmount;
        float fillAB = backArmorBar.fillAmount;
        float aFraction = currentArmor / maxArmor;

        if (currentArmor == 0)
        {
            armorBar.gameObject.SetActive(false);
            backArmorBar.fillAmount = 0;
        }
        else
        {
            armorBar.gameObject.SetActive(true);
            backArmorBar.fillAmount = aFraction;

            if (fillAB > aFraction)
            {
                frontArmorBar.fillAmount = aFraction;
                backArmorBar.color = Color.gray;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                backArmorBar.fillAmount = Mathf.Lerp(fillAB, aFraction, percentComplete);
            }
            if (fillAF < aFraction)
            {
                backArmorBar.color = Color.blue;
                backArmorBar.fillAmount = aFraction;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                frontArmorBar.fillAmount = Mathf.Lerp(fillAF, backArmorBar.fillAmount, percentComplete);
            }
        }
    }
    public void TakeDamage(float damage)
    {
        if (!isAlive)
            return;

        float damageToHealth = damage;

        if (currentArmor > 0)
        {
            float armorMultiplier = 0.5f;
            damageToHealth = damage * armorMultiplier;
            currentArmor -= damage;
            currentArmor = Mathf.Clamp(currentArmor, 0, maxArmor);
        }

        currentHealth -= damageToHealth;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        lerpTimer = 0f;

        if (vignetteMaterial != null)
        {
            float percent = currentHealth / maxHealth;
            float voronoiIntensity = Mathf.Lerp(0f, 0.8f, 1 - percent);
            float vignetteRadiusPower = Mathf.Lerp(10f, 7f, 1 - percent);
            vignetteMaterial.SetFloat("_VoronoiIntensity", voronoiIntensity);
            vignetteMaterial.SetFloat("_VignetteRadiusPower", vignetteRadiusPower);
        }
        if (impactClips.Length > 0)
        {
            int randomIndex = Random.Range(0, impactClips.Length - 1);
            impactSound.PlayOneShot(impactClips[randomIndex]);
        }
        if (currentHealth <= 0)
            Die();
    }
    public void TakeFallingDamage(float damage)
    {
        if (!isAlive)
            return;

        currentHealth -= damage;
        lerpTimer = 0f;

        if (vignetteMaterial != null)
        {
            float percent = currentHealth / maxHealth;
            float voronoiIntensity = Mathf.Lerp(0f, 0.8f, 1 - percent);
            float vignetteRadiusPower = Mathf.Lerp(10f, 7f, 1 - percent);
            vignetteMaterial.SetFloat("_VoronoiIntensity", voronoiIntensity);
            vignetteMaterial.SetFloat("_VignetteRadiusPower", vignetteRadiusPower);
        }
        if (impactClips.Length > 0)
            impactSound.PlayOneShot(impactClips[2], 0.5f);
        if (currentHealth <= 0)
            Die();
    }
    public void TakeGasDamage(float damage)
    {
        if (!isAlive)
            return;

        currentHealth -= damage;
        lerpTimer = 0f;

        if (vignetteMaterial != null)
        {
            float percent = currentHealth / maxHealth;
            float voronoiIntensity = Mathf.Lerp(0f, 0.8f, 1 - percent);
            float vignetteRadiusPower = Mathf.Lerp(10f, 7f, 1 - percent);
            vignetteMaterial.SetFloat("_VoronoiIntensity", voronoiIntensity);
            vignetteMaterial.SetFloat("_VignetteRadiusPower", vignetteRadiusPower);
        }
        if (impactClips.Length > 0)
        {
            int randomIndex = Random.Range(0, impactClips.Length - 1);
            impactSound.PlayOneShot(impactClips[randomIndex]);

            if (currentHealth <= 3)
                impactSound.PlayOneShot(impactClips[3], 0.5f);
        }
        if (currentHealth <= 0)
            Die();
    }
    private void Die()
    {
        isAlive = false;
        deathCamera.gameObject.SetActive(true);
        deathCamera.transform.SetParent(null);
        CharacterController controller = GetComponent<CharacterController>();
        controller.enabled = false;
        PlayerMotor playerMotor = GetComponent<PlayerMotor>();
        playerMotor.enabled = false;
        PlayerShoot shoot = GetComponent<PlayerShoot>();
        shoot.enabled = false;
        PlayerInteract interact = GetComponent<PlayerInteract>();
        interact.enabled = false;
        PlayerInventory inventory = GetComponent<PlayerInventory>();
        PlayerStamina stamina = GetComponent<PlayerStamina>();
        stamina.staminaBarFill.transform.parent.gameObject.SetActive(false);
        stamina.enabled = false;

        foreach (Gun weapon in inventory.weapons)
        {
            if (weapon != null && weapon.gunStyle != GunStyle.Melee)
            {
                GameObject newWeapon = Instantiate(weapon.gunPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
                Rigidbody rb;

                if (newWeapon.GetComponent<Rigidbody>() != null)
                    rb = newWeapon.GetComponent<Rigidbody>();
                else
                    rb = newWeapon.AddComponent<Rigidbody>();

                rb.mass = 2f;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            }
        }

        for (int i = 1; i < inventory.weapons.Length; i++)
            inventory.weapons[i] = null;

        inventory.currentWeaponIndex = 0;
        Transform weaponHolder = transform.Find("Camera/Main Camera/WeaponHolder");

        foreach (Transform child in weaponHolder)
        {
            if (child.gameObject.name == "Knife_00(Clone)")
            {
                Destroy(child.gameObject);
                continue;
            }
        }

        Transform mesh = transform.Find("Mesh");

        foreach (Transform child in mesh)
            child.gameObject.SetActive(true);

        ragdoll.ActivateRagdoll();
        deathSound.Play();
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
    }
    public void RestoreHealth(float healAmount)
    {
        if (!isAlive)
            return;

        currentHealth += healAmount;
        lerpTimer = 0f;

        if (vignetteMaterial != null)
        {
            float percent = currentHealth / maxHealth;
            float voronoiIntensity = Mathf.Lerp(0f, 0.8f, 1 - percent);
            float vignetteRadiusPower = Mathf.Lerp(10f, 7f, 1 - percent);
            vignetteMaterial.SetFloat("_VoronoiIntensity", voronoiIntensity);
            vignetteMaterial.SetFloat("_VignetteRadiusPower", vignetteRadiusPower);
        }
    }
    public void PickupArmor()
    {
        if (!isAlive)
            return;
        if (currentArmor > 99f)
            return;

        currentArmor = 100;
        UpdateHealthUI();
    }
    private void OnDisable()
    {
        if (vignetteMaterial != null)
        {
            vignetteMaterial.SetFloat("_VoronoiIntensity", 0);
            vignetteMaterial.SetFloat("_VignetteRadiusPower", 10f);
        }
    }
}