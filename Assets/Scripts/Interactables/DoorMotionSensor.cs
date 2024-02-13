using Cinemachine;
using RatGamesStudios.OperationDeratization.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class DoorMotionSensor : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject leftDoor;
        [SerializeField] private GameObject rightDoor;
        private GameObject player;
        private AudioSource doorSound;
        public AudioClip[] soundClips;
        public CinemachineVirtualCamera vCamera;
        private AudioEventManager audioEventManager;

        [Header("Door")]
        public float doorSlideAmount = 1.75f;
        public float doorSlideTime = 1f;
        public float doorScaleAmount = 1f;
        public float doorScaleTime = 0.5f;
        public float activationDistance = 3f;
        private Vector3 originalLeftDoorScale;
        private Vector3 originalRightDoorScale;

        [Header("Bool checks")]
        private static bool doorsOpen = false;
        private static bool doorsMoving = false;
        public bool isUsingCameraCheck = false;

        private void Start()
        {
            doorSound = GetComponent<AudioSource>();
            player = GameObject.FindGameObjectWithTag("Player");
            audioEventManager = GameObject.FindGameObjectWithTag("AudioEventManager").GetComponent<AudioEventManager>();
        }
        private void Awake()
        {
            originalLeftDoorScale = leftDoor.transform.localScale;
            originalRightDoorScale = rightDoor.transform.localScale;
        }
        private void Update()
        {
            List<float> distance = new List<float>();
            int count = 0;

            if (player != null)
            {
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                distance.Add(distanceToPlayer);
            }
            if (isUsingCameraCheck)
            {
                float distanceToCamera = Vector3.Distance(transform.position, vCamera.transform.position);
                distance.Add(distanceToCamera);
                CinemachineTrackedDolly dollyCart = vCamera.GetCinemachineComponent<CinemachineTrackedDolly>();

                if (dollyCart != null)
                {
                    float pathPosition = dollyCart.m_PathPosition;

                    if (pathPosition >= 3f && pathPosition <= 4f || pathPosition >= 4.1f && pathPosition <= 5f)
                        count++;
                }
            }

            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
            {
                float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
                distance.Add(distanceToEnemy);
            }
            foreach (float item in distance)
            {
                if (item <= activationDistance)
                    count++;
            }

            if (count != 0 && !doorsMoving && !doorsOpen)
                OpenDoors();
            if (count == 0 && !doorsMoving && doorsOpen)
                CloseDoors();
        }
        private void OpenDoors()
        {
            if (leftDoor != null || rightDoor != null)
            {
                doorSound.pitch = 1.2f;
                doorSound.PlayOneShot(soundClips[0]);
                audioEventManager.NotifyAudioEvent(doorSound);
                SlideDoors(doorSlideAmount);
                ScaleDoors(doorScaleAmount, true);
                doorsOpen = true;
            }
        }
        private void CloseDoors()
        {
            if (leftDoor != null || rightDoor != null)
            {
                doorSound.pitch = 2f;
                doorSound.PlayOneShot(soundClips[1]);
                audioEventManager.NotifyAudioEvent(doorSound);
                SlideDoors(-doorSlideAmount);
                ScaleDoors(-doorScaleAmount, false);
                doorsOpen = false;
            }
        }
        private void SlideDoors(float amount)
        {
            if (leftDoor != null)
            {
                Vector3 leftDoorPos = leftDoor.transform.position;
                leftDoorPos.z += amount;
                StartCoroutine(LerpDoorPosition(leftDoor.transform, leftDoorPos, doorSlideTime));
            }
            if (rightDoor != null)
            {
                Vector3 rightDoorPos = rightDoor.transform.position;
                rightDoorPos.z -= amount;
                StartCoroutine(LerpDoorPosition(rightDoor.transform, rightDoorPos, doorSlideTime));
            }
            if (leftDoor != null || rightDoor != null)
                doorsMoving = true;
        }
        private void ScaleDoors(float amount, bool opening)
        {
            if (doorScaleAmount == 1f)
                return;
            if (leftDoor != null)
            {
                Vector3 leftDoorScale = leftDoor.transform.localScale;

                if (opening)
                    leftDoorScale.x *= Mathf.Abs(amount);
                else
                    leftDoorScale.x /= Mathf.Abs(amount);

                StartCoroutine(LerpDoorScale(leftDoor.transform, leftDoorScale, doorScaleTime));
            }
            if (rightDoor != null)
            {
                Vector3 rightDoorScale = rightDoor.transform.localScale;

                if (opening)
                    rightDoorScale.x *= Mathf.Abs(amount);
                else
                    rightDoorScale.x /= Mathf.Abs(amount);

                StartCoroutine(LerpDoorScale(rightDoor.transform, rightDoorScale, doorScaleTime));
            }
        }
        private IEnumerator LerpDoorPosition(Transform door, Vector3 targetPos, float slideTime)
        {
            Vector3 start = door.position;
            float elapsedTime = 0f;

            while (elapsedTime < slideTime)
            {
                if (door == null)
                {
                    doorsMoving = false;

                    yield break;
                }

                door.position = Vector3.Lerp(start, targetPos, (elapsedTime / slideTime));
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            door.position = targetPos;
            doorsMoving = false;
        }
        private IEnumerator LerpDoorScale(Transform door, Vector3 targetScale, float scaleTime)
        {
            Vector3 start = door.localScale;
            float elapsedTime = 0f;

            while (elapsedTime < scaleTime)
            {
                if (door == null)
                {
                    doorsMoving = false;

                    yield break;
                }

                door.localScale = Vector3.Lerp(start, targetScale, (elapsedTime / scaleTime));
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            door.localScale = targetScale;
        }
    }
}