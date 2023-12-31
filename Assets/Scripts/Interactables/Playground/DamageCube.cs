using RatGamesStudios.OperationDeratization.Player;
using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables.Playground
{
    public class DamageCube : Interactable
    {
        [Header("References")]
        public PlayerHealth hp;

        protected override void Interact()
        {
            if (hp != null)
                hp.TakeDamage(Random.Range(5, 20));
        }
    }
}