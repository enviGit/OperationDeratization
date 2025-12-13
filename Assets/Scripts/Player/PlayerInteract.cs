using RatGamesStudios.OperationDeratization.Interactables;
using RatGamesStudios.OperationDeratization.UI; // Zakładam, że InteractionUI tam wyląduje
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerInteract : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float interactDistance = 2.5f;
        [SerializeField] private float interactCooldown = 0.5f;
        [SerializeField] private LayerMask interactMask;

        [SerializeField] private float checkRate = 0.05f;

        [Header("References")]
        [SerializeField] private Camera cam;
        [SerializeField] private InteractionUI interactionUI;
        private PlayerInventory inventory;

        public RaycastHit CurrentHit { get; private set; }
        public AmmoBox CurrentAmmoBox { get; private set; }

        private float lastCheckTime;
        private float lastInteractTime;
        private Interactable currentInteractable;

        private void Start()
        {
            if (cam == null) cam = Camera.main;

            if (interactionUI == null) interactionUI = FindFirstObjectByType<InteractionUI>();

            inventory = GetComponent<PlayerInventory>();
            interactMask = ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("Postprocessing") | 1 << LayerMask.NameToLayer("Hitbox"));
        }

        private void Update()
        {
            HandleRaycastThrottled();
            HandleInput();
        }

        private void HandleRaycastThrottled()
        {
            if (Time.time - lastCheckTime > checkRate)
            {
                lastCheckTime = Time.time;
                PerformRaycast();
            }
        }

        private void PerformRaycast()
        {
            Ray ray = new Ray(cam.transform.position, cam.transform.forward);
            bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactMask);

            CurrentHit = hit;

            if (hitSomething)
            {
                Interactable interactable = hit.collider.GetComponent<Interactable>();
                CurrentAmmoBox = hit.collider.GetComponent<AmmoBox>();

                if (interactable != null)
                {
                    Weapon weaponInteractable = interactable as Weapon;

                    if (weaponInteractable != null && IsMainWeapon(weaponInteractable.gun))
                    {
                        Gun groundGun = weaponInteractable.gun;

                        bool shouldSwap = inventory.HasWeaponOfSameCategory(groundGun);

                        if (shouldSwap)
                        {
                            if (interactionUI)
                            {
                                Gun currentEquipped = GetBestWeaponToSwap(groundGun);
                                interactionUI.ShowWeaponComparison(groundGun, currentEquipped);
                            }
                            currentInteractable = interactable;
                        }
                        else
                        {
                            if (interactable != currentInteractable)
                            {
                                currentInteractable = interactable;
                                if (interactionUI)
                                {
                                    interactionUI.SetPrompt(interactable.prompt, groundGun.activeGunIcon);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (interactable != currentInteractable)
                        {
                            currentInteractable = interactable;
                            if (interactionUI) interactionUI.SetPrompt(interactable ? interactable.prompt : "");
                        }
                    }
                }
                else
                {
                    ClearInteraction();
                }
            }
            else
            {
                ClearInteraction();
                CurrentAmmoBox = null;
            }
        }

        private Gun GetBestWeaponToSwap(Gun groundGun)
        {
            if (inventory.CurrentWeapon.gunStyle == groundGun.gunStyle)
            {
                return inventory.CurrentWeapon;
            }

            foreach (var w in inventory.weapons)
            {
                if (w != null && w.gunStyle == groundGun.gunStyle)
                    return w;
            }
            return null;
        }

        private void ClearInteraction()
        {
            if (currentInteractable != null)
            {
                currentInteractable = null;
                if (interactionUI) interactionUI.HideAll();
            }
        }

        private bool IsMainWeapon(Gun gun)
        {
            if (gun == null) return false;
            return gun.gunStyle == GunStyle.Primary || gun.gunStyle == GunStyle.Secondary;
        }

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.F) && currentInteractable != null)
            {
                if (Time.time - lastInteractTime >= interactCooldown)
                {
                    currentInteractable.BaseInteract();
                    lastInteractTime = Time.time;

                    if (interactionUI) interactionUI.HideAll();
                    lastCheckTime = 0;
                }
            }
        }
    }
}