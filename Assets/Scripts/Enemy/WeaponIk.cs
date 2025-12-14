using System;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    [Serializable]
    public class HumanBone
    {
        public HumanBodyBones bone;
        public float weight = 1f;
    }
    public class WeaponIk : MonoBehaviour
    {
        private Transform targetTransform;
        private Transform aimTransform;
        public int iterations = 20;
        [Range(0, 1)] public float weight = 1f;
        public float angleLimit = 90f;
        public float distanceLimit = 1.5f;
        public HumanBone[] humanBones;
        private Transform[] boneTransforms;

        [Header("IK Smoothing")]
        public float slerpSpeed = 8f;

        private void Start()
        {
            Animator animator = GetComponent<Animator>();
            boneTransforms = new Transform[humanBones.Length];

            for (int i = 0; i < boneTransforms.Length; i++)
                boneTransforms[i] = animator.GetBoneTransform(humanBones[i].bone);
        }
        private void LateUpdate()
        {
            if (aimTransform == null || targetTransform == null)
                return;

            Vector3 targetPosition = targetTransform.position;
            Vector3 aimOrigin = aimTransform.position;
            Vector3 targetDirection = targetPosition - aimOrigin;
            Vector3 aimDirection = aimTransform.forward;

            float targetAngle = Vector3.Angle(targetDirection, aimDirection);
            float targetDistance = targetDirection.magnitude;

            float targetWeight = weight;
            if (targetDistance < distanceLimit)
            {
                targetWeight = Mathf.Lerp(targetWeight, 0.5f, (distanceLimit - targetDistance) / distanceLimit);
            }

            if (targetAngle > angleLimit)
            {
                targetWeight = Mathf.Lerp(weight, 0f, (targetAngle - angleLimit) / 45f);
            }

            targetWeight = Mathf.Clamp01(targetWeight);

            for (int i = 0; i < iterations; i++)
                for (int j = 0; j < boneTransforms.Length; j++)
                {
                    Transform bone = boneTransforms[j];
                    float boneWeight = humanBones[j].weight * targetWeight;
                    AimAtTarget(bone, targetPosition, boneWeight);
                }
        }
        private void AimAtTarget(Transform bone, Vector3 targetPosition, float weight)
        {
            Vector3 aimDirection = aimTransform.forward;
            Vector3 targetDirection = targetPosition - aimTransform.position;

            Quaternion aimTowards = Quaternion.FromToRotation(aimDirection, targetDirection);

            Quaternion blendedRotation = Quaternion.Slerp(Quaternion.identity, aimTowards, weight);
            Quaternion desiredRotation = blendedRotation * bone.rotation;
            bone.rotation = Quaternion.Slerp(bone.rotation, desiredRotation, Time.deltaTime * slerpSpeed);
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
}