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

    [Header("Health")]
    private float currentHealth;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    private bool isAlive = true;

    [Header("Armor")]
    public float currentArmor = 0;
    public float maxArmor = 100f;

    private void Start()
    {
        currentHealth = maxHealth;
        ragdoll = GetComponent<Ragdoll>();
        var rigidBodies = GetComponentsInChildren<Rigidbody>();

        foreach (var rigidBody in rigidBodies)
        {
            HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
            hitBox.playerHealth = this;
        }
    }
    private void Update()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        currentArmor = Mathf.Clamp(currentArmor, 0, maxArmor);
        UpdateHealthUI();
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

        if (currentHealth <= 0)
            Die();
    }
    public void TakeFallingDamage(float damage)
    {
        if (!isAlive)
            return;

        currentHealth -= damage;
        lerpTimer = 0f;

        if (currentHealth <= 0)
            Die();
    }
    private void Die()
    {
        isAlive = false;
        deathCamera.gameObject.SetActive(true);
        Vector3 playerPosition = transform.position;
        Ray cameraRay = new Ray(playerPosition, Vector3.back);
        RaycastHit hit;

        if (Physics.Raycast(cameraRay, out hit))
            deathCamera.transform.position = hit.point;
        else
            deathCamera.transform.position = playerPosition - transform.forward * 5f + Vector3.up * 2f;

        deathCamera.transform.LookAt(playerPosition);
        CharacterController controller = GetComponent<CharacterController>();
        controller.enabled = false;
        PlayerMotor playerMotor = GetComponent<PlayerMotor>();
        playerMotor.enabled = false;
        PlayerShoot shoot = GetComponent<PlayerShoot>();
        shoot.enabled = false;
        PlayerInteract interact = GetComponent<PlayerInteract>();
        interact.enabled = false;
        PlayerInventory inventory = GetComponent<PlayerInventory>();

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
                child.gameObject.SetActive(false);
                continue;
            }

            Destroy(child.gameObject);
        }

        Transform mesh = transform.Find("Mesh");

        foreach (Transform child in mesh)
            child.gameObject.SetActive(true);

        ragdoll.ActivateRagdoll();
        inventoryUI.gameObject.SetActive(false);
    }
    public void RestoreHealth(float healAmount)
    {
        if (!isAlive)
            return;

        currentHealth += healAmount;
        lerpTimer = 0f;
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
}
