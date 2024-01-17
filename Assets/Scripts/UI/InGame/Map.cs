using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class Map : MonoBehaviour
    {
        public GameObject map;
        public GameObject playerUI;
        public GameObject tracker;
        public GameObject miniMap;
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
            map.gameObject.SetActive(isTabHeld);
            playerUI.gameObject.SetActive(!isTabHeld);
            tracker.gameObject.SetActive(!isTabHeld);
            miniMap.gameObject.SetActive(!isTabHeld);
        }
    }
}