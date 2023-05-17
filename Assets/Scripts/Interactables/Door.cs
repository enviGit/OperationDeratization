using UnityEngine;

public class Door : Interactable
{
    [Header("References")]
    [SerializeField] private GameObject door;

    [Header("Door")]
    private bool doorOpen;

    protected override void Interact()
    {
        prompt = "Open door";
        doorOpen = !doorOpen;
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);

        if (doorOpen)
            prompt = "Close door";
        else
            prompt = "Open door";
    }
}
