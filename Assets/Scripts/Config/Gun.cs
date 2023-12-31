using UnityEngine;

namespace RatGamesStudios.OperationDeratization
{
    [CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
    public class Gun : ScriptableObject
    {
        [Header("Weapon")]
        public string gunName;
        public GameObject gunPrefab;
        public Sprite gunIcon;
        public Sprite activeGunIcon;
        public GunType gunType;
        public GunStyle gunStyle;
        public AudioClip[] gunAudioClips;

        [Header("Stats")]
        public int editorAmmoValue;
        public int magazineSize;
        public int currentAmmoCount;
        public int maxAmmoCount;
        public int minimumDamage;
        public int maximumDamage;
        public float range;
        public float timeBetweenShots;
        public float impactForce;
        public bool autoFire;

        [Header("Recoil stats")]
        public float recoilX;
        public float recoilY;
        public float recoilZ;
        public float aimRecoilX;
        public float aimRecoilY;
        public float aimRecoilZ;
        public float snappiness;
        public float returnSpeed;

        private void OnEnable()
        {
            magazineSize = editorAmmoValue;
            currentAmmoCount = editorAmmoValue;
            maxAmmoCount = editorAmmoValue * 3;
        }
        public void OnLoad()
        {
            magazineSize = editorAmmoValue;
            currentAmmoCount = editorAmmoValue;
            maxAmmoCount = editorAmmoValue * 3;
        }
    }

    public enum GunType { Melee, Pistol, Revolver, Shotgun, Rifle, Sniper, Grenade, Flashbang, Smoke }
    public enum GunStyle { Melee, Primary, Secondary, Grenade, Flashbang, Smoke }
}