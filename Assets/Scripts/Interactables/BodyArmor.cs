using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyArmor : Interactable
{
    [Header("References")]
    public PlayerHealth playerArmor;
    private AudioSource pickingArmorSound;
    private float delayBeforeDestroy = 1f;
    private bool used = false;
    private List<MeshRenderer> meshes = new List<MeshRenderer>();

    private void Start()
    {
        pickingArmorSound = GetComponent<AudioSource>();

        foreach (Transform child in transform)
        {
            MeshRenderer mr = child.GetComponent<MeshRenderer>();

            if (mr != null)
                meshes.Add(mr);
        }
    }
    protected override void Interact()
    {
        if (!used && playerArmor.currentArmor <= 99)
        {
            playerArmor.PickupArmor();
            prompt = "";
            StartCoroutine(DestroyAfterSound());
            used = true;
        }
    }
    private IEnumerator DestroyAfterSound()
    {
        pickingArmorSound.Play();
        SetShaderParameters(0);
        float elapsedTime = 0f;
        float duration = delayBeforeDestroy;

        while (elapsedTime < duration)
        {
            SetShaderParameters(elapsedTime / duration);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        Destroy(gameObject);
    }
    private void SetShaderParameters(float disappearIntensity)
    {
        foreach (MeshRenderer skinnedMeshRenderer in meshes)
        {
            Material[] materials = skinnedMeshRenderer.materials;

            foreach (var material in materials)
                material.SetFloat("_dissolve", disappearIntensity);
        }
    }
}
