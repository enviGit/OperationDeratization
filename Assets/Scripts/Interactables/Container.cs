using RatGamesStudios.OperationDeratization.Enemy;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class Container : Interactable
    {
        [Header("References")]
        [SerializeField] private GameObject container;
        private AudioSource sound;
        private Animator animator;

        [Header("Container")]
        private bool containerOpen;
        private float enemyDetectionRadius = 2f;

        private void Start()
        {
            sound = GetComponent<AudioSource>();
            animator = container.GetComponent<Animator>();
        }
        private void Update()
        {
            if (DetectEnemyNearby())
            {
                containerOpen = true;
                animator.SetBool("IsOpen", containerOpen);
            }
            if (animator.GetBool("IsOpen"))
                prompt = "Close container";
            else
                prompt = "Open container";
        }
        protected override void Interact()
        {
            containerOpen = !containerOpen;
            animator.SetBool("IsOpen", containerOpen);
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