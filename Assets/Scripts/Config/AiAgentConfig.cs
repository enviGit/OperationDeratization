using UnityEngine;

namespace RatGamesStudios.OperationDeratization
{
    [CreateAssetMenu()]
    public class AiAgentConfig : ScriptableObject
    {
        public float maxTime = 3f;
        public float maxDistance = 1f;
        public float dieForce = 5f;
        public float maxSightDistance = 25f;
        public float findWeaponSpeed = 4f;
        public float findTargetSpeed = 4f;
        public float patrolSpeed = 1f;

        [Header("Attack State")]
        public float attackStoppingDistance = 15f;
        public float attackSpeed = 3f;
        public float attackCloseRange = 7f;
    }
}