using RatGamesStudios.OperationDeratization.Enemy;
using RatGamesStudios.OperationDeratization.UI.Menu;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class Tracker : MonoBehaviour
    {
        [SerializeField] private Image cooldownFillImage;
        [SerializeField] private TextMeshProUGUI trackerCooldownText;
        [SerializeField] private PauseMenu playerUI;
        public float trackingCooldown = 31f;
        public float trackingDuration = 5f;
        public Transform indicator;
        private Transform player;
        private List<GameObject> opponents = new List<GameObject>();
        private GameObject nearestOpponent;
        private bool isTracking = false;
        private bool isOnCooldown = false;
        private Quaternion targetRotation;
        private float rotationSpeed = 360f;
        private float currentCooldownTime;
        public Material terrainScanMat;
        private AudioSource trackerSound;
        private Transform cam;

        private void Start()
        {
            indicator.GetChild(0).gameObject.SetActive(false);
            currentCooldownTime = trackingCooldown;
            trackerSound = GetComponent<AudioSource>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
            opponents.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));
            cam = Camera.main.transform;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                if (!isTracking && !isOnCooldown && opponents.Count > 0)
                {
                    StartTracking();
                    StartCoroutine(SceneScanning());
                    trackerSound.Play();
                }
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
                opponents.Remove(opponent);

                if (isTracking)
                    StartCoroutine(UpdateTrackingRoutine());
            }

            StartCoroutine(DelayedCheckForVictory());
        }
        private IEnumerator DelayedCheckForVictory()
        {
            yield return new WaitForSeconds(5f);

            CheckForVictory();
        }
        private void CheckForVictory()
        {
            if (opponents.Count == 0)
                playerUI.victoryScreen.SetActive(true);
        }
        public IEnumerator SceneScanning()
        {
            int numScans = 4;
            float timeBetweenScans = 0.5f;

            for (int i = 0; i < numScans; i++)
            {
                float timer = 0f;
                float scanRange = 0f;
                float startOpacity = 1f;
                float endOpacity = 0f;
                terrainScanMat.SetVector("_Position", player.position);

                while (timer <= 1f)
                {
                    timer += Time.deltaTime;
                    scanRange = Mathf.Lerp(0f, 100f, timer);
                    float interpolatedOpacity = Mathf.Lerp(startOpacity, endOpacity, timer);
                    terrainScanMat.SetFloat("_Range", scanRange);
                    terrainScanMat.SetFloat("_Opacity", interpolatedOpacity);

                    yield return null;
                }

                terrainScanMat.SetFloat("_Opacity", 0f);

                yield return new WaitForSeconds(timeBetweenScans);
            }
        }
    }
}