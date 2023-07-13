using UnityEngine;

public class Ragdoll : MonoBehaviour
{
    Rigidbody[] rigidBodies;
    Animator animator;

    private void Start()
    {
        rigidBodies = GetComponentsInChildren<Rigidbody>();
        animator = GetComponent<Animator>();
        DeactivateRagdoll();
    }
    public void DeactivateRagdoll()
    {
        foreach(var rigidBody in rigidBodies)
        {
            rigidBody.isKinematic = true;
            rigidBody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        }

        if(animator != null)
            animator.enabled = true;
    }
    public void ActivateRagdoll()
    {
        foreach (var rigidBody in rigidBodies)
            rigidBody.isKinematic = false;

        if (animator != null)
            animator.enabled = false;
    }
    public void ApplyForce(Vector3 force)
    {
        var rigidBody = animator.GetBoneTransform(HumanBodyBones.Hips).GetComponent<Rigidbody>();
        rigidBody.AddForce(force, ForceMode.VelocityChange);
    }
}
