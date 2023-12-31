using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class Smoke : MonoBehaviour
    {
        [Header("References")]
        public GameObject smokeEffect;
        private AudioSource bang;
        private GameObject parentObject;

        [Header("Grenade")]
        public float delay = 2f;
        public bool shouldSmoke = false;
        private bool hasSmoked = false;
        private float countdown;

        private void Start()
        {
            parentObject = GameObject.Find("3D");
            countdown = delay;
            bang = GameObject.FindGameObjectWithTag("Bang").GetComponent<AudioSource>();
        }
        private void Update()
        {
            if (shouldSmoke)
            {
                countdown -= Time.deltaTime;

                if (countdown <= 0 && !hasSmoked)
                {
                    SmokeOn();
                    hasSmoked = true;
                }
            }
        }
        private void SmokeOn()
        {
            bang.PlayOneShot(bang.GetComponent<ProjectileSound>().audioClips[2]);
            Instantiate(smokeEffect, transform.position, transform.rotation, parentObject.transform);
            Destroy(gameObject);
        }
    }
}