using UnityEngine;

[CreateAssetMenu()]
public class AiAgentConfig : ScriptableObject
{
    public float maxTime = 3f;
    public float maxDistance = 1f;
    public float dieForce = 5f;
    public float maxSightDistance = 10f;
    public float attackStoppingDistance = 5f;
    public float attackSpeed = 3f;
    public float findWeaponSpeed = 4f;
}
