using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Image healthBarSliderImage;
    List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    AiAgent agent;
    
    [Header("Enemy health bar")]
    [SerializeField] private Color maxHealthColour;
    [SerializeField] private Color noHealthColour;
    private int currentHealth;
    private float lastDamageTime;
    public bool showHealthBar;

    [Header("Enemy health")]
    public float blinkIntensity = 10f;
    public float blinkDuration = 0.05f;
    float blinkTimer;

    private void Start()
    {
        agent = GetComponent<AiAgent>();
        currentHealth = enemyStats.maxHealth;
        SetHealthBarUI();
        lastDamageTime = Time.time;
        showHealthBar = false;
        var rigidBodies = GetComponentsInChildren<Rigidbody>();

        foreach (var rigidBody in rigidBodies)
        {
            HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
            hitBox.health = this;
        }
        foreach (Transform child in transform)
        {
            SkinnedMeshRenderer smr = child.GetComponent<SkinnedMeshRenderer>();

            if (smr != null)
                skinnedMeshRenderers.Add(smr);
        }
    }
    private void Update()
    {
        if (Time.time - lastDamageTime > 3f)
            showHealthBar = false;

        healthBarSlider.gameObject.SetActive(showHealthBar);
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = (lerp * blinkIntensity) + 1f;

        foreach (var item in skinnedMeshRenderers)
            item.material.color = Color.white * intensity;
    }
    public void DealDamage(int damage, Vector3 direction)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die(direction);

        blinkTimer = blinkDuration;
        SetHealthBarUI();
        lastDamageTime = Time.time;
        showHealthBar = true;
    }
    private void Die(Vector3 direction)
    {
        AiDeathState deathState = agent.stateMachine.GetState(AiStateId.Death) as AiDeathState;
        deathState.direction = direction;
        agent.stateMachine.ChangeState(AiStateId.Death);
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
