using RatGamesStudios.OperationDeratization.Enemy;
using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.Player;
using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class AmmoBox : Interactable
    {
        [Header("References")]
        public TextMeshProUGUI ammoRefillPrompt;
        private GameObject player;
        private PlayerInventory inventory;
        private PlayerRefillHandler refillHandler;
        private AudioSource lootingSound;
        public Animator ammoBoxAnimator;
        private AudioEventManager audioEventManager;

        [Header("Ammo")]
        public bool isFilling = false;
        private int allWeapons = 0;
        private int weaponsFullAmmo = 0;

        private void Start()
        {
            if (lootingSound == null) lootingSound = GetComponent<AudioSource>();
            if (ammoBoxAnimator == null) ammoBoxAnimator = GetComponent<Animator>();

            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                refillHandler = player.GetComponent<PlayerRefillHandler>();
            }

            var audioMgr = GameObject.FindGameObjectWithTag("AudioEventManager");
            if (audioMgr) audioEventManager = audioMgr.GetComponent<AudioEventManager>();

            prompt = "Refill ammo";
        }
        private void Update()
        {
            HandleState();
        }
        private void HandleState()
        {
            if (isFilling)
            {
                prompt = "";

                if (!lootingSound.isPlaying)
                {
                    lootingSound.Play();
                    audioEventManager?.NotifyAudioEvent(lootingSound);
                    ammoBoxAnimator.SetTrigger("isLooting");
                }
            }
            else
            {
                prompt = "Refill ammo";
                if (lootingSound.isPlaying) lootingSound.Stop();
            }
        }
        private void IsFilling()
        {
            if (isFilling)
            {
                ammoBoxAnimator.SetTrigger("isLooting");
                prompt = "";
            }
            else
            {
                lootingSound.Stop();
                prompt = "Refill ammo";
            }
        }
        protected override void Interact()
        {
            if (refillHandler != null && !isFilling)
            {
                StartCoroutine(refillHandler.ProcessRefill(this));
            }
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                AiWeapons weapons = other.GetComponent<AiWeapons>();

                if (weapons.currentWeapon != null)
                {
                    weapons.RefillAmmo(weapons.currentWeapon.GetComponent<Weapon>().gun.magazineSize);

                    if (weapons.hasLootedAmmo)
                    {
                        ammoBoxAnimator.SetTrigger("isLooting");
                        lootingSound.Play();
                        audioEventManager.NotifyAudioEvent(lootingSound);
                        weapons.hasLootedAmmo = false;
                    }
                }
            }
        }
    }
}