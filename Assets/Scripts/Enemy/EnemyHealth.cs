using RatGamesStudios.OperationDeratization.Enemy.State;
using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.RagdollPhysics;
using RatGamesStudios.OperationDeratization.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class EnemyHealth : MonoBehaviour
    {
        [Header("References")]
        private Transform playerTransform;
        private Camera cam;
        private AiAgent agent;
        private WeaponIk weaponIk;
        private AudioEventManager audioEventManager;
        private KillFeedbackUI killFeedbackUI;

        [Header("Visuals")]
        private List<SkinnedMeshRenderer> skinnedMeshRenderers = new List<SkinnedMeshRenderer>();
        private List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
        public Transform armorSocket;

        [Header("Stats")]
        [SerializeField] private EnemyStats enemyStats;
        public float currentHealth;
        [SerializeField] private float lowHealthThreshold = 15f;
        public float currentArmor = 0;
        public float maxArmor = 100f;
        public bool isAlive = true;
        public bool isMarkedAsDead = false;

        [Header("Audio")]
        [SerializeField] private AudioSource impactSound;
        [SerializeField] private AudioClip[] impactClips;
        private AudioSource markSound;
        private AudioClip[] markClips;
        [Header("Settings")]
        [SerializeField] private Tracker tracker;

        private void Start()
        {
            agent = GetComponent<AiAgent>();
            weaponIk = GetComponent<WeaponIk>();
            cam = Camera.main;

            var playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj)
            {
                playerTransform = playerObj.transform;
                markSound = playerTransform.Find("Sounds/OpponentMarking")?.GetComponent<AudioSource>();
            }

            killFeedbackUI = FindFirstObjectByType<KillFeedbackUI>();

            var audioMgr = GameObject.FindGameObjectWithTag("AudioEventManager");
            if (audioMgr) audioEventManager = audioMgr.GetComponent<AudioEventManager>();

            if (impactSound == null) impactSound = transform.Find("Sounds/Impact")?.GetComponent<AudioSource>();

            currentHealth = enemyStats.maxHealth;

            SetupVisualsAndHitboxes();

            LoadMarkSounds();
        }
        private void SetupVisualsAndHitboxes()
        {
            foreach (var rb in GetComponentsInChildren<Rigidbody>())
            {
                HitBox hitBox = rb.gameObject.AddComponent<HitBox>();
                hitBox.health = this;
                if (hitBox.gameObject != gameObject)
                    hitBox.gameObject.layer = LayerMask.NameToLayer("Hitbox");
            }

            GetComponentsInChildren(true, skinnedMeshRenderers);

            if (armorSocket && armorSocket.childCount > 0)
            {
                var armorRenderers = armorSocket.GetChild(0).GetComponentsInChildren<MeshRenderer>();
                meshRenderers.AddRange(armorRenderers);
            }
        }

        private void LoadMarkSounds()
        {
            markClips = Resources.LoadAll<AudioClip>("Audio/Tracker");
        }
        private void Update()
        {
            if (!isAlive && !isMarkedAsDead && playerTransform != null)
            {
                if (Vector3.Distance(transform.position, playerTransform.position) < 3f)
                {
                    StartCoroutine(HandleDeathConfirmation());
                }
            }
        }
        public void TakeDamage(int damage, Vector3 direction, bool isAttackedByPlayer)
        {
            if (!isAlive) return;

            ApplyDamage(damage);
            PlayImpactSound();

            if (currentHealth <= 0) Die(direction);
        }
        private void PlayImpactSound()
        {
            if (impactClips.Length > 0 && impactSound)
            {
                impactSound.pitch = Random.Range(0.85f, 1.15f);
                impactSound.PlayOneShot(impactClips[Random.Range(0, impactClips.Length)]);
                audioEventManager?.NotifyAudioEvent(impactSound);
            }
        }
        private void ApplyDamage(float damage)
        {
            float damageToHealth = damage;

            if (currentArmor > 0)
            {
                damageToHealth = damage * 0.5f;
                currentArmor = Mathf.Clamp(currentArmor - damage, 0, maxArmor);
            }
            else if (armorSocket && armorSocket.childCount > 0)
            {
                armorSocket.GetChild(0).gameObject.SetActive(false);
            }

            currentHealth = Mathf.Clamp(currentHealth - damageToHealth, 0, enemyStats.maxHealth);
        }
        public bool IsLowHealth()
        {
            return currentHealth <= lowHealthThreshold;
        }
        private void Die(Vector3 direction)
        {
            if (!isAlive) return;

            isAlive = false;
            if (weaponIk) weaponIk.enabled = false;

            if (agent && agent.stateMachine != null)
            {
                var deathState = agent.stateMachine.GetState(AiStateId.Death) as AiDeathState;
                if (deathState != null)
                {
                    deathState.direction = direction;
                    agent.stateMachine.ChangeState(AiStateId.Death);
                }
            }
        }
        private IEnumerator HandleDeathConfirmation()
        {
            isMarkedAsDead = true;

            if (tracker) tracker.MarkOpponentAsDead(gameObject);

            if (killFeedbackUI) killFeedbackUI.ShowNeutralizedMessage();

            if (markSound && markClips != null && markClips.Length > 0)
            {
                markSound.clip = markClips[Random.Range(0, markClips.Length)];
                markSound.Play();
            }

            float duration = 5f;
            yield return new WaitForSeconds(2f);

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                SetShaderDissolve(t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            Destroy(gameObject);
        }
        private void SetShaderDissolve(float intensity)
        {
            float minDissolve = -1.5f;
            float maxDissolve = 3f;
            float mappedDissolve = Mathf.Lerp(minDissolve, maxDissolve, Mathf.Clamp01(intensity));

            foreach (var smr in skinnedMeshRenderers)
                foreach (var mat in smr.materials)
                    mat.SetFloat("_dissolveAmount", mappedDissolve);

            foreach (var mr in meshRenderers)
                foreach (var mat in mr.materials)
                    mat.SetFloat("_dissolveAmount", mappedDissolve);
        }
        public void RestoreHealth(float healAmount)
        {
            if (!isAlive) return;
            currentHealth = Mathf.Min(currentHealth + healAmount, enemyStats.maxHealth);
        }

        public void PickupArmor()
        {
            if (!isAlive || currentArmor > 99f) return;
            currentArmor = 100;

            if (armorSocket && armorSocket.childCount > 0) armorSocket.GetChild(0).gameObject.SetActive(true);
        }
    }
}