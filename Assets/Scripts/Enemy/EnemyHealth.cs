using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyHealth : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyStats enemyStats;
    private GameObject player;
    private List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
    private WeaponIk weaponIk;
    private AiAgent agent;

    [Header("Dmg popup")]
    [SerializeField] private GameObject damageTextPrefab;
    private TextMeshPro textToDisplay;
    private GameObject damageTextInstance;

    [Header("Enemy health")]
    public float currentHealth;
    public bool isAlive = true;
    public bool isMarkedAsDead = false;

    [Header("Armor")]
    public float currentArmor = 0;
    public float maxArmor = 100f;

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
            float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            damageTextInstance = Instantiate(damageTextPrefab, transform.position + new Vector3(0f, 1f, 0f), Camera.main.transform.rotation, transform);
            textToDisplay = damageTextInstance.transform.GetChild(0).GetComponent<TextMeshPro>();
            float minScale = 0.2f;
            float midScale = 1f;
            float maxScale = 5f;
            float t = Mathf.Clamp01((distance - 2f) / (30f - 2f));
            damageTextInstance.transform.localScale = Vector3.Lerp(new Vector3(minScale, minScale, minScale), new Vector3(midScale, midScale, midScale), t);
            damageTextInstance.transform.localScale = Vector3.Lerp(new Vector3(minScale, minScale, minScale), new Vector3(maxScale, maxScale, maxScale), t);
            textToDisplay.text = damage.ToString("0");

            if (!IsObjectVisible(damageTextInstance.transform.GetChild(0).gameObject) && distance < 5f)
            {
                Vector3 directionToTarget = (transform.position - Camera.main.transform.position).normalized;
                float xOffset = 0f;
                float zOffset = 0f;
                float angle = Mathf.Atan2(directionToTarget.x, directionToTarget.z) * Mathf.Rad2Deg;

                if (angle > -45f && angle <= 45f)
                    xOffset = 0.45f;
                else if (angle > 45f && angle <= 135f)
                    zOffset = -0.45f;
                else if (angle > -135f && angle <= -45f)
                    zOffset = 0.45f;
                else
                    xOffset = -0.45f;

                float cameraAngle = Camera.main.transform.rotation.eulerAngles.x;
                float offsetY;

                if (cameraAngle > 0f && cameraAngle <= 180f)
                    offsetY = Mathf.Lerp(-0.7f, -1f, cameraAngle / 180f);
                else
                    offsetY = Mathf.Lerp(-0.7f, -0.5f, (360f - cameraAngle) / 180f);

                damageTextInstance.transform.position += new Vector3(xOffset, offsetY, zOffset);
            }

            damageTextInstance.transform.GetChild(0).GetComponent<Animator>().enabled = true;
        }
    }
    /*public void TakeDamage(float damage)
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
    }*/
    private bool IsObjectVisible(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer == null)
            return false;

        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(Camera.main), renderer.bounds);
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
        foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
        {
            Material[] materials = skinnedMeshRenderer.materials;

            foreach (var material in materials)
            {
                material.SetFloat("_dissolve", disappearIntensity);
                material.SetFloat("_dissolveIntensity", colorIntensity);
            }
        }
    }
    public void RestoreHealth(float healAmount)
    {
        if (!isAlive)
            return;

        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, enemyStats.maxHealth);
    }
    public void PickupArmor()
    {
        if (!isAlive)
            return;
        if (currentArmor > 99f)
            return;

        currentArmor = 100;
    }
}