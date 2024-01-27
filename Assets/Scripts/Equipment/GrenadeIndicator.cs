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
        private Camera cam;

        private void Start()
        {
            indicator = transform.Find("Indicator");
            indicator.gameObject.SetActive(true);
            distanceText = transform.Find("Indicator/Distance");
            distanceShow = distanceText.GetComponent<TextMeshPro>();
            distanceShow.gameObject.SetActive(true);
            cam = Camera.main;
        }
        private void Update()
        {
            if(cam != null)
            {
                float distance = Vector3.Distance(cam.transform.position, transform.position);
                distanceShow.text = Mathf.RoundToInt(distance) + "m";
            }
        }
        private void FixedUpdate()
        {
            if (cam != null)
                indicator.transform.LookAt(indicator.transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
        }
    }
}