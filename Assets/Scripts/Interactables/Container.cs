using UnityEngine;

public class Container : Interactable
{
    [Header("References")]
    [SerializeField] private GameObject container;
    [SerializeField] private AudioSource sound;

    [Header("Container")]
    private bool containerOpen;
    private float enemyDetectionRadius = 2f;

    private void Update()
    {
        if (DetectEnemyNearby())
        {
            containerOpen = true;
            container.GetComponent<Animator>().SetBool("IsOpen", containerOpen);
        }
        if (container.GetComponent<Animator>().GetBool("IsOpen"))
            prompt = "Close container";
        else
            prompt = "Open container";
    }
    protected override void Interact()
    {
        containerOpen = !containerOpen;
        container.GetComponent<Animator>().SetBool("IsOpen", containerOpen);
        sound.Play();
    }
    private bool DetectEnemyNearby()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy") && collider.GetComponent<EnemyHealth>().isAlive)
            {
                if (!containerOpen)
                    collider.transform.Find("Sounds/Chest").GetComponent<AudioSource>().Play();

                return true;
            }
        }

        return false;
    }
}
