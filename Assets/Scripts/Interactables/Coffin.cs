using RatGamesStudios.OperationDeratization.Enemy;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class Coffin : Interactable
    {
        [Header("References")]
        [SerializeField] private GameObject coffin;
        private AudioSource sound;

        [Header("Door")]
        private bool coffinOpen;
        private float enemyDetectionRadius = 2f;
        private Animator animator;

        private void Start()
        {
            sound = GetComponent<AudioSource>();
            animator = coffin.GetComponent<Animator>();
        }
        private void Update()
        {
            if (DetectEnemyNearby())
            {
                coffinOpen = true;
                animator.SetBool("IsOpen", coffinOpen);
            }
            if (animator.GetBool("IsOpen"))
                prompt = "Close coffin";
            else
                prompt = "Open coffin";
        }
        protected override void Interact()
        {
            coffinOpen = !coffinOpen;
            animator.SetBool("IsOpen", coffinOpen);
            sound.PlayOneShot(sound.clip);
        }
        private bool DetectEnemyNearby()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Enemy") && collider.GetComponent<EnemyHealth>().isAlive)
                {
                    if (!coffinOpen)
                        sound.PlayOneShot(sound.clip);

                    return true;
                }
            }

            return false;
        }
    }
}