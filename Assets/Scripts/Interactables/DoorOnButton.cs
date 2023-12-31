using RatGamesStudios.OperationDeratization.Enemy;
using System.Collections;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class DoorOnButton : Interactable
    {
        [Header("References")]
        [SerializeField] private GameObject leftDoor;
        [SerializeField] private GameObject rightDoor;
        private AudioSource doorSound;
        public AudioClip[] soundClips;

        [Header("Door")]
        public float doorSlideAmount = 1.75f;
        public float doorSlideTime = 1f;
        public float doorScaleAmount = 1f;
        public float doorScaleTime = 0.5f;
        private Vector3 originalLeftDoorScale;
        private Vector3 originalRightDoorScale;
        private float enemyDetectionRadius = 2f;

        [Header("Bool checks")]
        public static bool doorsOpen = false;
        public static bool doorsMoving = false;
        private Coroutine closeDoorCoroutine;

        private void Start()
        {
            doorSound = transform.parent.GetComponent<AudioSource>();
        }
        private void Awake()
        {
            originalLeftDoorScale = leftDoor.transform.localScale;
            originalRightDoorScale = rightDoor.transform.localScale;
        }
        private void Update()
        {
            if (leftDoor != null || rightDoor != null)
            {
                if (DetectEnemyNearby())
                {
                    if (!doorsMoving)
                    {
                        if (!doorsOpen)
                        {
                            SlideDoors(doorSlideAmount);
                            ScaleDoors(doorScaleAmount, true);
                            doorsOpen = true;

                            if (closeDoorCoroutine != null)
                                StopCoroutine(closeDoorCoroutine);

                            closeDoorCoroutine = StartCoroutine(CloseDoorAfterDelay());
                        }
                    }
                }
            }
            else if (leftDoor == null && rightDoor == null)
                prompt = "";
        }
        protected override void Interact()
        {
            if (leftDoor != null || rightDoor != null)
            {
                prompt = "Interact with doors";

                if (!doorsMoving)
                {
                    if (!doorsOpen)
                    {
                        doorSound.pitch = 1.2f;
                        doorSound.PlayOneShot(soundClips[0]);
                        SlideDoors(doorSlideAmount);
                        ScaleDoors(doorScaleAmount, true);
                        doorsOpen = true;

                        if (closeDoorCoroutine != null)
                            StopCoroutine(closeDoorCoroutine);

                        closeDoorCoroutine = StartCoroutine(CloseDoorAfterDelay());
                    }
                    else
                    {
                        doorSound.pitch = 2f;
                        doorSound.PlayOneShot(soundClips[1]);
                        SlideDoors(-doorSlideAmount);
                        ScaleDoors(doorScaleAmount, false);
                        doorsOpen = false;

                        if (closeDoorCoroutine != null)
                            StopCoroutine(closeDoorCoroutine);
                    }
                }
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
        private IEnumerator CloseDoorAfterDelay()
        {
            yield return new WaitForSeconds(10f);

            if (doorsOpen)
            {
                doorSound.pitch = 2f;
                doorSound.PlayOneShot(soundClips[1]);
                SlideDoors(-doorSlideAmount);

                if (doorScaleAmount != 1f)
                    ScaleDoors(-doorScaleAmount, false);

                doorsOpen = false;
            }
        }
        private bool DetectEnemyNearby()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Enemy") && collider.GetComponent<EnemyHealth>().isAlive)
                    return true;
            }

            return false;
        }
    }
}