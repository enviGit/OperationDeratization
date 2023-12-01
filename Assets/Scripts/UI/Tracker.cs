using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Tracker : MonoBehaviour
{
    [SerializeField] private Image cooldownFillImage;
    [SerializeField] private TextMeshProUGUI trackerCooldownText;
    public float trackingCooldown = 61f;
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
        //indicator.rotation = Quaternion.Slerp(indicator.rotation, Quaternion.LookRotation(nearestOpponent.transform.position - player.position) * Quaternion.Euler(-15, 1, 60), rotationSpeed * Time.deltaTime);
    }
    private void StartTracking()
    {
        isTracking = true;
        indicator.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(DelayedTrackingRoutine());
        Invoke("StopTracking", trackingDuration);
        StartCoroutine(StartCooldownRoutine());
    }
    private void StopTracking()
    {
        isTracking = false;
        nearestOpponent = null;
        indicator.GetChild(0).gameObject.SetActive(false);
    }
    private IEnumerator DelayedTrackingRoutine()
    {
        yield return new WaitForSeconds(trackingDuration);

        StartCoroutine(UpdateTrackingRoutine());
    }
    private IEnumerator UpdateTrackingRoutine()
    {
        while (isTracking)
        {
            UpdateTracking();

            yield return new WaitForSeconds(trackingCooldown);
        }
    }
    private IEnumerator StartCooldownRoutine()
    {
        isOnCooldown = true;
        currentCooldownTime = trackingCooldown;

        yield return new WaitForSeconds(trackingDuration);

        while (currentCooldownTime > 0f)
        {
            yield return new WaitForSeconds(1f);

            currentCooldownTime--;
            UpdateCooldownFillAmount();
        }

        isOnCooldown = false;
        UpdateCooldownFillAmount();
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
    private void UpdateCooldownFillAmount()
    {
        cooldownFillImage.fillAmount = 1 - (currentCooldownTime / trackingCooldown);
        trackerCooldownText.text = Mathf.CeilToInt(currentCooldownTime).ToString() != "0" ? Mathf.CeilToInt(currentCooldownTime).ToString() : "";
    }
    public void MarkOpponentAsDead(GameObject opponent)
    {
        if (opponent == nearestOpponent)
        {
            nearestOpponent = null;

            if (isTracking)
                StartCoroutine(UpdateTrackingRoutine());
        }
    }
}