using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.RagdollPhysics;
using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject healthBar;
        [SerializeField] private GameObject armorBar;
        private Image frontHealthBar;
        [HideInInspector] public Image backHealthBar;
        private Image frontArmorBar;
        [HideInInspector] public Image backArmorBar;
        private Ragdoll ragdoll;
        public Transform inventoryUI;
        [SerializeField] private GameObject miniMapCanvas;
        [SerializeField] private Camera miniMapCamera;
        private PlayerInventory inventory;
        private AudioEventManager audioEventManager;
        private Transform sounds;

        [Header("Impact Sounds")]
        public Material vignetteMaterial;
        private AudioSource heartbeatSound;
        private AudioSource impactSound;
        [SerializeField] private AudioClip[] impactClips = new AudioClip[3];
        [SerializeField] private AudioClip[] gasClips = new AudioClip[3];

        [Header("Death")]
        private Transform cam;
        [SerializeField] private bool isTutorialActive = false;
        private AudioSource deathSounds;
        [SerializeField] private AudioClip[] deathClips = new AudioClip[2];

        [Header("Health")]
        public float currentHealth;
        private float lerpTimer;
        public float maxHealth = 100f;
        public float chipSpeed = 2f;
        public bool isAlive = true;

        [Header("Armor")]
        public float currentArmor = 0;
        public float maxArmor = 100f;
        [SerializeField] private Transform armorSocket;

        [Header("Heartbeat")]
        private float heartbeatMultiplier = 1.2f;
        private float initialMultiplier = 0.05f;

        private void Start()
        {
            heartbeatSound = transform.Find("Sounds/Heartbeat").GetComponent<AudioSource>();
            impactSound = transform.Find("Sounds/Impact").GetComponent<AudioSource>();
            deathSounds = transform.Find("Sounds/Death").GetComponent<AudioSource>();
            ragdoll = GetComponent<Ragdoll>();
            inventory = GetComponent<PlayerInventory>();
            sounds = transform.Find("Sounds");
            frontHealthBar = healthBar.transform.GetChild(2).GetComponent<Image>();
            backHealthBar = healthBar.transform.GetChild(1).GetComponent<Image>();
            frontArmorBar = armorBar.transform.GetChild(2).GetComponent<Image>();
            backArmorBar = armorBar.transform.GetChild(1).GetComponent<Image>();
            cam = Camera.main.transform;
            var rigidBodies = GetComponentsInChildren<Rigidbody>();
            audioEventManager = GameObject.FindGameObjectWithTag("AudioEventManager").GetComponent<AudioEventManager>();

            foreach (var rigidBody in rigidBodies)
            {
                HitBox hitBox = rigidBody.gameObject.AddComponent<HitBox>();
                hitBox.playerHealth = this;

                if (hitBox.gameObject != gameObject)
                    hitBox.gameObject.layer = LayerMask.NameToLayer("Hitbox");
            }

            vignetteMaterial.SetFloat("_VoronoiIntensity", 0);
            vignetteMaterial.SetFloat("_VignetteRadiusPower", 10);
        }
        private void Update()
        {
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            currentArmor = Mathf.Clamp(currentArmor, 0, maxArmor);
            UpdateHealthUI();
            Heartbeat();
        }
        private void Heartbeat()
        {
            float healthPercentage = currentHealth / maxHealth;

            if (healthPercentage <= 0.3f && isAlive == true)
            {
                if (!heartbeatSound.isPlaying)
                    heartbeatSound.Play();

                float pitchJumpMultiplier = Mathf.Lerp(initialMultiplier, 1f, 1f - healthPercentage);
                float volumeMultiplier = Mathf.Lerp(initialMultiplier, 1f, 1f - healthPercentage);
                heartbeatSound.volume = Mathf.Lerp(0.5f, 0.75f, 1f - healthPercentage) * heartbeatMultiplier * volumeMultiplier;
                heartbeatSound.pitch = Mathf.Lerp(1f, 1.5f, 1f - healthPercentage) * heartbeatMultiplier * pitchJumpMultiplier;
            }
            else
                heartbeatSound.Stop();
        }
        public void UpdateHealthUI()
        {
            float fillF = frontHealthBar.fillAmount;
            float fillB = backHealthBar.fillAmount;
            float hFraction = currentHealth / maxHealth;
            float hFractionNormalized = hFraction * 0.738f;

            if (fillB > hFractionNormalized)
            {
                frontHealthBar.fillAmount = hFractionNormalized;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                backHealthBar.fillAmount = Mathf.Lerp(fillB, hFractionNormalized, percentComplete);
            }
            if (fillF < hFractionNormalized)
            {
                backHealthBar.fillAmount = hFractionNormalized;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                frontHealthBar.fillAmount = Mathf.Lerp(fillF, backHealthBar.fillAmount, percentComplete);
            }
            if (!isAlive)
            {
                backArmorBar.fillAmount = 0;

                return;
            }

            float fillAF = frontArmorBar.fillAmount;
            float fillAB = backArmorBar.fillAmount;
            float aFraction = currentArmor / maxArmor;
            float aFractionNormalized = aFraction * 0.25f;

            if (fillAB > aFractionNormalized)
            {
                frontArmorBar.fillAmount = aFractionNormalized;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                backArmorBar.fillAmount = Mathf.Lerp(fillAB, aFractionNormalized, percentComplete);
            }
            if (fillAF < aFractionNormalized)
            {
                backArmorBar.fillAmount = aFractionNormalized;
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                frontArmorBar.fillAmount = Mathf.Lerp(fillAF, backArmorBar.fillAmount, percentComplete);
            }
        }
        public void TakeDamage(float damage)
        {
            if (!isAlive)
                return;

            backHealthBar.color = Color.red;
            backArmorBar.color = Color.gray;
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
            UpdateVignette();

            if (impactClips.Length > 0)
            {
                int randomIndex = Random.Range(0, impactClips.Length - 1);
                impactSound.pitch = Random.Range(0.85f, 1.15f);
                impactSound.PlayOneShot(impactClips[randomIndex]);
                audioEventManager.NotifyAudioEvent(impactSound);
            }
            if (currentHealth <= 0)
                Die();
        }
        public void TakeFallingDamage(float damage)
        {
            if (!isAlive)
                return;

            backHealthBar.color = Color.red;
            currentHealth -= damage;
            lerpTimer = 0f;
            UpdateVignette();

            if (impactClips.Length > 0)
            {
                impactSound.pitch = Random.Range(0.85f, 1.15f);
                impactSound.PlayOneShot(impactClips[2], 0.5f);
                audioEventManager.NotifyAudioEvent(impactSound);
            }
            if (currentHealth <= 0)
                Die();
        }
        public void TakeGasDamage(float damage)
        {
            if (!isAlive)
                return;

            backHealthBar.color = Color.red;
            currentHealth -= damage;
            lerpTimer = 0f;
            UpdateVignette();

            if (impactClips.Length > 0)
            {
                int randomIndex = Random.Range(0, gasClips.Length - 1);
                impactSound.pitch = Random.Range(0.85f, 1.15f);
                impactSound.PlayOneShot(gasClips[randomIndex]);
                audioEventManager.NotifyAudioEvent(impactSound);
            }
            if (currentHealth <= 0)
                Die();
        }
        public void TakeFireDamage(float damage)
        {
            if (!isAlive)
                return;

            backHealthBar.color = Color.red;
            currentHealth -= damage;
            lerpTimer = 0f;
            UpdateVignette();

            if (impactClips.Length > 0)
            {
                int randomIndex = Random.Range(0, impactClips.Length - 1);
                impactSound.pitch = Random.Range(0.85f, 1.15f);
                impactSound.PlayOneShot(impactClips[randomIndex]);
                audioEventManager.NotifyAudioEvent(impactSound);
            }
            if (currentHealth <= 0)
                Die();
        }
        private void Die()
        {
            isAlive = false;
            miniMapCanvas.SetActive(false);
            miniMapCamera.targetTexture = null;

            if (deathClips.Length > 0)
            {
                deathSounds.clip = deathClips[0];
                deathSounds.Play();
                audioEventManager.NotifyAudioEvent(deathSounds);
            }
            if (deathClips.Length > 1)
            {
                float delay = deathClips[0].length;
                Invoke("PlaySecondDeathClip", delay);
            }

            foreach (Gun weapon in inventory.weapons)
            {
                if (weapon != null && weapon.gunStyle != GunStyle.Melee)
                {
                    GameObject newWeapon = Instantiate(weapon.gunPrefab, transform.position + new Vector3(0f, 1f, 0f), Quaternion.identity);
                    Rigidbody rb;

                    if (newWeapon.GetComponent<Rigidbody>() != null)
                        rb = newWeapon.GetComponent<Rigidbody>();
                    else
                        rb = newWeapon.AddComponent<Rigidbody>();

                    rb.mass = 2f;
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                }
            }
            foreach(Transform child in cam.transform)
                child.gameObject.SetActive(false);

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
            if (!isAlive)
                return;

            currentHealth += healAmount;
            lerpTimer = 0f;
            UpdateVignette();
        }
        private void UpdateVignette()
        {
            if (vignetteMaterial != null)
            {
                if (currentHealth < 50)
                {
                    float percent = currentHealth / maxHealth;
                    float voronoiIntensity = Mathf.Lerp(0f, 0.3f, 1 - percent);
                    float vignetteRadiusPower = Mathf.Lerp(10f, 7f, 1 - percent);
                    vignetteMaterial.SetFloat("_VoronoiIntensity", voronoiIntensity);
                    vignetteMaterial.SetFloat("_VignetteRadiusPower", vignetteRadiusPower);
                }
                else
                {
                    vignetteMaterial.SetFloat("_VoronoiIntensity", 0f);
                    vignetteMaterial.SetFloat("_VignetteRadiusPower", 0f);
                }
            }
        }
        public void PickupArmor()
        {
            if (!isAlive)
                return;
            if (currentArmor > 99f)
                return;

            currentArmor = 100;
            lerpTimer = 0f;
        }
        private void OnDisable()
        {
            if (vignetteMaterial != null)
            {
                vignetteMaterial.SetFloat("_VoronoiIntensity", 0);
                vignetteMaterial.SetFloat("_VignetteRadiusPower", 10f);
            }
        }
    }
}