using System.Text;
using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI
{
    public class AmmoDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private bool isAlwaysUpdated = true;

        private Gun currentWeapon;
        private StringBuilder ammoStringBuilder = new StringBuilder();

        private void Start()
        {
            if (_text == null) _text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (isAlwaysUpdated && currentWeapon != null)
            {
                UpdateAmmoText();
            }
        }

        public void SetWeapon(Gun weapon)
        {
            currentWeapon = weapon;
            UpdateAmmoText();
        }

        private void UpdateAmmoText()
        {
            if (_text == null) return;

            ammoStringBuilder.Clear();

            if (currentWeapon == null || currentWeapon.gunStyle == GunStyle.Melee)
            {
                _text.text = "";
                return;
            }

            if (currentWeapon.gunStyle == GunStyle.Primary || currentWeapon.gunStyle == GunStyle.Secondary)
                ammoStringBuilder.Append(currentWeapon.currentAmmoCount).Append(" / ").Append(currentWeapon.maxAmmoCount);
            else
                ammoStringBuilder.Append(currentWeapon.currentAmmoCount);

            _text.text = ammoStringBuilder.ToString();
        }
    }
}