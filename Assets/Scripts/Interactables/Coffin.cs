using UnityEngine;

public class Coffin : Interactable
{
    [Header("References")]
    [SerializeField] private GameObject container;

    [Header("Container")]
    private bool OpenCoffin;

    protected override void Interact()
    {
        prompt = "Open container";
        OpenCoffin = !OpenCoffin;
        container.GetComponent<Animator>().SetBool("IsOpen", OpenCoffin);

        if (OpenCoffin)
            prompt = "Close container";
        else
            prompt = "Open container";
    }
}
