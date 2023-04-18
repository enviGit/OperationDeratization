using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField]
    private EnemyStats enemyStats;
    [SerializeField]
    private Slider healthBarSlider;
    [SerializeField]
    private Image healthBarSliderImage;
    [SerializeField]
    private Color maxHealthColour;
    [SerializeField]
    private Color noHealthColour;
    private int currentHealth;

    private void Start()
    {
        currentHealth = enemyStats.maxHealth;
        SetHealthBarUI();
    }
    public void DealDamage(int damage)
    {
        currentHealth -= damage;
        CheckIfDead();
        SetHealthBarUI();
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
