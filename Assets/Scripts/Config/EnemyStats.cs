using UnityEngine;

namespace RatGamesStudios.OperationDeratization
{
    [CreateAssetMenu(fileName = "Enemy Stats", menuName = "Enemy/Stats")]
    public class EnemyStats : ScriptableObject
    {
        [Header("Enemy")]
        public string enemyName;
        public float maxHealth;
    }
}