using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.UI;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerStamina : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private PlayerMotor playerMotor;
        [SerializeField] private ResourceBar staminaBarUI;
        [SerializeField] private AudioSource heavyBreathingSound;

        [Header("Settings")]
        public float maxStamina = 150f;
        public float currentStamina = 100f;
        public float staminaRegenRate = 25f;

        [Header("Costs")]
        public float sprintStaminaCost = 5f;
        public float jumpStaminaCost = 10f;
        public float attackStaminaCost = 15f;

        public bool isStaminaRegenBlocked = false;
        private AudioEventManager audioEventManager;

        private void Start()
        {
            if (playerMotor == null) playerMotor = GetComponent<PlayerMotor>();
            if (heavyBreathingSound == null) heavyBreathingSound = transform.Find("Sounds/HeavyBreathing")?.GetComponent<AudioSource>();

            var audioMgr = GameObject.FindGameObjectWithTag("AudioEventManager");
            if (audioMgr) audioEventManager = audioMgr.GetComponent<AudioEventManager>();

            currentStamina = maxStamina;
            if (staminaBarUI) staminaBarUI.SetActive(false);
        }

        private void Update()
        {
            HandleStaminaRegen();
            UpdateUI();
        }

        private void HandleStaminaRegen()
        {
            if (playerMotor.isRunning && playerMotor.isMoving)
            {
                UseStamina(sprintStaminaCost * Time.deltaTime);
            }
            else if (!playerMotor.isRunning && playerMotor.isGrounded)
            {
                if (!isStaminaRegenBlocked && currentStamina < maxStamina)
                {
                    if (staminaBarUI) staminaBarUI.SetBackColor(new Color(0.88f, 0.31f, 0.12f, 1f));
                    currentStamina = Mathf.Clamp(currentStamina + staminaRegenRate * Time.deltaTime, 0, maxStamina);
                }
            }
        }

        private void UpdateUI()
        {
            if (staminaBarUI == null) return;

            if (currentStamina >= maxStamina)
            {
                staminaBarUI.SetActive(false);
            }
            else
            {
                staminaBarUI.SetActive(true);
                staminaBarUI.UpdateBar(currentStamina, maxStamina);
            }
        }

        public bool HasStamina(float amount)
        {
            return currentStamina >= amount;
        }

        public void UseStamina(float amount)
        {
            if (staminaBarUI) staminaBarUI.SetBackColor(Color.gray);

            currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);

            if (currentStamina <= 0.1f)
            {
                BlockStaminaRegen(4f);
                if (!heavyBreathingSound.isPlaying)
                {
                    heavyBreathingSound.Play();
                    audioEventManager?.NotifyAudioEvent(heavyBreathingSound);
                }
            }
        }

        public void BlockStaminaOnAttack()
        {
            BlockStaminaRegen(currentStamina <= 0 ? 4f : 2f);
        }

        private void BlockStaminaRegen(float duration)
        {
            isStaminaRegenBlocked = true;
            CancelInvoke(nameof(UnblockStaminaRegen));
            Invoke(nameof(UnblockStaminaRegen), duration);
        }

        private void UnblockStaminaRegen()
        {
            isStaminaRegenBlocked = false;
            heavyBreathingSound.Stop();
        }
    }
}