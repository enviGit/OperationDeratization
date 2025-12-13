using RatGamesStudios.OperationDeratization.Equipment;
using RatGamesStudios.OperationDeratization.Enemy;
using RatGamesStudios.OperationDeratization.Manager;
using RatGamesStudios.OperationDeratization.Player;
using System.Collections;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class AmmoBox : Interactable
    {
        [Header("Settings")]
        [SerializeField] private float refillTime = 2f;
        [SerializeField] private float maxDistance = 3.0f;

        [Header("References")]
        public Animator ammoBoxAnimator;
        private AudioSource lootingSound;
        private AudioEventManager audioEventManager;

        private GameObject player;
        private PlayerInventory inventory;
        private PlayerRefillHandler uiHandler;

        private bool isFilling = false;
        private bool canRefillCurrent = false;
        private ActiveWeapon currentActiveLogic;

        private void Start()
        {
            if (lootingSound == null) lootingSound = GetComponent<AudioSource>();
            if (ammoBoxAnimator == null) ammoBoxAnimator = GetComponent<Animator>();

            player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                inventory = player.GetComponent<PlayerInventory>();
                uiHandler = player.GetComponent<PlayerRefillHandler>();
            }

            var audioMgr = GameObject.FindGameObjectWithTag("AudioEventManager");
            if (audioMgr) audioEventManager = audioMgr.GetComponent<AudioEventManager>();
        }

        private void Update()
        {
            if (inventory == null) return;

            if (isFilling)
            {
                CheckIfShouldCancel();
                return;
            }

            CheckCurrentWeaponStatus();
        }

        private void CheckCurrentWeaponStatus()
        {
            Gun currentGun = inventory.CurrentWeapon;
            GameObject model = inventory.CurrentWeaponModel;

            currentActiveLogic = null;
            prompt = "";
            canRefillCurrent = false;

            if (currentGun == null || currentGun.gunStyle == GunStyle.Melee || IsGrenade(currentGun.gunStyle))
            {
                prompt = "";
                return;
            }

            if (model != null)
                currentActiveLogic = model.GetComponentInChildren<ActiveWeapon>();

            if (currentActiveLogic == null)
            {
                prompt = "Cannot refill";
                return;
            }

            if (currentActiveLogic.IsFull())
            {
                prompt = "Ammo Full";
            }
            else
            {
                prompt = "Refill Ammo";
                canRefillCurrent = true;
            }
        }

        protected override void Interact()
        {
            if (!isFilling && canRefillCurrent && currentActiveLogic != null)
            {
                StartCoroutine(RefillRoutine());
            }
        }

        private IEnumerator RefillRoutine()
        {
            isFilling = true;
            prompt = "";

            if (uiHandler) uiHandler.SetSliderVisible(true);

            if (ammoBoxAnimator) ammoBoxAnimator.SetTrigger("isLooting");
            if (lootingSound && !lootingSound.isPlaying)
            {
                lootingSound.Play();
                if (audioEventManager) audioEventManager.NotifyAudioEvent(lootingSound);
            }

            float timer = 0f;
            while (timer < refillTime)
            {
                timer += Time.deltaTime;
                float progress = timer / refillTime;

                if (uiHandler) uiHandler.UpdateSlider(progress, refillTime);

                yield return null;
            }

            if (currentActiveLogic != null)
            {
                currentActiveLogic.RefillAmmo(999);
            }

            StopRefill();
        }

        private void CheckIfShouldCancel()
        {
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);

                if (distance > maxDistance)
                {
                    StopAllCoroutines();
                    StopRefill();
                }
            }
            if (Input.GetMouseButtonDown(0))
            {
                StopAllCoroutines();
                StopRefill();
            }
        }

        private void StopRefill()
        {
            isFilling = false;
            if (uiHandler) uiHandler.SetSliderVisible(false);
            if (lootingSound && lootingSound.isPlaying) lootingSound.Stop();

            CheckCurrentWeaponStatus();
        }

        private bool IsGrenade(GunStyle style)
        {
            return style == GunStyle.Grenade || style == GunStyle.Flashbang || style == GunStyle.Smoke || style == GunStyle.Molotov;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                AiWeapons weapons = other.GetComponent<AiWeapons>();
                if (weapons != null && weapons.currentWeapon != null)
                {
                    var wScript = weapons.currentWeapon.GetComponent<Weapon>();
                    if (wScript && wScript.gun)
                    {
                        weapons.RefillAmmo(wScript.gun.magazineSize);
                        if (weapons.hasLootedAmmo)
                        {
                            ammoBoxAnimator.SetTrigger("isLooting");
                            lootingSound.Play();
                            if (audioEventManager) audioEventManager.NotifyAudioEvent(lootingSound);
                            weapons.hasLootedAmmo = false;
                        }
                    }
                }
            }
        }
    }
}