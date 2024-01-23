using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI.InGame
{
    public class AmmoDisplay : MonoBehaviour
    {
        private TextMeshProUGUI _text;
        public Gun weapon;

        private void Start()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }
        private void Update()
        {
            if (_text == null)
                return;
            if (weapon == null)
                _text.text = "";
            else
            {
                if (weapon.gunStyle == GunStyle.Primary || weapon.gunStyle == GunStyle.Secondary)
                    _text.text = weapon.currentAmmoCount + " / " + weapon.maxAmmoCount;
                else
                    _text.text = weapon.currentAmmoCount.ToString();
            }
        }
    }
}