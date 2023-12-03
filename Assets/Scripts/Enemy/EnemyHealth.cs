using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyStats enemyStats;
    private GameObject player;
    List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    WeaponIk weaponIk;
    AiAgent agent;

    [Header("Dmg popup")]
    [SerializeField] private GameObject damageTextPrefab;
    private string textToDisplay;
    private GameObject damageTextInstance;

    [Header("Enemy health")]
    private int currentHealth;
    public bool isAlive = true;
    public bool isMarkedAsDead = false;

    private void Start()
    {
        agent = GetComponent<AiAgent>();
        weaponIk = GetComponent<WeaponIk>();
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = enemyStats.maxHealth;
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
        if (!isAlive && !isMarkedAsDead && Vector3.Distance(transform.position, player.transform.position) < 2f)
            StartCoroutine(HandleDeathEffects());
    }
    public void TakeDamage(int damage, Vector3 direction, bool isAttackedByPlayer)
    {
        currentHealth -= damage;

        if (currentHealth <= 0)
            Die(direction);
        if (isAttackedByPlayer && isAlive)
        {
            damageTextInstance = Instantiate(damageTextPrefab, transform.position + new Vector3(0f, 1.5f, 0f), Camera.main.transform.rotation, transform);
            textToDisplay = damage.ToString("0");
            damageTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>().text = textToDisplay;
        }
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
}