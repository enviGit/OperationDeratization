using System.Collections;
using UnityEngine;

public class FirstAidKit : Interactable
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public float hpToRestore = 15f;
    private float delayBeforeDestroy = 3.5f;
    private AudioSource restoreHealthSound;
    private bool used = false;
    private MeshRenderer mesh;

    private void Start()
    {
        restoreHealthSound = GetComponent<AudioSource>();
        mesh = GetComponent<MeshRenderer>();
    }
    protected override void Interact()
    {
        if (!used && playerHealth.currentHealth < 99f)
        {
            playerHealth.RestoreHealth(hpToRestore);
            prompt = "";
            StartCoroutine(DestroyAfterSound());
            used = true;
        }
    }
    private IEnumerator DestroyAfterSound()
    {
        restoreHealthSound.Play();
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
        mesh.material.SetFloat("_dissolve", disappearIntensity);
    }
}