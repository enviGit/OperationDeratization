using UnityEngine;

public class DestroyInOneHit : MonoBehaviour, IDamageable
{
    public void DealDamage(int damage)
    {
        Destroy(gameObject);
    }
}
