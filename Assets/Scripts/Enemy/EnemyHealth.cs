using RatGamesStudios.OperationDeratization.Enemy.State;
using RatGamesStudios.OperationDeratization.Optimization.ObjectPooling;
using RatGamesStudios.OperationDeratization.Player;
using RatGamesStudios.OperationDeratization.RagdollPhysics;
using RatGamesStudios.OperationDeratization.UI.InGame;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("References")]
        private GameObject player;
        private List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
        private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
        private WeaponIk weaponIk;
        private AiAgent agent;
        private Camera cam;

        [Header("Tracker")]
        [SerializeField] private Tracker tracker;
        private AudioSource markSound;
        private GameObject markText;

        [Header("Dmg popup")]
        [SerializeField] private bool isTutorialActive = false;
        [SerializeField] private GameObject damageTextPrefab;
        private TextMeshPro textToDisplay;
        private GameObject damageTextInstance;

        [Header("Enemy health")]
        [SerializeField] private EnemyStats enemyStats;
        public float currentHealth;
        private float lowHealth = 10f;
        public bool isAlive = true;
        public bool isMarkedAsDead = false;

        [Header("Armor")]
        public float currentArmor = 0;
        public float maxArmor = 100f;
        public Transform armorSocket;

        private void Start()
        {
            agent = GetComponent<AiAgent>();
            weaponIk = GetComponent<WeaponIk>();
            cam = Camera.main;
            player = GameObject.FindGameObjectWithTag("Player");
            markText = player.GetComponent<PlayerUI>().markText.gameObject;
            markSound = player.transform.Find("Sounds/OpponentMarking").GetComponent<AudioSource>();
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
            foreach (Transform child in armorSocket.GetChild(0))
            {
                MeshRenderer mr = child.GetComponent<MeshRenderer>();

                if (mr != null)
                    meshRenderers.Add(mr);
            }
        }
        private void Update()
        {
            if (!isAlive && !isMarkedAsDead && Vector3.Distance(transform.position, player.transform.position) < 3f)
                StartCoroutine(HandleDeathEffects());
        }
        public void TakeDamage(int damage, Vector3 direction, bool isAttackedByPlayer)
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
            else
                armorSocket.GetChild(0).gameObject.SetActive(false);

            currentHealth -= damageToHealth;
            currentHealth = Mathf.Clamp(currentHealth, 0, enemyStats.maxHealth);

            if (currentHealth <= 0)
                Die(direction);
            if (isAttackedByPlayer && isAlive && isTutorialActive)
            {
                float distance = Vector3.Distance(cam.transform.position, transform.position);
                damageTextInstance = ObjectPoolManager.SpawnObject(damageTextPrefab, transform.position + new Vector3(0f, 1f, 0f), cam.transform.rotation, transform);
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
                    Vector3 directionToTarget = (transform.position - cam.transform.position).normalized;
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

                    float cameraAngle = cam.transform.rotation.eulerAngles.x;
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
        private bool IsObjectVisible(GameObject obj)
        {
            Renderer renderer = obj.GetComponent<Renderer>();

            if (renderer == null)
                return false;

            return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), renderer.bounds);
        }
        public bool IsLowHealth()
        {
            return currentHealth < lowHealth;
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
            tracker.MarkOpponentAsDead(gameObject);

            if (!markSound.isPlaying)
            {
                markSound.clip = Resources.Load<AudioClip>("Audio/Tracker/Mark" + Random.Range(1, 9));
                markSound.Play();
            }

            SetShaderParameters(0);
            float elapsedTime = 0f;
            float duration = 5f;

            yield return new WaitForSeconds(2f);

            while (elapsedTime < duration)
            {
                SetShaderParameters(elapsedTime / duration);
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            Destroy(gameObject);
        }
        private IEnumerator FadeOutPromptText()
        {
            float duration = 3f;
            float elapsedTime = 0f;
            markText.SetActive(true);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            markText.SetActive(false);
        }
        private void SetShaderParameters(float disappearIntensity)
        {
            float minDissolve = -1.5f;
            float maxDissolve = 3f;

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in skinnedMeshRenderers)
            {
                Material[] materials = skinnedMeshRenderer.materials;

                foreach (var material in materials)
                {
                    float clampedDisapperIntensity = Mathf.Clamp(disappearIntensity, 0f, 1f);
                    float mappedDissolve = Mathf.Lerp(minDissolve, maxDissolve, clampedDisapperIntensity);
                    material.SetFloat("_dissolveAmount", mappedDissolve);
                }
            }
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                Material[] materials = meshRenderer.materials;

                foreach (var material in materials)
                {
                    float clampedDisapperIntensity = Mathf.Clamp(disappearIntensity, 0f, 1f);
                    float mappedDissolve = Mathf.Lerp(minDissolve, maxDissolve, clampedDisapperIntensity);
                    material.SetFloat("_dissolveAmount", mappedDissolve);
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
}