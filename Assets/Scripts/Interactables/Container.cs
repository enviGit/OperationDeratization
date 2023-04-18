using UnityEngine;

public class Container : Interactable
{
    [SerializeField]
    private GameObject container;
    private bool containerOpen;

    protected override void Interact()
    {
        prompt = "Open container";
        containerOpen = !containerOpen;
        container.GetComponent<Animator>().SetBool("IsOpen", containerOpen);

        if (containerOpen)
            prompt = "Close container";
        else
            prompt = "Open container";
    }
}
