using System.Collections;
using UnityEngine;

public class DoorOnButton : Interactable
{
    [Header("References")]
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;

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

    private void Awake()
    {
        originalLeftDoorScale = leftDoor.transform.localScale;
        originalRightDoorScale = rightDoor.transform.localScale;
    }
    private void Update()
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
    protected override void Interact()
    {
        prompt = "Interact with doors";

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
            else
            {
                SlideDoors(-doorSlideAmount);
                ScaleDoors(doorScaleAmount, false);
                doorsOpen = false;

                if (closeDoorCoroutine != null)
                    StopCoroutine(closeDoorCoroutine);
            }
        }
    }
    private void SlideDoors(float amount)
    {
        Vector3 leftDoorPos = leftDoor.transform.position;
        Vector3 rightDoorPos = rightDoor.transform.position;
        leftDoorPos.z += amount;
        rightDoorPos.z -= amount;
        StartCoroutine(LerpDoorPosition(leftDoor.transform, leftDoorPos, doorSlideTime));
        StartCoroutine(LerpDoorPosition(rightDoor.transform, rightDoorPos, doorSlideTime));
        doorsMoving = true;
    }
    private void ScaleDoors(float amount, bool opening)
    {
        if (doorScaleAmount == 1f) return;

        Vector3 leftDoorScale = leftDoor.transform.localScale;
        Vector3 rightDoorScale = rightDoor.transform.localScale;

        if (opening)
        {
            leftDoorScale.x *= amount;
            rightDoorScale.x *= amount;
        }
        else
        {
            leftDoorScale.x /= amount;
            rightDoorScale.x /= amount;
        }

        StartCoroutine(LerpDoorScale(leftDoor.transform, leftDoorScale, doorScaleTime));
        StartCoroutine(LerpDoorScale(rightDoor.transform, rightDoorScale, doorScaleTime));
    }
    IEnumerator LerpDoorPosition(Transform door, Vector3 targetPos, float slideTime)
    {
        Vector3 start = door.position;
        float elapsedTime = 0f;

        while (elapsedTime < slideTime)
        {
            door.position = Vector3.Lerp(start, targetPos, (elapsedTime / slideTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        door.position = targetPos;
        doorsMoving = false;
    }
    IEnumerator LerpDoorScale(Transform door, Vector3 targetScale, float scaleTime)
    {
        Vector3 start = door.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < scaleTime)
        {
            door.localScale = Vector3.Lerp(start, targetScale, (elapsedTime / scaleTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        door.localScale = targetScale;
    }
    IEnumerator CloseDoorAfterDelay()
    {
        yield return new WaitForSeconds(10f);

        if (doorsOpen)
        {
            SlideDoors(-doorSlideAmount);

            if(doorScaleAmount != 1f) ScaleDoors(-doorScaleAmount, false);

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