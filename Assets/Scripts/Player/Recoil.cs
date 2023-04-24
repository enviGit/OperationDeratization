using UnityEngine;

public class Recoil : MonoBehaviour
{
    [SerializeField]
    private PlayerMotor playerMot;
    [SerializeField]
    private PlayerInventory playerInv;
    private Gun gun;
    private bool isAiming;
    private Vector3 currentRotation;
    private Vector3 targetRotation;

    private void Update()
    {
        isAiming = playerMot.isAiming;
        gun = playerInv.CurrentWeapon;
        targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, gun.returnSpeed * Time.deltaTime);
        currentRotation = Vector3.Slerp(currentRotation, targetRotation, gun.snappiness * Time.fixedDeltaTime);
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
    public void RecoilFire()
    {
        if(isAiming)
            targetRotation += new Vector3(gun.recoilX, Random.Range(-gun.aimRecoilY, gun.aimRecoilY), Random.Range(-gun.aimRecoilZ, gun.aimRecoilZ));
        else
            targetRotation += new Vector3(gun.recoilX, Random.Range(-gun.recoilY, gun.recoilY), Random.Range(-gun.recoilZ, gun.recoilZ));
    }
}
