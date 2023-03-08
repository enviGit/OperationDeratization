using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Container : Interactable
{
    [SerializeField]
    private GameObject container;
    private bool containerOpen;

    protected override void Interact()
    {
        prompt = "Open Container";
        containerOpen = !containerOpen;
        container.GetComponent<Animator>().SetBool("IsOpen", containerOpen);

        if (containerOpen)
            prompt = "Close Container";
        else
            prompt = "Open Container";
    }
}
