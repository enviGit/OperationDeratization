using UnityEngine;
using DG.Tweening;

public class ProjectileMovement : MonoBehaviour
{
    [HideInInspector] public Vector3 hitpoint;
    public float maxDistance = 20f;
    public float maxTimeNeededToReach = 2f;
    public AnimationCurve curve;

    private void Start()
    {
        Invoke("StartMovement", 0.02f);
    }

    private void StartMovement()
    {
        float distance = Vector3.Distance(transform.position, hitpoint);
        float normalizedDistance = Mathf.Clamp01(distance / maxDistance);
        float timeNeededToReach = Mathf.Lerp(0.1f, maxTimeNeededToReach, normalizedDistance);
        this.transform.DOMove(hitpoint, timeNeededToReach).SetEase(curve);
        Destroy(this.gameObject, timeNeededToReach + 0.1f);
    }
}
