using UnityEngine;

public class Door : Interactable
{
    [SerializeField]
    private GameObject door;
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
