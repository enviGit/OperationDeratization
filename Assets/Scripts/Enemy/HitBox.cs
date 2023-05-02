using UnityEngine;

public class HitBox : MonoBehaviour
{
    public EnemyHealth health;

    public void OnRaycastHit(Gun weapon, Vector3 direction)
    {
        health.DealDamage(Random.Range(weapon.minimumDamage, weapon.maximumDamage), direction);
    }
}