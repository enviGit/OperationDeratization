using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    public bool isAlive = true;
    public bool isMarkedAsDead = false;

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

        if (!isAlive && !isMarkedAsDead && Vector3.Distance(transform.position, player.transform.position) < 2f)
            StartCoroutine(HandleDeathEffects());
    }
    public void DealDamage(int damage, Vector3 direction)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die(direction);

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
        StartCoroutine(FadeOutPromptText());
        Tracker tracker = FindObjectOfType<Tracker>();
        tracker.MarkOpponentAsDead(gameObject);
        SetShaderParameters(0, 0);
        float elapsedTime = 0f;
        float duration = 5f;

        while (elapsedTime < duration)
        {
            SetShaderParameters(elapsedTime / duration, elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
    private IEnumerator FadeOutPromptText()
    {
        float fadeDuration = 2f;
        float elapsedTime = 0f;
        player.GetComponent<PlayerUI>().markText.text = "Opponent marked as dead";

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        player.GetComponent<PlayerUI>().markText.text = "";
    }

    private void SetShaderParameters(float disappearIntensity, float colorIntensity)
    {
        foreach (var skinnedMeshRenderer in skinnedMeshRenderers)
        {
            Material[] materials = skinnedMeshRenderer.materials;

            foreach (var material in materials)
            {
                material.SetFloat("_dissolve", disappearIntensity);
                material.SetFloat("_dissolveIntensity", colorIntensity);
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
