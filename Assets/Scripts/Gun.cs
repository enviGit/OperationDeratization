using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "Gun")]
public class Gun : ScriptableObject
{
    public string gunName;
    public GameObject gunPrefab;
    [Header("Stats")]
    public int minimumDamage;
    public int maximumDamage;
    public float maximumRange;
}
