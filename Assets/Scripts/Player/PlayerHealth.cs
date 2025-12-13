using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.RagdollPhysics;
using RatGamesStudios.OperationDeratization.UI.InGame;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private ResourceBar healthBarUI;
        [SerializeField] private ResourceBar armorBarUI;

        [Header("Visuals & Audio References")]
        [SerializeField] private GameObject miniMapCanvas;
        [SerializeField] private Camera miniMapCamera;
        [SerializeField] private Material vignetteMaterial;

        [SerializeField] private AudioSource heartbeatSound;
        [SerializeField] private AudioSource impactSound;
        [SerializeField] private AudioSource deathSounds;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip[] impactClips;
        [SerializeField] private AudioClip[] gasClips;
        [SerializeField] private AudioClip[] deathClips;

        [Header("Stats")]
        public float currentHealth;
        public float maxHealth = 100f;
        public float currentArmor = 0;
        public float maxArmor = 100f;
        public bool isAlive = true;

        private PlayerInventory inventory;
        private AudioEventManager audioEventManager;
        private Transform cam;

        private float heartbeatMultiplier = 1.2f;
        private float initialMultiplier = 0.05f;

        private void Start()
        {
            if (heartbeatSound == null) heartbeatSound = transform.Find("Sounds/Heartbeat")?.GetComponent<AudioSource>();
            if (impactSound == null) impactSound = transform.Find("Sounds/Impact")?.GetComponent<AudioSource>();
            if (deathSounds == null) deathSounds = transform.Find("Sounds/Death")?.GetComponent<AudioSource>();

            inventory = GetComponent<PlayerInventory>();
            cam = Camera.main.transform;

            var audioManagerObj = GameObject.FindGameObjectWithTag("AudioEventManager");
            if (audioManagerObj) audioEventManager = audioManagerObj.GetComponent<AudioEventManager>();

            SetupHitboxes();
            ResetVignette();
        }

        private void SetupHitboxes()
        {
            var rigidBodies = GetComponentsInChildren<Rigidbody>();
            foreach (var rigidBody in rigidBodies)
            {
                HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
                hitBox.playerHealth = this;
                if (hitBox.gameObject != gameObject)
                    hitBox.gameObject.layer = LayerMask.NameToLayer("Hitbox");
            }
        }

        private void Update()
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            currentArmor = Mathf.Clamp(currentArmor, 0, maxArmor);

            if (healthBarUI != null) healthBarUI.UpdateBar(currentHealth, maxHealth);

            if (isAlive)
            {
                if (armorBarUI != null) armorBarUI.UpdateBar(currentArmor, maxArmor);
            }
            else
            {
                if (armorBarUI != null) armorBarUI.UpdateBar(0, maxArmor);
            }

            HandleHeartbeat();
        }

        private void HandleHeartbeat()
        {
            if (!isAlive) { heartbeatSound.Stop(); return; }

            float healthPercentage = currentHealth / maxHealth;

            if (healthPercentage <= 0.3f)
            {
                if (!heartbeatSound.isPlaying) heartbeatSound.Play();

                float t = 1f - healthPercentage;
                float pitchJump = Mathf.Lerp(initialMultiplier, 1f, t);
                float volMult = Mathf.Lerp(initialMultiplier, 1f, t);

                heartbeatSound.volume = Mathf.Lerp(0.5f, 0.75f, t) * heartbeatMultiplier * volMult;
                heartbeatSound.pitch = Mathf.Lerp(1f, 1.5f, t) * heartbeatMultiplier * pitchJump;
            }
            else
            {
                heartbeatSound.Stop();
            }
        }

        public void TakeDamage(float damage)
        {
            if (!isAlive) return;

            if (healthBarUI) healthBarUI.SetBackColor(Color.red);
            if (armorBarUI) armorBarUI.SetBackColor(Color.gray);

            float damageToHealth = damage;

            if (currentArmor > 0)
            {
                damageToHealth = damage * 0.5f;
                currentArmor = Mathf.Clamp(currentArmor - damage, 0, maxArmor);
            }

            currentHealth = Mathf.Clamp(currentHealth - damageToHealth, 0, maxHealth);
            UpdateVignette();

            PlayImpactSound(impactClips);

            if (currentHealth <= 0) Die();
        }

        private void PlayImpactSound(AudioClip[] clips)
        {
            if (clips != null && clips.Length > 0)
            {
                int randomIndex = Random.Range(0, clips.Length);
                impactSound.pitch = Random.Range(0.85f, 1.15f);
                impactSound.PlayOneShot(clips[randomIndex], 1.0f);
                audioEventManager?.NotifyAudioEvent(impactSound);
            }
        }

        public void TakeFallingDamage(float damage)
        {
            if (!isAlive) return;
            if (healthBarUI) healthBarUI.SetBackColor(Color.red);

            currentHealth -= damage;
            UpdateVignette();

            if (impactClips.Length > 2)
            {
                impactSound.pitch = Random.Range(0.85f, 1.15f);
                impactSound.PlayOneShot(impactClips[2], 0.5f);
                audioEventManager?.NotifyAudioEvent(impactSound);
            }

            if (currentHealth <= 0) Die();
        }

        public void TakeGasDamage(float damage)
        {
            if (!isAlive) return;
            if (healthBarUI) healthBarUI.SetBackColor(Color.red);

            currentHealth -= damage;
            UpdateVignette();
            PlayImpactSound(gasClips);

            if (currentHealth <= 0) Die();
        }

        public void TakeFireDamage(float damage) => TakeDamage(damage);

        private void Die()
        {
            isAlive = false;
            miniMapCanvas.SetActive(false);
            miniMapCamera.targetTexture = null;

            if (deathClips.Length > 0)
            {
                deathSounds.clip = deathClips[0];
                deathSounds.Play();
                audioEventManager?.NotifyAudioEvent(deathSounds);

                if (deathClips.Length > 1) Invoke(nameof(PlaySecondDeathClip), deathClips[0].length);
            }

            DropWeaponsOnDeath();
            DisablePlayerComponents();
        }

        private void DropWeaponsOnDeath()
        {
            foreach (Gun weapon in inventory.weapons)
            {
                if (weapon != null && weapon.gunStyle != GunStyle.Melee)
                {
                    GameObject newWeapon = Instantiate(weapon.gunPrefab, transform.position + Vector3.up, Quaternion.identity);
                    if (!newWeapon.TryGetComponent(out Rigidbody rb)) rb = newWeapon.AddComponent<Rigidbody>();

                    rb.mass = 2f;
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                }
            }
        }

        private void DisablePlayerComponents()
        {
            foreach (Transform child in cam) child.gameObject.SetActive(false);

            GetComponent<CharacterController>().enabled = false;
            GetComponent<PlayerMotor>().enabled = false;
            GetComponent<PlayerInteract>().enabled = false;
            GetComponent<PlayerInventory>().enabled = false;
            GetComponent<PlayerShoot>().enabled = false;
        }

        private void PlaySecondDeathClip()
        {
            if (deathClips.Length > 1)
            {
                deathSounds.clip = deathClips[1];
                deathSounds.Play();
            }
        }

        public void RestoreHealth(float healAmount)
        {
            if (!isAlive) return;

            if (healthBarUI) healthBarUI.SetBackColor(new Color(0.25f, 0.5f, 0f, 1f));

            currentHealth += healAmount;
            currentHealth = Mathf.Min(currentHealth, maxHealth);
            UpdateVignette();
        }

        public void PickupArmor()
        {
            if (!isAlive || currentArmor > 99f) return;

            if (armorBarUI) armorBarUI.SetBackColor(new Color(0f, 0.44f, 0.78f, 1f));

            currentArmor = 100;
        }

        private void UpdateVignette()
        {
            if (vignetteMaterial == null) return;

            if (currentHealth < 50)
            {
                float percent = currentHealth / maxHealth;
                float invPercent = 1 - percent;
                vignetteMaterial.SetFloat("_VoronoiIntensity", Mathf.Lerp(0f, 0.3f, invPercent));
                vignetteMaterial.SetFloat("_VignetteRadiusPower", Mathf.Lerp(10f, 7f, invPercent));
            }
            else
            {
                ResetVignette();
            }
        }

        private void ResetVignette()
        {
            if (vignetteMaterial == null) return;
            vignetteMaterial.SetFloat("_VoronoiIntensity", 0f);
            vignetteMaterial.SetFloat("_VignetteRadiusPower", 10f);
        }

        private void OnDisable() => ResetVignette();
    }
}