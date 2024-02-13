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
        private AudioEventManager audioEventManager;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            movementSound = transform.Find("Sounds/Movement").GetComponent<AudioSource>();
            audioEventManager = GameObject.FindGameObjectWithTag("AudioEventManager").GetComponent<AudioEventManager>();
        }
        private void Update()
        {
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
                        movementSound.pitch = Random.Range(0.35f, 0.65f);
                    else if(agent.velocity.magnitude > 1f && agent.velocity.magnitude != 0f)
                        movementSound.pitch = Random.Range(0.85f, 1.15f);

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