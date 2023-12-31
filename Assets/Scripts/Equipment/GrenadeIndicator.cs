using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class GrenadeIndicator : MonoBehaviour
    {
        [Header("Indicator")]
        private Transform indicator;
        private Transform distanceText;
        private TextMeshPro distanceShow;

        private void Start()
        {
            indicator = transform.Find("Indicator");
            indicator.gameObject.SetActive(true);
            distanceText = transform.Find("Indicator/Distance");
            distanceShow = distanceText.GetComponent<TextMeshPro>();
            distanceShow.gameObject.SetActive(true);
        }
        private void Update()
        {
            float distance = Vector3.Distance(Camera.main.transform.position, transform.position);
            distanceShow.text = Mathf.RoundToInt(distance) + "m";
        }
        private void FixedUpdate()
        {
            indicator.transform.LookAt(indicator.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
        }
    }
}