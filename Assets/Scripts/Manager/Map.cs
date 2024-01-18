using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Manager
{
    public class Map : MonoBehaviour
    {
        public GameObject miniMapCanvas;
        public GameObject mapCanvas;
        public GameObject playerUI;
        public GameObject tracker;
        private bool isKeyHeld = false;

        private void Update()
        {
            if (Input.GetKey(KeyCode.M))
            {
                if (!isKeyHeld)
                {
                    isKeyHeld = true;
                    ToggleMap(true);
                }
            }
            else
            {
                if (isKeyHeld)
                {
                    isKeyHeld = false;
                    ToggleMap(false);
                }
            }
        }
        private void ToggleMap(bool isTabHeld)
        {
            miniMapCanvas.gameObject.SetActive(!isTabHeld);
            mapCanvas.gameObject.SetActive(isTabHeld);
            playerUI.gameObject.SetActive(!isTabHeld);
            tracker.gameObject.SetActive(!isTabHeld);
        }
    }
}