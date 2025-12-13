using RatGamesStudios.OperationDeratization.Player;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class WeaponRecoil : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerInventory playerInv;
        [SerializeField] private PlayerWeaponController playerShoot;
        private Gun gun;

        [Header("Recoil")]
        private bool isAiming;
        private Vector3 currentRotation;
        private Vector3 targetRotation;

        private void Update()
        {
            gun = playerInv.CurrentWeapon;

            if (gun == null) return;

            isAiming = playerShoot.isAiming;

            targetRotation = Vector3.Lerp(targetRotation, Vector3.zero, gun.returnSpeed * Time.deltaTime);
            currentRotation = Vector3.Slerp(currentRotation, targetRotation, gun.snappiness * Time.fixedDeltaTime);
            transform.localRotation = Quaternion.Euler(currentRotation);
        }

        public void RecoilFire()
        {
            if (gun == null) return;

            if (isAiming)
                targetRotation += new Vector3(gun.recoilX, Random.Range(-gun.aimRecoilY, gun.aimRecoilY), Random.Range(-gun.aimRecoilZ, gun.aimRecoilZ));
            else
                targetRotation += new Vector3(gun.recoilX, Random.Range(-gun.recoilY, gun.recoilY), Random.Range(-gun.recoilZ, gun.recoilZ));
        }
    }
}