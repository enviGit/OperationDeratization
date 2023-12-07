using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMotionSensor : Interactable
{
    [Header("References")]
    [SerializeField] private GameObject leftDoor;
    [SerializeField] private GameObject rightDoor;
    private CharacterController playerController;
    private AudioSource doorSound;
    public AudioClip[] soundClips;

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

    private void Start()
    {
        doorSound = GetComponent<AudioSource>();
        playerController = FindObjectOfType<CharacterController>();
    }
    private void Awake()
    {
        originalLeftDoorScale = leftDoor.transform.localScale;
        originalRightDoorScale = rightDoor.transform.localScale;
    }
    private void Update()
    {
        List<float> distance = new List<float>();
        float distanceToPlayer = Vector3.Distance(transform.position, playerController.transform.position);
        distance.Add(distanceToPlayer);
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        int count = 0;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);
            distance.Add(distanceToEnemy);
        }
        foreach(float item in distance)
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
        doorSound.pitch = 1.2f;
        doorSound.PlayOneShot(soundClips[0]);
        SlideDoors(doorSlideAmount);
        ScaleDoors(doorScaleAmount, true);
        doorsOpen = true;
    }
    private void CloseDoors()
    {
        doorSound.pitch = 2f;
        doorSound.PlayOneShot(soundClips[1]);
        SlideDoors(-doorSlideAmount);
        ScaleDoors(-doorScaleAmount, false);
        doorsOpen = false;
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
}
