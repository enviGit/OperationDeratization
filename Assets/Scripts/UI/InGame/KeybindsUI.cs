using UnityEngine;
using TMPro;

public class KeybindsUI : MonoBehaviour
{
    private TextMeshProUGUI holdButtonText;
    private TextMeshProUGUI keybindsText;
    private bool isTabHeld = false;
    public TextMeshPro[] otherTexts;

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
        float targetAlpha = isTabHeld ? 0.1f : 1.0f;

        foreach (TextMeshPro otherText in otherTexts)
        {
            Color currentColor = otherText.color;
            currentColor.a = targetAlpha;
            otherText.color = currentColor;
        }
    }
}
