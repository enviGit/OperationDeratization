using UnityEngine;

public class TakeDmgTrigger : MonoBehaviour
{
    private Collider obj;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            obj = other;
            PlayerHealth playerHealth = obj.GetComponent<PlayerHealth>();

            if (playerHealth != null)
                InvokeRepeating("DealGasDamage", 0f, 0.5f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            CancelInvoke("DealGasDamage");
    }
    private void DealGasDamage()
    {
        PlayerHealth playerHealth = obj.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.TakeGasDamage(Random.Range(5, 18));
    }
}