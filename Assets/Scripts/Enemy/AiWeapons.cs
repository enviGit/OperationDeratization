using UnityEngine;

public class AiWeapons : MonoBehaviour
{
    GameObject currentWeapon;
    Animator animator;
    MeshSockets weaponSockets;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
    public void Equip(GameObject weapon, MeshSockets sockets)
    {
        weaponSockets = sockets;
        currentWeapon = weapon;
        currentWeapon.GetComponent<Weapon>().enabled = false;
        sockets.Attach(currentWeapon.transform, MeshSockets.SocketId.Spine);
    }
    public void ActiveWeapon()
    {
        animator.SetBool("Equip", true);
    }
    public void DropWeapon()
    {
        if (currentWeapon)
        {
            currentWeapon.transform.SetParent(null);
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
            weaponSockets.Attach(currentWeapon.transform, MeshSockets.SocketId.RightHand);
    }
}
