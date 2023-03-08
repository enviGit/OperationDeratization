using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingCube : Interactable
{
    private PlayerHealth hp;

    private void Start()
    {
        hp = FindObjectOfType<PlayerHealth>();
    }
    protected override void Interact()
    {
        if (hp != null)
            hp.RestoreHealth(Random.Range(5, 20));
    }
}