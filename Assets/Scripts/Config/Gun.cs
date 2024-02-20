using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public float reloadTime;

        [Header("Recoil stats")]
        public float recoilX;
        public float recoilY;
        public float recoilZ;
        public float aimRecoilX;
        public float aimRecoilY;
        public float aimRecoilZ;
        public float snappiness;
        public float returnSpeed;
        private Dictionary<GunType, (Vector3 position, Vector3 rotation, Vector3 aimingPosition, Vector3 aimingRotation)> gunTypePositions = new Dictionary<GunType, (Vector3 position, Vector3 rotation, Vector3 aimingPosition, Vector3 aimingRotation)>()
        {
            { GunType.Melee, (new Vector3(0.05f, -0.0578f, 0.1701f), new Vector3(20.84f, -161.87f, 100f), new Vector3(0.05f, -0.0578f, 0.1701f), new Vector3(20.84f, -161.87f, 100f)) },
            { GunType.Pistol, (new Vector3(0.18f, -0.12f, 0.46f), new Vector3(3f, 5f, 0), new Vector3(0, -0.07f, 0.52f), new Vector3(0, 0, 0)) },
            { GunType.Revolver, (new Vector3(0.19f, -0.22f, 0.35f), new Vector3(-90f, 5f, 0), new Vector3(0, -0.173f, 0.4f), new Vector3(-87f, 0, 0)) },
            { GunType.Shotgun, (new Vector3(0.16f, -0.23f, 0.44f), new Vector3(3f, 5f, 0), new Vector3(0.015f, -0.15f, 0.56f), new Vector3(5f, 0.5f, 0)) },
            { GunType.Rifle, (new Vector3(0.16f, -0.27f, 0.25f), new Vector3(3f, 5f, 0), new Vector3(0, -0.17f, 0.47f), new Vector3(0, 0, 0)) },
            { GunType.Sniper, (new Vector3(0.12f, -0.23f, 0.45f), new Vector3(3f, 5f, 0), new Vector3(0.0119f, -0.14f, 0.5f), Vector3.zero) },
            { GunType.Grenade, (new Vector3(0.16f, -0.15f, 0.3f), new Vector3(3f, 0, 0), new Vector3(0.16f, -0.15f, 0.3f), new Vector3(3f, 0, 0)) },
            { GunType.Flashbang, (new Vector3(0.16f, -0.15f, 0.3f), new Vector3(3f, 0, 0), new Vector3(0.16f, -0.15f, 0.3f), new Vector3(3f, 0, 0)) },
            { GunType.Smoke, (new Vector3(0.16f, -0.15f, 0.3f), new Vector3(3f, 0, 0), new Vector3(0.16f, -0.15f, 0.3f), new Vector3(3f, 0, 0)) },
            { GunType.Molotov, (new Vector3(0.12f, -0.16f, 0.17f), new Vector3(3f, 0, 0), new Vector3(0.12f, -0.16f, 0.17f), new Vector3(3f, 0, 0)) }
        };
        public Dictionary<GunType, (Vector3 position, Vector3 rotation, Vector3 aimingPosition, Vector3 aimingRotation)> GunTypePositions
        {
            get
            {
                return new Dictionary<GunType, (Vector3 position, Vector3 rotation, Vector3 aimingPosition, Vector3 aimingRotation)>(gunTypePositions);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            magazineSize = editorAmmoValue;
            currentAmmoCount = editorAmmoValue;
            maxAmmoCount = editorAmmoValue * 3;
        }
    }

    public enum GunType { Melee, Pistol, Revolver, Shotgun, Rifle, Sniper, Grenade, Flashbang, Smoke, Molotov }
    public enum GunStyle { Melee, Primary, Secondary, Grenade, Flashbang, Smoke, Molotov }
}