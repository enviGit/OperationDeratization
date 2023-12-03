using System.Collections;
using UnityEngine;

public class AiWeapons : MonoBehaviour
{
    [Header("References")]
    [HideInInspector] public GameObject currentWeapon;
    private EnemyShoot weapon;
    private GameObject parentObject;

    [Header("Weapons")]
    Animator animator;
    MeshSockets weaponSockets;
    WeaponIk weaponIk;
    Transform currentTarget;
    bool weaponActive = false;
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
            Vector3 target = currentTarget.position + weaponIk.targetOffset;
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
        sockets.Attach(currentWeapon.transform, MeshSockets.SocketId.Spine);
    }
    public void ActiveWeapon()
    {
        StartCoroutine(EquipWeapon());
    }
    IEnumerator EquipWeapon()
    {
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
    IEnumerator HolsterWeapon()
    {
        weaponActive = false;
        animator.SetBool("Equip", false);
        yield return new WaitForSeconds(0.5f);

        while (animator.GetCurrentAnimatorStateInfo(1).normalizedTime < 1f)
            yield return null;

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
}
