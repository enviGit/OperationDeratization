using RatGamesStudios.OperationDeratization.Enemy;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class Door : Interactable
    {
        [Header("References")]
        [SerializeField] private GameObject door;
        [SerializeField] private AudioSource doorSound;
        public AudioClip[] soundClips;
        private Animator animator;

        [Header("Door")]
        private bool doorOpen;
        private float enemyDetectionRadius = 2f;


        private void Start()
        {
            animator = door.GetComponent<Animator>();
        }
        private void Update()
        {
            if (DetectEnemyNearby())
            {
                doorOpen = true;
                animator.SetBool("IsOpen", doorOpen);
            }
            if (animator.GetBool("IsOpen"))
                prompt = "Close door";
            else
                prompt = "Open door";
        }
        protected override void Interact()
        {
            doorOpen = !doorOpen;
            doorSound.PlayOneShot(soundClips[0]);
            animator.SetBool("IsOpen", doorOpen);
        }
        private bool DetectEnemyNearby()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Enemy") && collider.GetComponent<EnemyHealth>().isAlive)
                    return true;
            }

            return false;
        }
    }
}