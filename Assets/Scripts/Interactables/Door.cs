using UnityEngine;

public class Door : Interactable
{
    [Header("References")]
    [SerializeField] private GameObject door;

    [Header("Door")]
    private bool doorOpen;
    private float enemyDetectionRadius = 2f;

    private void Update()
    {
        if (DetectEnemyNearby())
        {
            doorOpen = true;
            door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
        }
        if (door.GetComponent<Animator>().GetBool("IsOpen"))
            prompt = "Close door";
        else
            prompt = "Open door";
    }
    protected override void Interact()
    {
        doorOpen = !doorOpen;
        door.GetComponent<Animator>().SetBool("IsOpen", doorOpen);
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
