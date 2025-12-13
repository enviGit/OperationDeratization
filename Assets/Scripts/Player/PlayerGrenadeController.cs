using RatGamesStudios.OperationDeratization.Equipment;
using RatGamesStudios.OperationDeratization.Interactables;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Player
{
    public class PlayerGrenadeController : MonoBehaviour
    {
        [Header("Throw Settings")]
        public float throwForce = 20f;
        public float throwUpForce = 5f;
        [SerializeField] private float throwCooldown = 1.0f;

        [Header("Weak Throw Settings")]
        public float weakThrowForce = 8f;
        public float weakThrowUpForce = 2f;

        [Header("Trajectory Settings")]
        [SerializeField] private int linePoints = 40;
        [SerializeField] private float timeStep = 0.05f;
        [SerializeField] private LayerMask grenadeCollisionMask;

        [Header("Physics Settings")]
        [SerializeField] private float grenadeMass = 2f;

        [Header("References")]
        private LineRenderer lineRenderer;
        private Transform weaponHolder;
        private AudioSource throwAudio;

        private Gun currentWeapon;
        private GameObject currentWeaponModel;
        private ActiveWeapon activeGunLogic;
        private PlayerInventory inventory;
        private Collider playerCollider;
        private float lastThrowTime = -100f;

        private void Start()
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
            if (!lineRenderer && Camera.main) lineRenderer = Camera.main.GetComponent<LineRenderer>();

            weaponHolder = Camera.main.transform.Find("WeaponHolder");
            inventory = GetComponent<PlayerInventory>();
            playerCollider = GetComponent<Collider>();
            if (!playerCollider) playerCollider = GetComponentInChildren<Collider>();

            throwAudio = transform.Find("Sounds/WeaponFire")?.GetComponent<AudioSource>();
        }

        public void OnWeaponChanged(Gun newGun, GameObject newModel)
        {
            currentWeapon = newGun;
            currentWeaponModel = newModel;

            if (newModel != null)
                activeGunLogic = newModel.GetComponentInChildren<ActiveWeapon>();
            else
                activeGunLogic = null;

            if (lineRenderer) lineRenderer.enabled = false;
        }

        private void Update()
        {
            if (currentWeapon == null || !IsGrenade(currentWeapon.gunStyle) || activeGunLogic == null)
            {
                if (lineRenderer) lineRenderer.enabled = false;
                return;
            }

            if (Time.time - lastThrowTime < throwCooldown)
            {
                if (lineRenderer) lineRenderer.enabled = false;
                return;
            }

            if (activeGunLogic.TotalAmmo > 0)
            {
                bool isAiming = Input.GetMouseButton(1);

                if (isAiming)
                {
                    DrawTrajectory(true);
                }
                else
                {
                    if (lineRenderer) lineRenderer.enabled = false;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    ThrowGrenade(isAiming);
                }
            }
            else
            {
                if (lineRenderer) lineRenderer.enabled = false;
            }
        }

        private Vector3 GetThrowVelocity(bool isStrongThrow)
        {
            float force = isStrongThrow ? throwForce : weakThrowForce;
            float upForce = isStrongThrow ? throwUpForce : weakThrowUpForce;

            Vector3 forceVector = weaponHolder.forward * force + weaponHolder.up * upForce;

            return forceVector / grenadeMass;
        }

        private void DrawTrajectory(bool isStrongThrow)
        {
            if (!lineRenderer) return;

            lineRenderer.enabled = true;
            lineRenderer.positionCount = linePoints;

            Vector3 startPos = weaponHolder.position;
            Vector3 startVelocity = GetThrowVelocity(isStrongThrow);

            lineRenderer.SetPosition(0, startPos);

            Vector3 previousPos = startPos;

            for (int i = 1; i < linePoints; i++)
            {
                // s = s0 + v0*t + 0.5*g*t^2
                float t = i * timeStep;

                Vector3 currentPos = startPos + (startVelocity * t) + (Physics.gravity * 0.5f * t * t);

                if (Physics.Raycast(previousPos, (currentPos - previousPos).normalized, out RaycastHit hit, (currentPos - previousPos).magnitude, grenadeCollisionMask))
                {
                    lineRenderer.SetPosition(i, hit.point);
                    lineRenderer.positionCount = i + 1;
                    return;
                }

                lineRenderer.SetPosition(i, currentPos);
                previousPos = currentPos;
            }
        }

        private void ThrowGrenade(bool isStrongThrow)
        {
            if (!activeGunLogic.TryShoot()) return;

            lastThrowTime = Time.time;

            if (throwAudio && currentWeapon.gunAudioClips.Length > 0)
                throwAudio.PlayOneShot(currentWeapon.gunAudioClips[0]);

            Vector3 spawnPos = weaponHolder.position + weaponHolder.forward * 0.5f;

            GameObject grenadeObj = Instantiate(currentWeapon.gunPrefab, spawnPos, weaponHolder.rotation);

            Vector3 finalVelocity = GetThrowVelocity(isStrongThrow);

            PrepareGrenadePhysics(grenadeObj, finalVelocity);
            ApplyGrenadeLogic(grenadeObj);

            if (activeGunLogic.TotalAmmo <= 0)
            {
                inventory.RemoveItem();
                if (inventory.currentWeaponIndex >= 3)
                {
                    inventory.StartCoroutine(inventory.SwitchWeapon(0));
                }
            }
        }

        private void PrepareGrenadePhysics(GameObject obj, Vector3 initialVelocity)
        {
            var pickupScript = obj.GetComponent<Weapon>();
            if (pickupScript) Destroy(pickupScript);

            obj.AddComponent<GrenadeIndicator>();

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (!rb) rb = obj.AddComponent<Rigidbody>();

            rb.mass = grenadeMass;

            Collider grenadeCollider = obj.GetComponent<Collider>();
            if (playerCollider != null && grenadeCollider != null)
            {
                Physics.IgnoreCollision(playerCollider, grenadeCollider, true);
            }

            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.linearVelocity = initialVelocity;

            rb.AddTorque(Random.insideUnitSphere * 10f, ForceMode.Impulse);
        }

        private void ApplyGrenadeLogic(GameObject obj)
        {
            if (currentWeapon.gunStyle == GunStyle.Grenade) obj.GetComponent<Grenade>().shouldExplode = true;
            else if (currentWeapon.gunStyle == GunStyle.Flashbang) obj.AddComponent<Flashbang>().shouldFlash = true;
            else if (currentWeapon.gunStyle == GunStyle.Smoke) obj.GetComponent<Smoke>().shouldSmoke = true;
            else if (currentWeapon.gunStyle == GunStyle.Molotov) obj.GetComponent<Molotov>().shouldExplode = true;
        }

        private bool IsGrenade(GunStyle style) => style == GunStyle.Grenade || style == GunStyle.Flashbang || style == GunStyle.Smoke || style == GunStyle.Molotov;
    }
}