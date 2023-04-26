using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Image healthBarSliderImage;

    [Header("Health bar colours")]
    [SerializeField] private Color maxHealthColour;
    [SerializeField] private Color noHealthColour;

    [Header("Enemy health bar")]
    private int currentHealth;
    private float lastDamageTime;
    private bool showHealthBar;

    private void Start()
    {
        currentHealth = enemyStats.maxHealth;
        SetHealthBarUI();
        lastDamageTime = Time.time;
        showHealthBar = false;
    }
    private void Update()
    {
        if (Time.time - lastDamageTime > 3f)
            showHealthBar = false;

        healthBarSlider.gameObject.SetActive(showHealthBar);
    }
    public void DealDamage(int damage)
    {
        currentHealth -= damage;
        CheckIfDead();
        SetHealthBarUI();
        lastDamageTime = Time.time;
        showHealthBar = true;
    }
    private void CheckIfDead()
    {
        if(currentHealth <= 0)
            Destroy(gameObject);
    }
    private void SetHealthBarUI()
    {
        float healthPercentage = CalculateHealthPercentage();
        healthBarSlider.value = healthPercentage;
        healthBarSliderImage.color = Color.Lerp(noHealthColour, maxHealthColour, healthPercentage / 100);
    }
    private float CalculateHealthPercentage()
    {
        return ((float)currentHealth / (float)enemyStats.maxHealth) * 100;
    }
}
