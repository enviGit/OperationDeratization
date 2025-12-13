using RatGamesStudios.OperationDeratization.Equipment;
using RatGamesStudios.OperationDeratization.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerInventory : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private GameObject wheels;
        [SerializeField] private AmmoDisplay ammoDisplay;

        private Collider playerCollider;
        private PlayerWeaponController weaponController;
        private PlayerGrenadeController grenadeController;
        private PlayerRefillHandler refillHandler;

        [Header("Weapon Models Setup")]
        public List<WeaponModelEntry> allAvailableModels = new List<WeaponModelEntry>();

        [System.Serializable]
        public class WeaponModelEntry
        {
            public string id;
            public GameObject modelPrefab;
        }

        private GameObject[] activeSlotModels = new GameObject[7];

        [Header("Inventory Data")]
        [SerializeField] private Gun meleeData;
        public Gun[] weapons = new Gun[7];

        public int currentWeaponIndex = -1;
        [HideInInspector] public int currentItemIndex = 0;
        public bool isSwitchingWeapon = false;

        public int GetGrenadeCount(int index)
        {
            if (!IsIndexValid(index) || activeSlotModels[index] == null) return 0;
            var logic = activeSlotModels[index].GetComponentInChildren<ActiveWeapon>();
            return logic ? logic.TotalAmmo : 0;
        }

        public int grenadeCount => GetGrenadeCount(3);
        public int flashbangCount => GetGrenadeCount(4);
        public int smokeCount => GetGrenadeCount(5);
        public int molotovCount => GetGrenadeCount(6);

        public GameObject CurrentWeaponModel => IsIndexValid(currentWeaponIndex) ? activeSlotModels[currentWeaponIndex] : null;
        public Gun CurrentWeapon => IsIndexValid(currentWeaponIndex) ? weapons[currentWeaponIndex] : null;

        private bool IsIndexValid(int i) => i >= 0 && i < weapons.Length;

        private void Start()
        {
            weaponController = GetComponent<PlayerWeaponController>();
            grenadeController = GetComponent<PlayerGrenadeController>();
            refillHandler = GetComponent<PlayerRefillHandler>();

            playerCollider = GetComponent<Collider>();
            if (!playerCollider) playerCollider = GetComponentInChildren<Collider>();

            DisableAllPhysicalModels();
            if (meleeData != null) AddItem(meleeData);
            StartCoroutine(SwitchWeapon(0));
        }

        public bool HasWeaponOfSameCategory(Gun newGun)
        {
            if (newGun == null) return false;

            int idx = (int)newGun.gunStyle;

            if (idx >= 0 && idx < weapons.Length)
            {
                return weapons[idx] != null;
            }
            return false;
        }

        private void DisableAllPhysicalModels()
        {
            foreach (var entry in allAvailableModels)
                if (entry.modelPrefab != null) entry.modelPrefab.SetActive(false);
        }

        private void Update()
        {
            HandleInput();
            HandleDrop();
        }

        public bool AddItem(Gun newItem)
        {
            if (newItem == null) return false;

            int slotIndex = (int)newItem.gunStyle;
            if (slotIndex < 0 || slotIndex >= weapons.Length) return false;

            if (weapons[slotIndex] != null)
            {
                if (IsGrenade(newItem.gunStyle))
                {
                    GameObject existingModel = activeSlotModels[slotIndex];
                    if (existingModel != null)
                    {
                        ActiveWeapon logic = existingModel.GetComponentInChildren<ActiveWeapon>();
                        if (logic != null)
                        {
                            return logic.AddAmmo(1);
                        }
                    }
                    return false;
                }
                DropWeapon(slotIndex);
            }

            GameObject modelForThisGun = null;
            var entry = allAvailableModels.FirstOrDefault(x => x.id == newItem.gunName);

            if (entry != null) modelForThisGun = entry.modelPrefab;
            else
            {
                Debug.LogError($"[Inventory] Missing model for '{newItem.gunName}'!");
                return false;
            }

            weapons[slotIndex] = newItem;
            activeSlotModels[slotIndex] = modelForThisGun;

            if (modelForThisGun != null)
            {
                ActiveWeapon logic = modelForThisGun.GetComponentInChildren<ActiveWeapon>();
                if (logic != null)
                {
                    if (IsGrenade(newItem.gunStyle))
                    {
                        logic.InitializeAsSinglePickup();
                    }
                    else
                    {
                        logic.InitializeAmmo();
                    }
                }
            }

            if (slotIndex == currentWeaponIndex || ((newItem.gunStyle == GunStyle.Primary || newItem.gunStyle == GunStyle.Secondary) && currentWeaponIndex == 0))
            {
                StartCoroutine(PullOutWeapon(slotIndex));
            }

            return true;
        }

        private void HandleInput()
        {
            if ((weaponController != null && weaponController.isAiming) || (wheels != null && wheels.activeSelf) || isSwitchingWeapon) return;

            int scrollDelta = (int)Input.mouseScrollDelta.y;
            if (scrollDelta != 0)
            {
                int newIndex = FindNextWeaponIndex(scrollDelta);
                if (newIndex != currentWeaponIndex && newIndex != -1)
                {
                    StartCoroutine(SwitchWeapon(newIndex));
                    UpdateGrenadeIndex(newIndex);
                }
            }
            if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchToSlot(0);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchToSlot(1);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchToSlot(2);
            else if (Input.GetKeyDown(KeyCode.Alpha4)) CycleGrenades();
        }

        private void SwitchToSlot(int index)
        {
            if (!IsIndexValid(index) || weapons[index] == null) return;
            StartCoroutine(SwitchWeapon(index));
            currentItemIndex = -1;
        }

        private void CycleGrenades()
        {
            currentItemIndex++;
            if (currentItemIndex > 3) currentItemIndex = 0;
            int attempts = 0;
            int newWeaponIndex = currentItemIndex + 3;
            while ((!IsIndexValid(newWeaponIndex) || weapons[newWeaponIndex] == null) && attempts < 4)
            {
                currentItemIndex++;
                if (currentItemIndex > 3) currentItemIndex = 0;
                newWeaponIndex = currentItemIndex + 3;
                attempts++;
            }
            if (IsIndexValid(newWeaponIndex) && weapons[newWeaponIndex] != null) StartCoroutine(SwitchWeapon(newWeaponIndex));
        }

        private void UpdateGrenadeIndex(int newIndex)
        {
            if (newIndex >= 3 && newIndex <= 6) currentItemIndex = newIndex - 3;
            else currentItemIndex = -1;
        }

        public void HandleDrop()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                if (CurrentWeapon != null && (CurrentWeapon.gunStyle == GunStyle.Primary || CurrentWeapon.gunStyle == GunStyle.Secondary))
                {
                    DropWeapon(currentWeaponIndex);
                    StartCoroutine(SwitchWeapon(0));
                }
            }
        }

        private void DropWeapon(int index)
        {
            if (!IsIndexValid(index)) return;
            Gun weaponToDrop = weapons[index];
            if (weaponToDrop == null) return;

            if (weaponToDrop.gunPrefab != null)
            {
                Vector3 dropPos = transform.position + transform.forward * 1.0f + Vector3.up * 1.5f;
                GameObject pickup = Instantiate(weaponToDrop.gunPrefab, dropPos, Quaternion.identity);
                Collider pickUpCollider = pickup.GetComponent<Collider>();
                if (playerCollider != null && pickup != null)
                {
                    Physics.IgnoreCollision(playerCollider, pickUpCollider, true);
                }
                pickup.layer = LayerMask.NameToLayer("Interactable");
                SetLayerRecursively(pickup, pickup.layer);
                Rigidbody rb = pickup.AddComponent<Rigidbody>();
                rb.mass = 2f;
                rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                rb.AddForce((transform.forward * 2f + Vector3.up * 1f), ForceMode.Impulse);
                float tumbleForce = 0.5f;

                rb.AddTorque(new Vector3(
                    Random.Range(-1f, 1f),
                    0f,
                    Random.Range(-0.2f, 0.2f)
                ) * tumbleForce, ForceMode.Impulse);
            }
            if (activeSlotModels[index] != null)
            {
                activeSlotModels[index].SetActive(false);
                activeSlotModels[index] = null;
            }
            weapons[index] = null;
        }

        public IEnumerator SwitchWeapon(int newIndex)
        {
            if (!IsIndexValid(newIndex) || weapons[newIndex] == null) yield break;
            if (newIndex == currentWeaponIndex && activeSlotModels[newIndex] != null && activeSlotModels[newIndex].activeSelf) yield break;
            isSwitchingWeapon = true;
            yield return MoveWeaponHolder(new Vector3(0f, -0.5f, 0f), 0.2f);
            if (IsIndexValid(currentWeaponIndex) && activeSlotModels[currentWeaponIndex] != null) activeSlotModels[currentWeaponIndex].SetActive(false);
            currentWeaponIndex = newIndex;
            GameObject nextModel = activeSlotModels[currentWeaponIndex];
            if (nextModel != null) nextModel.SetActive(true);

            Gun newGunData = weapons[currentWeaponIndex];
            ActiveWeapon activeLogic = null;
            if (nextModel != null) activeLogic = nextModel.GetComponentInChildren<ActiveWeapon>();

            if (ammoDisplay) ammoDisplay.SetWeapon(newGunData, activeLogic);
            if (weaponController) weaponController.OnWeaponChanged(newGunData, nextModel);
            if (grenadeController) grenadeController.OnWeaponChanged(newGunData, nextModel);

            yield return MoveWeaponHolder(Vector3.zero, 0.2f);
            isSwitchingWeapon = false;
        }

        public IEnumerator PullOutWeapon(int newIndex)
        {
            if (!IsIndexValid(newIndex) || weapons[newIndex] == null) yield break;
            isSwitchingWeapon = true;
            if (IsIndexValid(currentWeaponIndex) && activeSlotModels[currentWeaponIndex] != null) activeSlotModels[currentWeaponIndex].SetActive(false);
            currentWeaponIndex = newIndex;
            GameObject nextModel = activeSlotModels[currentWeaponIndex];
            if (nextModel != null) nextModel.SetActive(true);
            Gun newGunData = weapons[currentWeaponIndex];
            ActiveWeapon activeLogic = null;
            if (nextModel != null) activeLogic = nextModel.GetComponentInChildren<ActiveWeapon>();
            if (ammoDisplay) ammoDisplay.SetWeapon(newGunData, activeLogic);
            if (weaponController) weaponController.OnWeaponChanged(newGunData, nextModel);
            if (grenadeController) grenadeController.OnWeaponChanged(newGunData, nextModel);
            weaponHolder.localPosition = new Vector3(0f, -0.5f, 0f);
            yield return MoveWeaponHolder(Vector3.zero, 0.25f);
            isSwitchingWeapon = false;
        }

        private IEnumerator MoveWeaponHolder(Vector3 targetPos, float duration)
        {
            Vector3 startPos = weaponHolder.localPosition;
            float time = 0;
            while (time < duration)
            {
                weaponHolder.localPosition = Vector3.Lerp(startPos, targetPos, time / duration);
                time += Time.deltaTime;
                yield return null;
            }
            weaponHolder.localPosition = targetPos;
        }

        private int FindNextWeaponIndex(int dir)
        {
            int newIndex = currentWeaponIndex;
            if (weapons.Length == 0) return -1;
            int loopSafety = 0;
            do
            {
                newIndex = (newIndex + dir) % weapons.Length;
                if (newIndex < 0) newIndex += weapons.Length;
                loopSafety++;
                if (loopSafety > weapons.Length * 2) return currentWeaponIndex;
            } while (weapons[newIndex] == null && newIndex != currentWeaponIndex);
            return weapons[newIndex] == null ? -1 : newIndex;
        }

        private bool IsGrenade(GunStyle style) => style == GunStyle.Grenade || style == GunStyle.Flashbang || style == GunStyle.Smoke || style == GunStyle.Molotov;
        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.layer = layer;
            foreach (Transform child in obj.transform) SetLayerRecursively(child.gameObject, layer);
        }

        public void RemoveItem()
        {
            if (IsIndexValid(currentWeaponIndex))
            {
                if (activeSlotModels[currentWeaponIndex] != null) activeSlotModels[currentWeaponIndex].SetActive(false);
                weapons[currentWeaponIndex] = null;
            }
        }
    }
}