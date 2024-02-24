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
        private PlayerUI ui;
        private AudioSource lootingSound;
        public Animator ammoBoxAnimator;
        private AudioEventManager audioEventManager;

        [Header("Ammo")]
        public bool isFilling = false;
        private int allWeapons = 0;
        private int weaponsFullAmmo = 0;

        private void Start()
        {
            lootingSound = GetComponent<AudioSource>();
            player = GameObject.FindGameObjectWithTag("Player");
            audioEventManager = GameObject.FindGameObjectWithTag("AudioEventManager").GetComponent<AudioEventManager>();

            if (player != null)
            {
                inventory = player.GetComponent<PlayerInventory>();
                ui = player.GetComponent<PlayerUI>();
            }
        }
        private void Update()
        {
            IsFilling();
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
            if (inventory != null)
            {
                foreach (Gun gun in inventory.weapons)
                {
                    if (gun != null && gun.gunStyle != GunStyle.Melee && gun.gunStyle != GunStyle.Grenade && gun.gunStyle != GunStyle.Flashbang && gun.gunStyle != GunStyle.Smoke)
                    {
                        allWeapons++;

                        if (gun.maxAmmoCount >= gun.magazineSize * 3)
                        {
                            weaponsFullAmmo++;
                            ui.ShowAmmoRefillPrompt(gun.gunName);
                        }
                    }
                }
            }
            if (weaponsFullAmmo == allWeapons)
            {
                allWeapons = 0;
                weaponsFullAmmo = 0;

                return;
            }
            if (!isFilling)
            {
                lootingSound.Play();
                audioEventManager.NotifyAudioEvent(lootingSound);
                StartCoroutine(ui.RefillAmmo());
            }

            allWeapons = 0;
            weaponsFullAmmo = 0;
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