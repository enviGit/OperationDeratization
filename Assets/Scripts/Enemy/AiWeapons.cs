using System.Collections;
using UnityEngine;

public class AiWeapons : MonoBehaviour
{
    /*public enum WeaponState
    {
        Holstering,
        Holstered,
        Activating,
        Active,
        Reloading
    }
    public enum WeaponSlot
    {
        Primary,
        Secondary
    }
    public Gun currentGun
    {
        get
        {
            return weapons[current];
        }
    }
    public WeaponSlot currentWeaponSlot
    {
        get
        {
            return (WeaponSlot)current;
        }
    }
    private Gun[] weapons = new Gun[2];
    private int current = 0;
    private WeaponState weaponState = WeaponState.Holstered
    private GameObject magazineHand
    public bool IsActive()
    {
        return weaponState == WeaponState.Active;
    }*/

    [Header("References")]
    [HideInInspector] public GameObject currentWeapon;
    private EnemyShoot weapon;
    private GameObject parentObject;
    public bool hasLootedAmmo = false;

    [Header("Weapons")]
    private Animator animator;
    private MeshSockets weaponSockets;
    private WeaponIk weaponIk;
    private Transform currentTarget;
    private bool weaponActive = false;
    public float inaccuracy = 0.4f;

    private void Start()
    {
        animator = GetComponent<Animator>();
        weaponIk = GetComponent<WeaponIk>();
        weapon = GetComponent<EnemyShoot>();
        parentObject = GameObject.Find("3D");
    }
    private void Update()
    {
        if (currentTarget && currentWeapon && weaponActive)
        {
            Vector3 target = currentTarget.position;
            target += Random.insideUnitSphere * inaccuracy;
            weapon.Shoot();
        }
    }
    public void SetFiring(bool enabled)
    {
        if (enabled)
            weapon.StartFiring();
        else
            weapon.StopFiring();
    }
    public void Equip(GameObject weapon, MeshSockets sockets)
    {
        weaponSockets = sockets;
        currentWeapon = weapon;
        currentWeapon.GetComponent<Weapon>().prompt = "";
        currentWeapon.GetComponent<Weapon>().enabled = false;
        currentWeapon.layer = LayerMask.NameToLayer("Default");
        SetLayerRecursively(currentWeapon, LayerMask.NameToLayer("Default"));
        sockets.Attach(currentWeapon.transform, MeshSockets.SocketId.Spine);
    }
    public void ActiveWeapon()
    {
        StartCoroutine(EquipWeapon());
    }
    private IEnumerator EquipWeapon()
    {
        animator.runtimeAnimatorController = currentWeapon.GetComponent<Weapon>().animator;
        animator.SetBool("Equip", true);

        yield return new WaitForSeconds(0.5f);

        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f)
            yield return null;

        weaponIk.SetAimTransform(currentWeapon.transform.Find("muzzle"));
        weaponActive = true;
    }
    public void DeactiveWeapon()
    {
        SetTarget(null);
        SetFiring(false);
        StartCoroutine(HolsterWeapon());
    }
    private IEnumerator HolsterWeapon()
    {
        weaponActive = false;
        animator.SetBool("Equip", false);

        yield return new WaitForSeconds(0.5f);

        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f)
            yield return null;

        if(currentWeapon != null)
            weaponIk.SetAimTransform(currentWeapon.transform.Find("muzzle"));
    }
    public void DropWeapon()
    {
        if (currentWeapon)
        {
            currentWeapon.transform.SetParent(parentObject.transform);
            Rigidbody rb = currentWeapon.AddComponent<Rigidbody>();
            rb.mass = 2f;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            currentWeapon.GetComponent<Weapon>().enabled = true;
            currentWeapon.layer = LayerMask.NameToLayer("Interactable");
            SetLayerRecursively(currentWeapon, LayerMask.NameToLayer("Interactable"));
            currentWeapon = null;
        }
    }
    public bool HasWeapon()
    {
        return currentWeapon != null;
    }
    public void OnAnimationEvent(string eventName)
    {
        if (eventName == "equipWeapon")
        {
            weaponSockets.Attach(currentWeapon.transform, MeshSockets.SocketId.RightHand);
            currentWeapon.transform.localPosition = new Vector3(0, 0, 0);
        }
        //if (eventName == "holsterWeapon")
            //weaponSockets.Attach(currentWeapon.transform, MeshSockets.SocketId.Spine);
    }
    public void SetTarget(Transform target)
    {
        weaponIk.SetTargetTransform(target);
        currentTarget = target;
    }
    public void RefillAmmo(int magazineSize)
    {
        Gun weapon = currentWeapon.GetComponent<Weapon>().gun;

        if (weapon && weapon.maxAmmoCount < weapon.editorAmmoValue * 3)
        {
            weapon.maxAmmoCount += magazineSize;
            hasLootedAmmo = true;
        }
    }
    public bool IsLowAmmo()
    {
        Gun weapon = currentWeapon.GetComponent<Weapon>().gun;
        
        if(weapon)
            return weapon.currentAmmoCount == 0 && weapon.maxAmmoCount == 0;

        return false;
    }
    public static void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;

        foreach (Transform child in obj.transform)
            SetLayerRecursively(child.gameObject, layer);
    }
}
