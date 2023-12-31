using RatGamesStudios.OperationDeratization.Enemy;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public class Gate : Interactable
    {
        [Header("References")]
        [SerializeField] private GameObject gate;

        [Header("Door")]
        private bool gateOpen;
        private float enemyDetectionRadius = 2f;

        private void Update()
        {
            if (DetectEnemyNearby())
            {
                gateOpen = true;
                gate.GetComponent<Animator>().SetBool("IsOpen", gateOpen);
            }
            if (gate.GetComponent<Animator>().GetBool("IsOpen"))
                prompt = "Close gate";
            else
                prompt = "Open gate";
        }
        protected override void Interact()
        {
            gateOpen = !gateOpen;
            gate.GetComponent<Animator>().SetBool("IsOpen", gateOpen);
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
