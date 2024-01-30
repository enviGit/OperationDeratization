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
        private PlayerInventory inventory;

        [Header("Impact Sounds")]
        public Material vignetteMaterial;
        private AudioSource heartbeatSound;
        private AudioSource impactSound;
        [SerializeField] private AudioClip[] impactClips = new AudioClip[3];
        [SerializeField] private AudioClip[] gasClips = new AudioClip[3];

        [Header("Death")]
        private Transform cam;
        [SerializeField] private bool isTutorialActive = false;
        [SerializeField] private AudioSource deathSounds;
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
            ragdoll = GetComponent<Ragdoll>();
            inventory = GetComponent<PlayerInventory>();
            frontHealthBar = healthBar.transform.GetChild(2).GetComponent<Image>();
            backHealthBar = healthBar.transform.GetChild(1).GetComponent<Image>();
            frontArmorBar = armorBar.transform.GetChild(2).GetComponent<Image>();
            backArmorBar = armorBar.transform.GetChild(1).GetComponent<Image>();
            cam = Camera.main.transform;
            var rigidBodies = GetComponentsInChildren<Rigidbody>();

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
            float percentMultiplier = 1.5f;
            float percent = (currentHealth * percentMultiplier) / maxHealth;
            lerpTimer = 0f;

            if (vignetteMaterial != null)
            {
                float voronoiIntensity = Mathf.Lerp(0f, 0.5f, 1 - percent);
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
        }
        public void TakeFallingDamage(float damage)
        {
            if (!isAlive)
                return;

            backHealthBar.color = Color.red;
            currentHealth -= damage;
            float percentMultiplier = 1.5f;
            float percent = (currentHealth * percentMultiplier) / maxHealth;
            lerpTimer = 0f;

            if (vignetteMaterial != null)
            {
                float voronoiIntensity = Mathf.Lerp(0f, 0.5f, 1 - percent);
                float vignetteRadiusPower = Mathf.Lerp(10f, 7f, 1 - percent);
                vignetteMaterial.SetFloat("_VoronoiIntensity", voronoiIntensity);
                vignetteMaterial.SetFloat("_VignetteRadiusPower", vignetteRadiusPower);
            }
            if (impactClips.Length > 0)
                impactSound.PlayOneShot(impactClips[2], 0.5f);
            if (currentHealth <= 0)
                Die();
        }
        public void TakeGasDamage(float damage)
        {
            if (!isAlive)
                return;

            backHealthBar.color = Color.red;
            currentHealth -= damage;
            float percentMultiplier = 1.5f;
            float percent = (currentHealth * percentMultiplier) / maxHealth;
            lerpTimer = 0f;

            if (vignetteMaterial != null)
            {
                float voronoiIntensity = Mathf.Lerp(0f, 0.5f, 1 - percent);
                float vignetteRadiusPower = Mathf.Lerp(10f, 7f, 1 - percent);
                vignetteMaterial.SetFloat("_VoronoiIntensity", voronoiIntensity);
                vignetteMaterial.SetFloat("_VignetteRadiusPower", vignetteRadiusPower);
            }
            if (impactClips.Length > 0)
            {
                int randomIndex = Random.Range(0, gasClips.Length - 1);
                impactSound.PlayOneShot(gasClips[randomIndex]);
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
            float percentMultiplier = 1.5f;
            float percent = (currentHealth * percentMultiplier) / maxHealth;
            lerpTimer = 0f;

            if (vignetteMaterial != null)
            {
                float voronoiIntensity = Mathf.Lerp(0f, 0.5f, 1 - percent);
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
        }
        private void Die()
        {
            isAlive = false;
            miniMapCanvas.SetActive(false);
            cam.SetParent(null);

            if(isTutorialActive)
            {
                cam.position = new Vector3(-23.95f, 6.14f, -16f);
                cam.rotation = Quaternion.Euler(32.69f, 44.98f, 0.05f);
            }
            else
            {
                cam.position = new Vector3(303.92f, 19.76f, -110.96f);
                cam.rotation = Quaternion.Euler(23.24f, 299.43f, 0.0456f);
            }

            cam.localScale = Vector3.one;

            if (deathClips.Length > 0)
            {
                deathSounds.clip = deathClips[0];
                deathSounds.Play();
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

            gameObject.SetActive(false);
            Destroy(cam.GetChild(0).gameObject);
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

            if (vignetteMaterial != null)
            {
                float percent = currentHealth / maxHealth;
                float voronoiIntensity = Mathf.Lerp(0f, 0.5f, 1 - percent);
                float vignetteRadiusPower = Mathf.Lerp(10f, 7f, 1 - percent);
                vignetteMaterial.SetFloat("_VoronoiIntensity", voronoiIntensity);
                vignetteMaterial.SetFloat("_VignetteRadiusPower", vignetteRadiusPower);
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