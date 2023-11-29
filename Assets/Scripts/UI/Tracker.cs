using UnityEngine;
using System.Collections;
using TMPro;

public class Tracker : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI trackerCooldownText;
    public float trackingCooldown = 65f;
    public float trackingDuration = 5f;
    public Transform indicator;
    public Transform player;
    private GameObject nearestOpponent;
    private bool isTracking = false;
    private bool isOnCooldown = false;
    private Quaternion targetRotation;
    private float rotationSpeed = 360f;
    private float currentCooldownTime;

    private void Start()
    {
        indicator.GetChild(0).gameObject.SetActive(false);
        currentCooldownTime = trackingCooldown;
        UpdateTrackerCooldownText();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (!isTracking && !isOnCooldown)
                StartTracking();
        }

        UpdateTracking();

        if (nearestOpponent != null)
        {
            Vector3 direction = nearestOpponent.transform.position - player.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            float angle = Vector3.SignedAngle(player.forward, direction, player.up);
            Vector3 eulerRotation = rotation.eulerAngles;

            if (Mathf.Abs(angle) < 90f)
                eulerRotation.z = angle;
            else if (angle >= 90f)
                eulerRotation.z = 90f;
            else if (angle <= -90f)
                eulerRotation.z = -90f;

            targetRotation = Quaternion.Euler(eulerRotation);
        }
        else
            indicator.GetChild(0).gameObject.SetActive(false);

        RotateIndicator();
    }
    private void RotateIndicator()
    {
        indicator.rotation = Quaternion.RotateTowards(indicator.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
    private void StartTracking()
    {
        isTracking = true;

        if (isTracking)
            trackerCooldownText.text = "Tracking...";

        StartCoroutine(UpdateTrackingRoutine());
        Invoke("StopTracking", trackingDuration);
        StartCoroutine(StartCooldownRoutine());
    }
    private void StopTracking()
    {
        isTracking = false;
        nearestOpponent = null;
        indicator.GetChild(0).gameObject.SetActive(false);
        UpdateTrackerCooldownText();
    }
    private void UpdateTracking()
    {
        GameObject[] opponents = GameObject.FindGameObjectsWithTag("Enemy");
        float nearestDistance = Mathf.Infinity;
        GameObject newNearestOpponent = null;

        foreach (GameObject opponent in opponents)
        {
            if (opponent == null || opponent.GetComponent<EnemyHealth>().isMarkedAsDead == true)
                continue;

            float distance = Vector3.Distance(player.position, opponent.transform.position);

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                newNearestOpponent = opponent;
            }
        }

        if (newNearestOpponent != null && nearestOpponent != newNearestOpponent)
            nearestOpponent = newNearestOpponent;
        else if (newNearestOpponent == null && nearestOpponent != null)
            nearestOpponent = null;
    }
    private IEnumerator UpdateTrackingRoutine()
    {
        while (isTracking)
        {
            indicator.GetChild(0).gameObject.SetActive(true);
            UpdateTracking();
            yield return new WaitForSeconds(trackingCooldown);
        }
    }
    private IEnumerator StartCooldownRoutine()
    {
        isOnCooldown = true;
        currentCooldownTime = trackingCooldown;

        while (currentCooldownTime > 0f)
        {
            yield return new WaitForSeconds(1f);
            currentCooldownTime--;
            UpdateTrackerCooldownText();
        }

        isOnCooldown = false;
        UpdateTrackerCooldownText();
    }
    private IEnumerator UpdateTrackingDelayRoutine()
    {
        yield return new WaitForSeconds(trackingCooldown);
        UpdateTracking();
    }
    public void MarkOpponentAsDead(GameObject opponent)
    {
        if (opponent == nearestOpponent)
        {
            nearestOpponent = null;

            if (isTracking)
                StartCoroutine(UpdateTrackingDelayRoutine());
        }
    }
    private void UpdateTrackerCooldownText()
    {
        if (isTracking)
            trackerCooldownText.text = "Tracking...";
        else if (isOnCooldown)
            trackerCooldownText.text = Mathf.CeilToInt(currentCooldownTime - 1).ToString() + " s";
        else
            trackerCooldownText.text = "Ready";
    }
}
