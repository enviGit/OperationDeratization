using UnityEngine;

[CreateAssetMenu()]
public class AiAgentConfig : ScriptableObject
{
    public float maxTime = 3f;
    public float maxDistance = 1f;
    public float dieForce = 5f;
    public float maxSightDistance = 5f;
}
