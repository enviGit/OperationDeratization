using UnityEngine;

public class TakeDmgTrigger:MonoBehaviour
{
   private void OnTriggerEnter(Collider other)
 {
       if (other.CompareTag("Player"))
      {
           PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();

          if (playerHealth != null)
             playerHealth.TakeFallingDamage(100f);
    }
    }
  
}