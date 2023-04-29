using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health bar")]
    private float currentHealth;
    private float lerpTimer;
    public float maxHealth = 100f;
    public float chipSpeed = 2f;
    public Image frontHealthBar;
    public Image backHealthBar;

    [Header("Armor")]
    public float currentArmor = 0;
    public float maxArmor = 100f;
    private float armorLerpTimer;
public float armorChipSpeed = 2f;
    public Transform armorBar;
    public Image frontArmorBar;
    public Image backArmorBar;

    [Header("Player state")]
    private bool isAlive = true;

    private void Start()
    {
        currentHealth = maxHealth;
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
            float armorMultiplier = 0.1f;
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
