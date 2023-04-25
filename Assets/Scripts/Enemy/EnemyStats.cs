using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Stats", menuName = "Enemy/Stats")]
public class EnemyStats : ScriptableObject
{
    public string enemyName;
    public int maxHealth;
}
