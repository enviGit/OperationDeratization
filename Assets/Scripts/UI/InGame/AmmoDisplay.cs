using System.Text;
using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class AmmoDisplay : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        public Gun weapon;
        private StringBuilder ammoStringBuilder = new StringBuilder();

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }
        private void Update()
        {
            Check();
        }
        private void Check()
        {
            if (_text == null)
                return;
            if (weapon == null)
                _text.text = "";
            else
                UpdateAmmoText();
        }
        private void UpdateAmmoText()
        {
            ammoStringBuilder.Clear();  // Clear the StringBuilder before building the new text

            if (weapon.gunStyle == GunStyle.Primary || weapon.gunStyle == GunStyle.Secondary)
                ammoStringBuilder.Append(weapon.currentAmmoCount).Append(" / ").Append(weapon.maxAmmoCount);
            else
                ammoStringBuilder.Append(weapon.currentAmmoCount);

            _text.text = ammoStringBuilder.ToString();  // Update the TextMeshPro text
        }
    }
}