using UnityEngine;

public class HitBox : MonoBehaviour
{
    public EnemyHealth health;
    public PlayerHealth playerHealth;

    public void OnRaycastHit(Gun weapon, Vector3 direction)
    {
        health.DealDamage(Random.Range(weapon.minimumDamage, weapon.maximumDamage), direction);
    }
    public void OnExplosion(int damage)
    {
        health.DealDamage(damage, new Vector3(0, 0, 0));
    }
    public void OnRaycastHitPlayer(Gun weapon)
    {
        playerHealth.TakeDamage(Random.Range(weapon.minimumDamage, weapon.maximumDamage));
    }
    public void OnExplosionPlayer(int damage)
    {
        playerHealth.TakeDamage(damage);
    }
}