using RatGamesStudios.OperationDeratization.Manager;
using UnityEngine;
using UnityEngine.AI;

namespace RatGamesStudios.OperationDeratization.Enemy
{
    public class AiLocomotion : MonoBehaviour
    {
        private NavMeshAgent agent;
        private Animator animator;
        private AudioSource movementSound;
        private EnemyHealth health;
        private AudioEventManager audioEventManager;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            health = GetComponent<EnemyHealth>();
            movementSound = transform.Find("Sounds/Movement").GetComponent<AudioSource>();
            audioEventManager = GameObject.FindGameObjectWithTag("AudioEventManager").GetComponent<AudioEventManager>();
        }
        private void Update()
        {
            if(health.isAlive)
                SetSpeed();
        }
        private void SetSpeed()
        {
            if (agent.hasPath)
            {
                animator.SetFloat("Speed", agent.velocity.magnitude);

                if (!movementSound.isPlaying)
                {
                    if(agent.velocity.magnitude <= 1f && agent.velocity.magnitude != 0f)
                    {
                        movementSound.pitch = Random.Range(0.35f, 0.65f);
                        movementSound.volume = 0.5f;
                        movementSound.maxDistance = 15f;
                    }
                    else if(agent.velocity.magnitude > 1f && agent.velocity.magnitude != 0f)
                    {
                        movementSound.pitch = Random.Range(0.85f, 1.15f);
                        movementSound.volume = 1f;
                        movementSound.maxDistance = 30f;
                    }

                    movementSound.Play();
                    audioEventManager.NotifyAudioEvent(movementSound);
                }
            }
            else
            {
                movementSound.Stop();
                animator.SetFloat("Speed", 0);
            }
        }
    }
}