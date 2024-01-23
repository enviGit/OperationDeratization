using RatGamesStudios.OperationDeratization.Player;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class WeaponWheel : MonoBehaviour
    {
        [SerializeField] private KeyCode wheelKey = KeyCode.Tab;
        [SerializeField] private GameObject wheelParent;
        [SerializeField] private GameObject blur;
        [SerializeField] private Tracker tracker;
        private GameObject player;
        private PlayerInventory inventory;
        private PlayerHealth health;
        private bool m_WheelEnabled;
        private Camera playerCamera;
        [SerializeField] private float targetTimeScale = 0.1f, timeToGoToTargetTimeScale = 0.01f;
        private float m_TimeV;
        private int selectedIndex = -1;
        [SerializeField] private GameObject wheelAmmoCount;
        [SerializeField] private TextMeshProUGUI nameToDisplay;

        [Serializable]
        public class Wheel
        {
            public Sprite highlightSprite;
            private Sprite m_NormalSprite;
            public Image wheel;

            public Sprite NormalSprite
            {
                get => m_NormalSprite;
                set => m_NormalSprite = value;
            }
        }
        [SerializeField] private Wheel[] wheels = new Wheel[6];

        [Header("Dots & Lines")]
        [SerializeField] private Transform[] dots = new Transform[9];

        [Header("AmmoCount")]
        [SerializeField] private AmmoDisplay[] ammoText = new AmmoDisplay[6];

        [Header("WheelIcons")]
        [SerializeField] private Image[] icons = new Image[6];

        [Header("MenuCanvases")]
        [SerializeField] private GameObject[] pause_options_victory = new GameObject[3];

        private Vector2[] pos = new Vector2[9];
        private Vector3 start, end;
        private Vector2 mousePos;
        public bool WheelEnabled => m_WheelEnabled;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            for (int i = 0; i < pos.Length - 1; ++i)
                Gizmos.DrawLine(dots[i].position, dots[i + 1].position);

            Gizmos.DrawLine(dots[pos.Length - 1].position, dots[0].position);
            Vector3 center = transform.position;

            for (int i = 0; i < pos.Length; ++i)
                Gizmos.DrawLine(center, dots[i].position);
        }
        private void OnDrawGizmosSelected()
        {
            start.x = pos[0].x;
            start.y = pos[0].y;
            start.z = dots[0].position.z;

            for (int i = 0; i < pos.Length; ++i)
            {
                end.x = pos[i].x;
                end.y = pos[i].y;
                end.z = dots[i].position.z;
                Debug.DrawLine(start, end, Color.red);
            }
            for (int i = 0; i < pos.Length - 1; ++i)
            {
                start.x = pos[i].x;
                start.y = pos[i].y;
                start.z = dots[i].position.z;
                end.x = pos[i + 1].x;
                end.y = pos[i + 1].y;
                end.z = dots[i + 1].position.z;
                Debug.DrawLine(start, end, Color.red);
            }

            //For the Last Triangle
            start.x = pos[8].x;
            start.y = pos[8].y;
            start.z = dots[8].position.z;
            end.x = pos[1].x;
            end.y = pos[1].y;
            end.z = dots[1].position.z;
            Debug.DrawLine(start, end, Color.red);

        }
        private float Area(Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return Mathf.Abs((v1.x * (v2.y - v3.y) + v2.x * (v3.y - v1.y) + v3.x * (v1.y - v2.y)) / 2f);
        }
        private bool IsInside(Vector2 v1, Vector2 v2, Vector2 v3, Vector2 v)
        {
            float A = Area(v1, v2, v3);
            float A1 = Area(v1, v2, v);
            float A2 = Area(v1, v, v3);
            float A3 = Area(v, v2, v3);

            return (Mathf.Abs(A1 + A2 + A3 - A) < 1f);
        }
        public void EnableHighlight(int index)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                if (wheels[i].wheel != null && wheels[i].highlightSprite != null)
                {
                    if (i == index)
                        wheels[i].wheel.sprite = wheels[i].highlightSprite;
                    else
                        wheels[i].wheel.sprite = wheels[i].NormalSprite;
                }
            }
        }
        public void DisableAllHighlight()
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                if (wheels[i].wheel != null)
                    wheels[i].wheel.sprite = wheels[i].NormalSprite;
            }
        }
        private void CheckForCurrentWeapon()
        {
            if (playerCamera == null)
                return;

            for (int i = 0; i < pos.Length; i++)
            {
                //Changing World coordinates to screen coordinates
                pos[i] = playerCamera.WorldToScreenPoint(dots[i].position);
            }

            mousePos = Input.mousePosition;

            if (IsInside(pos[0], pos[1], pos[2], mousePos))
            {
                selectedIndex = 0;
                nameToDisplay.text = "Tracker";
            }
            else if (IsInside(pos[0], pos[2], pos[3], mousePos))
            {
                selectedIndex = 1;
                nameToDisplay.text = inventory.weapons[0].gunName;
            }
            else if (IsInside(pos[0], pos[3], pos[4], mousePos))
            {
                selectedIndex = 2;

                if (inventory.weapons[1] != null)
                    nameToDisplay.text = inventory.weapons[1].gunName;
                else
                    nameToDisplay.text = "";
            }
            else if (IsInside(pos[0], pos[4], pos[5], mousePos))
            {
                selectedIndex = 3;

                if (inventory.weapons[2] != null)
                    nameToDisplay.text = inventory.weapons[2].gunName;
                else
                    nameToDisplay.text = "";
            }
            else if (IsInside(pos[0], pos[5], pos[6], mousePos))
            {
                selectedIndex = 4;

                if (inventory.weapons[3] != null)
                    nameToDisplay.text = inventory.weapons[3].gunName;
                else
                    nameToDisplay.text = "";
            }
            else if (IsInside(pos[0], pos[6], pos[7], mousePos))
            {
                selectedIndex = 5;

                if (inventory.weapons[4] != null)
                    nameToDisplay.text = inventory.weapons[4].gunName;
                else
                    nameToDisplay.text = "";
            }
            else if (IsInside(pos[0], pos[7], pos[8], mousePos))
            {
                selectedIndex = 6;

                if (inventory.weapons[5] != null)
                    nameToDisplay.text = inventory.weapons[5].gunName;
                else
                    nameToDisplay.text = "";
            }
            else if (IsInside(pos[0], pos[8], pos[1], mousePos))
            {
                selectedIndex = 7;

                if (inventory.weapons[6] != null)
                    nameToDisplay.text = inventory.weapons[6].gunName;
                else
                    nameToDisplay.text = "";
            }

            EnableHighlight(selectedIndex);
        }
        private void Start()
        {
            DisableWheel();
            player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<PlayerInventory>();
            health = player.GetComponent<PlayerHealth>();
            playerCamera = Camera.main;

            for (int i = 0; i < wheels.Length; i++)
            {
                if (wheels[i].wheel != null)
                    wheels[i].NormalSprite = wheels[i].wheel.sprite;
            }
            for (int i = 0; i < dots.Length; i++)
            {
                pos[i].x = dots[i].position.x;
                pos[i].y = dots[i].position.y;
            }
        }
        private void EnableWheel()
        {
            wheelParent.SetActive(true);
            wheelAmmoCount.SetActive(true);
            m_WheelEnabled = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            for(int i = 0; i < icons.Length; i++)
            {
                if (inventory.weapons[i + 1] != null)
                {
                    icons[i].enabled = true;
                    icons[i].sprite = inventory.weapons[i + 1].activeGunIcon;
                }
                else
                    icons[i].enabled = false;
            }
            for (int i = 0; i < 2; i++)
                ammoText[i].weapon = inventory.weapons[i + 1];
            for (int i = 2; i < ammoText.Length; i++)
                ammoText[i].weapon = inventory.weapons[i + 1];

            if (blur != null)
                blur.SetActive(true);
        }
        private void DisableWheel()
        {
            wheelParent.SetActive(false);
            wheelAmmoCount.SetActive(false);
            m_WheelEnabled = false;

            if (blur != null)
                blur.SetActive(false);
        }
        private void Update()
        {
            bool isCoverActive = false;

            foreach (GameObject coverCanvas in pause_options_victory)
            {
                if (coverCanvas.activeSelf)
                {
                    isCoverActive = true;
                    break;
                }
            }

            if (!isCoverActive && health.isAlive)
            {
                // Check for wheel activation
                if (Input.GetKey(wheelKey))
                {
                    EnableWheel();
                    CheckForCurrentWeapon();
                }
                else if (Input.GetKeyUp(wheelKey))
                {
                    DisableWheel();
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;

                    if (selectedIndex == 0)
                    {
                        if (!tracker.isTracking && !tracker.isOnCooldown && tracker.opponents.Count > 0)
                        {
                            tracker.StartTracking();
                            StartCoroutine(tracker.SceneScanning());
                        }
                    }
                    else
                    {
                        if (inventory.weapons[selectedIndex - 1] != null) // IndexOutOfRangeException: Index was outside the bounds of the array.
                            inventory.SetCurrentWeapon(selectedIndex - 1);
                    }

                    selectedIndex = -1;
                }
            }
            else
            {
                // If cover is active, close the wheel
                DisableWheel();
                selectedIndex = -1;
            }
            if (m_WheelEnabled)
                Time.timeScale = Mathf.SmoothDamp(Time.timeScale, targetTimeScale, ref m_TimeV, timeToGoToTargetTimeScale);
            else
                Time.timeScale = Mathf.SmoothDamp(Time.timeScale, 1f, ref m_TimeV, timeToGoToTargetTimeScale / 10f);
        }
    }
}