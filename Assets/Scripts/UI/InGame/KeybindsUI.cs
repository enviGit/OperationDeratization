using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class KeybindsUI : MonoBehaviour
    {
        private TextMeshProUGUI holdButtonText;
        private TextMeshProUGUI keybindsText;
        private bool isTabHeld = false;
        [SerializeField] private Material whiteTextMat;

        private void Start()
        {
            holdButtonText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            keybindsText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            keybindsText.gameObject.SetActive(false);
        }
        private void Update()
        {
            if (Input.GetKey(KeyCode.Tab))
            {
                if (!isTabHeld)
                {
                    isTabHeld = true;
                    ToggleKeybindUI(true);
                }
            }
            else
            {
                if (isTabHeld)
                {
                    isTabHeld = false;
                    ToggleKeybindUI(false);
                }
            }
        }
        private void ToggleKeybindUI(bool isTabHeld)
        {
            holdButtonText.gameObject.SetActive(!isTabHeld);
            keybindsText.gameObject.SetActive(isTabHeld);
            Color targetColor = isTabHeld ? Color.gray : Color.white;
            whiteTextMat.color = targetColor;
        }
    }
}