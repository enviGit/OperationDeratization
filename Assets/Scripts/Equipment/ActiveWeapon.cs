using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class ActiveWeapon : MonoBehaviour
    {
        [Header("Weapon Data")]
        [SerializeField] private Gun gunData;
        public Gun GunData => gunData;

        private int _currentClip;
        private int _currentStash;
        private bool _isInitialized = false;

        public int TotalAmmo => CurrentClip + CurrentStash;

        public int CurrentClip
        {
            get { if (!_isInitialized) InitializeAmmo(); return _currentClip; }
        }

        public int CurrentStash
        {
            get { if (!_isInitialized) InitializeAmmo(); return _currentStash; }
        }

        private void Start()
        {
            if (!_isInitialized) InitializeAmmo();
        }

        public void InitializeAmmo()
        {
            if (_isInitialized) return;
            if (gunData != null)
            {
                _currentClip = gunData.currentAmmoCount;
                _currentStash = gunData.maxAmmoCount;
                _isInitialized = true;
            }
        }

        public void InitializeAsSinglePickup()
        {
            if (gunData != null)
            {
                _currentClip = 1;
                _currentStash = 0;
                _isInitialized = true;
            }
        }

        public bool TryShoot()
        {
            if (_currentClip > 0) { _currentClip--; return true; }
            if (_currentStash > 0) { _currentStash--; return true; }
            return false;
        }

        public void Reload()
        {
            if (gunData == null || _currentClip >= gunData.magazineSize || _currentStash <= 0) return;
            int ammoToLoad = Mathf.Min(gunData.magazineSize, _currentStash);
            _currentStash -= ammoToLoad;
            _currentClip = ammoToLoad;
        }

        public void RefillAmmo(int amount)
        {
            if (gunData == null) return;
            _currentClip = gunData.magazineSize;
            _currentStash = gunData.maxAmmoCount;
        }

        public bool AddAmmo(int amount)
        {
            int maxTotal = gunData.maxAmmoCount > 0 ? gunData.maxAmmoCount : 3;

            if (TotalAmmo >= maxTotal) return false;

            _currentStash = Mathf.Min(_currentStash + amount, maxTotal);
            return true;
        }

        public bool IsFull()
        {
            if (gunData == null) return true;
            int maxTotal = gunData.maxAmmoCount > 0 ? gunData.maxAmmoCount : 3;
            return TotalAmmo >= maxTotal;
        }
    }
}