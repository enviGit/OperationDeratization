using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerStamina : MonoBehaviour
    {
        [Header("References")]
        private PlayerMotor playerMotor;
        public GameObject staminaBar;
        private Image frontStaminaBar;
        private Image backStaminaBar;
        private AudioSource heavyBreathingSound;

        [Header("Stamina bar")]
        public float maxStamina = 150f;
        public float sprintStaminaCost = 5f;
        public float jumpStaminaCost = 10f;
        public float attackStaminaCost = 15f;
        public float staminaRegenRate = 25f;
        public float currentStamina = 100f;
        private float lerpTimer;
        public float chipSpeed = 2f;
        public bool isStaminaRegenBlocked = false;

        private void Start()
        {
            playerMotor = GetComponent<PlayerMotor>();
            frontStaminaBar = staminaBar.transform.GetChild(3).GetComponent<Image>();
            backStaminaBar = staminaBar.transform.GetChild(2).GetComponent<Image>();
            currentStamina = maxStamina;
            staminaBar.SetActive(false);
            heavyBreathingSound = transform.Find("Sounds/HeavyBreathing").GetComponent<AudioSource>();
        }
        private void Update()
        {
            if (playerMotor.isRunning && playerMotor.isMoving)
                UseStamina(sprintStaminaCost * Time.deltaTime);
            if (!playerMotor.isRunning && playerMotor.isGrounded)
            {
                if (!isStaminaRegenBlocked)
                    currentStamina = Mathf.Clamp(currentStamina + staminaRegenRate * Time.deltaTime, 0, maxStamina);
            }

            UpdateStaminaUI();
        }
        private void UpdateStaminaUI()
        {
            if (currentStamina == maxStamina)
                staminaBar.SetActive(false);
            else
            {
                staminaBar.SetActive(true);

                float fillF = frontStaminaBar.fillAmount;
                float fillB = backStaminaBar.fillAmount;
                float hFraction = currentStamina / maxStamina;
                float hFractionNormalized = hFraction * 0.25f;

                if (fillB > hFractionNormalized)
                {
                    frontStaminaBar.fillAmount = hFractionNormalized;
                    lerpTimer += Time.deltaTime;
                    float percentComplete = lerpTimer / chipSpeed;
                    percentComplete = percentComplete * percentComplete;
                    backStaminaBar.fillAmount = Mathf.Lerp(fillB, hFractionNormalized, percentComplete);
                }
                if (fillF < hFractionNormalized)
                {
                    backStaminaBar.fillAmount = hFractionNormalized;
                    lerpTimer += Time.deltaTime;
                    float percentComplete = lerpTimer / chipSpeed;
                    percentComplete = percentComplete * percentComplete;
                    frontStaminaBar.fillAmount = Mathf.Lerp(fillF, backStaminaBar.fillAmount, percentComplete);
                }
            }
        }
        public bool HasStamina(float amount)
        {
            return currentStamina >= amount;
        }
        public void UseStamina(float amount)
        {
            currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);
            lerpTimer = 0f;
            UpdateStaminaUI();

            if (currentStamina == 0)
            {
                isStaminaRegenBlocked = true;
                heavyBreathingSound.Play();
                Invoke("UnblockStaminaRegen", 4f);
            }
        }
        private void UnblockStaminaRegen()
        {
            isStaminaRegenBlocked = false;
            heavyBreathingSound.Stop();
        }
        public void BlockStaminaOnAttack()
        {
            isStaminaRegenBlocked = true;

            if (currentStamina == 0)
                Invoke("UnblockStaminaRegen", 4f);
            else
                Invoke("UnblockStaminaRegen", 2f);
        }
    }
}