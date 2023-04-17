using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    public string gunName;
    public GameObject gunPrefab;
    public Sprite gunIcon;
    public GunType gunType;
    public GunStyle gunStyle;

    [Header("Stats")]
    public int magazineSize;
    public int magazineCount;
    public int minimumDamage;
    public int maximumDamage;
    public float range;
}
public enum GunType { Melee, Pistol, Rifle, Shotgun, Sniper }
public enum GunStyle { Melee, Primary, Secondary }