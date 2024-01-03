using UnityEngine;

namespace RatGamesStudios.OperationDeratization.Interactables
{
    public abstract class Interactable : MonoBehaviour
    {
        [Header("Interactables")]
        public bool useEvents;
        public string prompt;

        public virtual string OnLook()
        {
            return prompt;
        }
        public void BaseInteract()
        {
            if (useEvents)
                GetComponent<InteractionEvent>().OnInteract.Invoke();

            Interact();
        }
        protected virtual void Interact()
        {

        }
    }
}