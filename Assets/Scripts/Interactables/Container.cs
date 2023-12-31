using RatGamesStudios.OperationDeratization.Enemy;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class Container : Interactable
    {
        [Header("References")]
        [SerializeField] private GameObject container;
        private AudioSource sound;

        [Header("Container")]
        private bool containerOpen;
        private float enemyDetectionRadius = 2f;

        private void Start()
        {
            sound = GetComponent<AudioSource>();
        }
        private void Update()
        {
            if (DetectEnemyNearby())
            {
                containerOpen = true;
                container.GetComponent<Animator>().SetBool("IsOpen", containerOpen);
            }
            if (container.GetComponent<Animator>().GetBool("IsOpen"))
                prompt = "Close container";
            else
                prompt = "Open container";
        }
        protected override void Interact()
        {
            containerOpen = !containerOpen;
            container.GetComponent<Animator>().SetBool("IsOpen", containerOpen);
            sound.PlayOneShot(sound.clip);
        }
        private bool DetectEnemyNearby()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Enemy") && collider.GetComponent<EnemyHealth>().isAlive)
                {
                    if (!containerOpen)
                        sound.PlayOneShot(sound.clip);

                    return true;
                }
            }

            return false;
        }
    }
}