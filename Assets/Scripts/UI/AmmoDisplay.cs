using RatGamesStudios.OperationDeratization.Equipment;
using System.Text;
using TMPro;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.UI
{
    public class AmmoDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private bool isAlwaysUpdated = true;
        private Gun currentGunAsset;
        private ActiveWeapon currentActiveWeapon;
        private StringBuilder ammoStringBuilder = new StringBuilder();

        private void Start()
        {
            if (_text == null) _text = GetComponent<TextMeshProUGUI>();
        }

        private void Update()
        {
            if (isAlwaysUpdated && currentGunAsset != null)
            {
                UpdateAmmoText();
            }
        }

        public void SetWeapon(Gun gun, ActiveWeapon activeLogic)
        {
            currentGunAsset = gun;
            currentActiveWeapon = activeLogic;
            UpdateAmmoText();
        }

        private void UpdateAmmoText()
        {
            if (_text == null) return;

            ammoStringBuilder.Clear();

            if (currentGunAsset == null || currentGunAsset.gunStyle == GunStyle.Melee)
            {
                _text.text = "";
                return;
            }

            if (currentActiveWeapon != null)
            {
                if (IsGrenade(currentGunAsset.gunStyle))
                {
                    ammoStringBuilder.Append(currentActiveWeapon.TotalAmmo);
                }
                else
                {
                    ammoStringBuilder.Append(currentActiveWeapon.CurrentClip)
                                     .Append(" / ")
                                     .Append(currentActiveWeapon.CurrentStash);
                }
            }
            else
            {
                ammoStringBuilder.Append("--");
            }

            _text.text = ammoStringBuilder.ToString();
        }

        private bool IsGrenade(GunStyle style)
        {
            return style == GunStyle.Grenade ||
                   style == GunStyle.Flashbang ||
                   style == GunStyle.Smoke ||
                   style == GunStyle.Molotov;
        }
    }
}