using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class ResourceBar : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image frontBar;
        [SerializeField] private Image backBar;

        [Header("Settings")]
        [SerializeField] private float fillScale = 1f;
        [SerializeField] private float chipSpeed = 2f;

        private float lerpTimer;

        public void UpdateBar(float currentValue, float maxValue)
        {
            float fraction = Mathf.Clamp01(currentValue / maxValue);
            float targetFill = fraction * fillScale;

            if (frontBar.fillAmount != targetFill)
            {
                frontBar.fillAmount = targetFill;
            }

            if (backBar.fillAmount > targetFill)
            {
                lerpTimer += Time.deltaTime;
                float percentComplete = lerpTimer / chipSpeed;
                percentComplete = percentComplete * percentComplete;
                backBar.fillAmount = Mathf.Lerp(backBar.fillAmount, targetFill, percentComplete);
            }
            else if (backBar.fillAmount < targetFill)
            {
                backBar.fillAmount = targetFill;
                lerpTimer = 0f;
            }
            else
            {
                lerpTimer = 0f;
            }
        }

        public void SetBackColor(Color color)
        {
            if (backBar.color != color)
                backBar.color = color;
        }

        public void SetActive(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}