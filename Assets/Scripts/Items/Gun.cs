using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    public string gunName;
    public GameObject gunPrefab;
    public GameObject bulletPrefab;
    public Sprite gunIcon;
    public Sprite activeGunIcon;
    public GunType gunType;
    public GunStyle gunStyle;
    public AudioClip[] gunAudioClips;

    [Header("Stats")]
    public int magazineSize;
    public int currentAmmoCount;
    public int maxAmmoCount;
    public int minimumDamage;
    public int maximumDamage;
    public float range;
    public float timeBetweenShots;
    public float impactForce;

    [Header("Recoil stats")]
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    public float aimRecoilX;
    public float aimRecoilY;
    public float aimRecoilZ;
    public float snappiness;
    public float returnSpeed;
}
public enum GunType { Melee, Pistol, Rifle, Shotgun, Sniper }
public enum GunStyle { Melee, Primary, Secondary }