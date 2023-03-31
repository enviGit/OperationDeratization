using UnityEngine;

public class LadderTrigger : Interactable
{
    [SerializeField] private float climbSpeed = 3f;
    private bool isClimbing;
     private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isClimbing = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isClimbing = false;
        }
    }
}