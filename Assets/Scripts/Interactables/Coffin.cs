using UnityEngine;

public class Coffin : Interactable
{
    [Header("References")]
    [SerializeField] private GameObject coffin;
    [SerializeField] private AudioSource sound;

    [Header("Door")]
    private bool coffinOpen;
    private float enemyDetectionRadius = 2f;

    private void Update()
    {
        if (DetectEnemyNearby())
        {
            coffinOpen = true;
            coffin.GetComponent<Animator>().SetBool("IsOpen", coffinOpen);
            sound.Play();
        }
        if (coffin.GetComponent<Animator>().GetBool("IsOpen"))
            prompt = "Close coffin";
        else
            prompt = "Open coffin";
    }
    protected override void Interact()
    {
        coffinOpen = !coffinOpen;
        coffin.GetComponent<Animator>().SetBool("IsOpen", coffinOpen);
        sound.Play();
    }
    private bool DetectEnemyNearby()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, enemyDetectionRadius);

        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy") && collider.GetComponent<EnemyHealth>().isAlive)
                return true;
        }

        return false;
    }
}
