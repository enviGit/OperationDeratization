using RatGamesStudios.OperationDeratization.Interactables;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RatGamesStudios.OperationDeratization.Equipment
{
    public class Flashbang : MonoBehaviour
    {
        [Header("Flashbang")]
        private Image whiteImage;
        private AudioSource bang;
        public float delay = 2f;
        public float distance = 11.5f;
        public bool shouldFlash = false;
        private bool hasFlashed = false;
        private float countdown;
        private float minimalCollisionForceToBreakGlass = 10f;
        private float impactForce = 100f;
        private GameObject mesh;
        private GameObject indicator;

        private void Start()
        {
            countdown = delay;
            whiteImage = GameObject.FindGameObjectWithTag("WhiteImage").GetComponent<Image>();
            bang = GetComponent<AudioSource>();
            mesh = transform.GetChild(1).gameObject;
            indicator = transform.GetChild(2).gameObject;
        }
        private void Update()
        {
            if (shouldFlash)
            {
                countdown -= Time.deltaTime;
                var weaponScript = transform.GetComponent<Weapon>();
                Destroy(weaponScript);

                if (countdown <= 0 && !hasFlashed)
                {
                    Flash();
                    hasFlashed = true;
                }
            }
        }
        private void DestroyObject()
        {
            Destroy(gameObject);
        }
        public void Flash()
        {
            Transform playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            Vector3 grenadeDirection = transform.position - playerTransform.position;
            float angle = Vector3.Angle(grenadeDirection, playerTransform.forward);
            float distanceComparision = Vector3.Distance(transform.position, playerTransform.position);
            bang.Play();

            if (angle < 60f && distanceComparision <= distance)
            {
                RaycastHit hit;

                if (Physics.Raycast(playerTransform.position, grenadeDirection, out hit, distanceComparision))
                {
                    if (hit.transform.gameObject != gameObject)
                    {
                        mesh.SetActive(false);
                        indicator.SetActive(false);
                        Invoke("DestroyObject", 3f);

                        return;
                    }
                }

                StartCoroutine(FlashCoroutine());
            }
            else
            {
                mesh.SetActive(false);
                indicator.SetActive(false);
                Invoke("DestroyObject", 3f);
            }
        }
        private IEnumerator FlashCoroutine()
        {
            whiteImage.color = new Vector4(1, 1, 1, 1);

            /*Collider[] collidersToAffect = Physics.OverlapSphere(transform.position, distance);

            foreach (Collider nearbyObject in collidersToAffect)
            {
              Enemy enemy = nearbyObject.GetComponent<Enemy>();

              if (enemy != null)
              {
                  float enemyDistance = Vector3.Distance(transform.position, enemy.transform.position);
                  float effect = Mathf.Clamp01((distance - enemyDistance) / distance);
                  enemy.ApplyStun();
              }*/

            foreach (Transform child in transform)
                child.gameObject.SetActive(false);

            float fadeSpeed = 1f;
            float modifier = 0.01f;
            float waitTime = 0;

            for (int i = 0; whiteImage.color.a > 0; i++)
            {
                whiteImage.color = new Vector4(1, 1, 1, fadeSpeed);
                fadeSpeed = fadeSpeed - 0.025f;
                modifier = modifier * 1.5f;
                waitTime = 0.5f - modifier;

                if (waitTime < 0.1f)
                    waitTime = 0.1f;

                bang.volume -= 0.05f;

                yield return new WaitForSeconds(waitTime);
            }

            bang.Stop();
            bang.volume = 1;
            Destroy(gameObject);
        }
        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("Glass"))
            {
                Glass glass = other.gameObject.GetComponent<Glass>();

                if (glass != null)
                {
                    float collisionForce = other.impulse.magnitude;

                    if (collisionForce >= minimalCollisionForceToBreakGlass)
                        glass.BreakFromGrenade(transform.position, impactForce);
                }
            }
            else
            {
                Rigidbody rb = GetComponent<Rigidbody>();
                rb.velocity *= 1f;
            }
        }
    }
}