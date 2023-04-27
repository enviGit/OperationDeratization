using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health bar")]
    private float currentHealth;
    private float lerpTimer;
    public float maxHealth = 100;
    public float chipSpeed = 2f;

    [Header("Health bar images")]
    public Image frontHealthBar;
    public Image backHealthBar;

    [Header("Armor bar")]
    private float currentArmor;
    public float maxArmor = 100;

    [Header("Armor bar images")]
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
    }
    public void TakeDamage(float damage)
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
}
