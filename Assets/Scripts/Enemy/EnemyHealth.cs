using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyStats enemyStats;
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Image healthBarSliderImage;
    private GameObject player;
    List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    WeaponIk weaponIk;
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
    public bool isAlive = true;
    public bool isMarkedAsDead = false;
    float blinkTimer;

    private void Start()
    {
        agent = GetComponent<AiAgent>();
        weaponIk = GetComponent<WeaponIk>();
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = enemyStats.maxHealth;
        SetHealthBarUI();
        lastDamageTime = Time.time;
        showHealthBar = false;
        var rigidBodies = GetComponentsInChildren<Rigidbody>();

        foreach (var rigidBody in rigidBodies)
        {
            HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
            hitBox.health = this;

            if (hitBox.gameObject != gameObject)
                hitBox.gameObject.layer = LayerMask.NameToLayer("Hitbox");
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
        if (Time.time - lastDamageTime > 3f || currentHealth <= 0)
            showHealthBar = false;

        healthBarSlider.gameObject.SetActive(showHealthBar);
        blinkTimer -= Time.deltaTime;
        float lerp = Mathf.Clamp01(blinkTimer / blinkDuration);
        float intensity = (lerp * blinkIntensity) + 1f;

        foreach (var item in skinnedMeshRenderers)
            item.material.color = Color.white * intensity;

        if (!isAlive && !isMarkedAsDead && Vector3.Distance(transform.position, player.transform.position) < 2f)
            StartCoroutine(HandleDeathEffects());
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
        if (isAlive)
        {
            isAlive = false;
            weaponIk.enabled = false;
            AiDeathState deathState = agent.stateMachine.GetState(AiStateId.Death) as AiDeathState;
            deathState.direction = direction;
            agent.stateMachine.ChangeState(AiStateId.Death);
        }
    }
    private IEnumerator HandleDeathEffects()
    {
        isMarkedAsDead = true;
        Tracker tracker = FindObjectOfType<Tracker>();
        tracker.MarkOpponentAsDead(gameObject);
        SetShaderParameters(0, new Color(1, 0, 0, 0), 0);
        float elapsedTime = 0f;
        float duration = 5f;

        while (elapsedTime < duration)
        {
            Color currentColor = Color.Lerp(new Color(1, 0, 0, 0), Color.gray, elapsedTime / (duration / 4f));
            SetShaderParameters(elapsedTime / duration, currentColor, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }
        
        Destroy(gameObject);
    }

    private void SetShaderParameters(float disappearIntensity, Color color, float lightIntensity)
    {
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            Material[] materials = skinnedMeshRenderer.materials;

            foreach (var material in materials)
            {
                material.SetFloat("_disappearIntensity", disappearIntensity);
                material.SetColor("_lightColor", color);
                material.SetFloat("_lightIntensity", lightIntensity);
            }
        }
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
