using System.Collections;
using UnityEngine;

public class DoorOnButton : Interactable
{
    [Header("References")]
    [SerializeField] private GameObject leftDoor1;
    [SerializeField] private GameObject rightDoor1;
    [SerializeField] private GameObject leftDoor2;
    [SerializeField] private GameObject rightDoor2;

    [Header("Door")]
    public float doorSlideAmount = 1.35f;
    public float doorSlideTime = 1f;
    public float doorScaleAmount = 0.5f;
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
        originalLeftDoorScale = leftDoor1.transform.localScale;
        originalRightDoorScale = rightDoor1.transform.localScale;
        originalLeftDoorScale = leftDoor2.transform.localScale;
        originalRightDoorScale = rightDoor2.transform.localScale;
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
        Vector3 leftDoorPos1 = leftDoor1.transform.position;
        Vector3 rightDoorPos1 = rightDoor1.transform.position;
        leftDoorPos1.z += amount;
        rightDoorPos1.z -= amount;
        StartCoroutine(LerpDoorPosition(leftDoor1.transform, leftDoorPos1, doorSlideTime));
        StartCoroutine(LerpDoorPosition(rightDoor1.transform, rightDoorPos1, doorSlideTime));
        Vector3 leftDoorPos2 = leftDoor2.transform.position;
        Vector3 rightDoorPos2 = rightDoor2.transform.position;
        leftDoorPos2.z += amount;
        rightDoorPos2.z -= amount;
        StartCoroutine(LerpDoorPosition(leftDoor2.transform, leftDoorPos2, doorSlideTime));
        StartCoroutine(LerpDoorPosition(rightDoor2.transform, rightDoorPos2, doorSlideTime));
        doorsMoving = true;
    }
    private void ScaleDoors(float amount, bool opening)
    {
        Vector3 leftDoorScale1 = leftDoor1.transform.localScale;
        Vector3 rightDoorScale1 = rightDoor1.transform.localScale;

        if (opening)
        {
            leftDoorScale1.x *= amount;
            rightDoorScale1.x *= amount;
        }
        else
        {
            leftDoorScale1.x /= amount;
            rightDoorScale1.x /= amount;
        }

        StartCoroutine(LerpDoorScale(leftDoor1.transform, leftDoorScale1, doorScaleTime));
        StartCoroutine(LerpDoorScale(rightDoor1.transform, rightDoorScale1, doorScaleTime));
        Vector3 leftDoorScale2 = leftDoor2.transform.localScale;
        Vector3 rightDoorScale2 = rightDoor2.transform.localScale;

        if (opening)
        {
            leftDoorScale2.x *= amount;
            rightDoorScale2.x *= amount;
        }
        else
        {
            leftDoorScale2.x /= amount;
            rightDoorScale2.x /= amount;
        }

        StartCoroutine(LerpDoorScale(leftDoor2.transform, leftDoorScale2, doorScaleTime));
        StartCoroutine(LerpDoorScale(rightDoor2.transform, rightDoorScale2, doorScaleTime));
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
