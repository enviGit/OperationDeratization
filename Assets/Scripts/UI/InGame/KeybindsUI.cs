using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class KeybindsUI : MonoBehaviour
    {
        private TextMeshProUGUI holdButtonText;
        private TextMeshProUGUI keybindsText;
        private bool isF1Held = false;
        [SerializeField] private Material whiteTextMat;

        private void Start()
        {
            holdButtonText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            keybindsText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            keybindsText.gameObject.SetActive(false);
        }
        private void Update()
        {
            if (Input.GetKey(KeyCode.F1))
            {
                if (!isF1Held)
                {
                    isF1Held = true;
                    ToggleKeybindUI(true);
                }
            }
            else
            {
                if (isF1Held)
                {
                    isF1Held = false;
                    ToggleKeybindUI(false);
                }
            }
        }
        private void ToggleKeybindUI(bool isF1Held)
        {
            holdButtonText.gameObject.SetActive(!isF1Held);
            keybindsText.gameObject.SetActive(isF1Held);
            Color targetColor = isF1Held ? Color.gray : Color.white;
            whiteTextMat.color = targetColor;
        }
    }
}