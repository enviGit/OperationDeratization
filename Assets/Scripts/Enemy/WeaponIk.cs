using System;
using UnityEngine;

[System.Serializable]
public class HumanBone
{
    public HumanBodyBones bone;
    public float weight = 1f;
}
public class WeaponIk : MonoBehaviour
{
    private Transform targetTransform;
    private Transform aimTransform;
    public Vector3 targetOffset = new Vector3(0, 0.65f, 0);
    public int iterations = 10;
    [Range(0, 1)] public float weight = 1f;
    public float angleLimit = 90f;
    public float distanceLimit = 1.5f;
    public HumanBone[] humanBones;
    Transform[] boneTransforms;

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        boneTransforms = new Transform[humanBones.Length];

        for (int i = 0; i < boneTransforms.Length; i++)
            boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
    }
    private void LateUpdate()
    {
        if (aimTransform == null)
            return;
        if (targetTransform == null)
            return;

        Vector3 targetPosition = GetTargetPosition();

        for(int i = 0; i < iterations; i++)
            for(int j = 0; j < boneTransforms.Length; j++)
            {
                Transform bone = boneTransforms[j];
                float boneWeight = humanBones[j].weight * weight;
                AimAtTarget(bone, targetPosition, boneWeight);
            }
    }
    private void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
    {
        Vector3 aimDirection = aimTransform.forward;
        Vector3 targetDirection = targetPosition - aimTransform.position;
        Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);
        Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
        bone.rotation = blendedRotation * bone.rotation;
    }
    private Vector3 GetTargetPosition()
    {
        Vector3 targetDirection = (targetTransform.position + targetOffset) - aimTransform.position;
        Vector3 aimDirection = aimTransform.forward;
        float blendOut = 0;
        float targetAngle = Vector3.Angle(targetDirection, aimDirection);

        if (targetAngle > angleLimit)
            blendOut += (targetAngle - angleLimit) / 50f;

        float targetDistance = targetDirection.magnitude;

        if (targetDistance < distanceLimit)
            blendOut += distanceLimit - targetDistance;

        Vector3 direction = Vector3.Slerp(targetDirection, aimDirection, blendOut);
        return aimTransform.position + direction;
    }
    public void SetTargetTransform(Transform target)
    {
        targetTransform = target;
    }
    public void SetAimTransform(Transform aim)
    {
        aimTransform = aim;
    }
}
