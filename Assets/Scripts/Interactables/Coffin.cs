using RatGamesStudios.OperationDeratization.Enemy;
using RatGamesStudios.OperationDeratization.Manager;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class Coffin : Interactable
    {
        [Header("References")]
        [SerializeField] private GameObject coffin;
        private AudioSource sound;
        private AudioEventManager audioEventManager;

        [Header("Door")]
        private bool coffinOpen;
        private float enemyDetectionRadius = 2f;
        private Animator animator;

        private void Start()
        {
            sound = GetComponent<AudioSource>();
            animator = coffin.GetComponent<Animator>();
            audioEventManager = GameObject.FindGameObjectWithTag("AudioEventManager").GetComponent<AudioEventManager>();
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
            audioEventManager.NotifyAudioEvent(sound);
        }
        private bool DetectEnemyNearby()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius);

            foreach (Collider collider in colliders)
            {
                if (collider.CompareTag("Enemy") && collider.GetComponent<EnemyHealth>().isAlive)
                {
                    if (!coffinOpen)
                    {
                        sound.PlayOneShot(sound.clip);
                        audioEventManager.NotifyAudioEvent(sound);
                    }

                    return true;
                }
            }

            return false;
        }
    }
}