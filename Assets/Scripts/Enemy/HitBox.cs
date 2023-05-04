using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public EnemyHealth health;
    private Gun gun;
    public PlayerHealth playerHealth;
    private Dictionary<string, float> damageMultiplier = new Dictionary<string, float>()
    {
        { "Hips", 1f },
        { "LeftUpLeg", 0.5f },
        { "LeftLeg", 0.5f },
        { "LeftFoot", 0.2f },
        { "RightUpLeg", 0.5f },
        { "RightLeg", 0.5f },
        { "RightFoot", 0.2f },
        { "Spine2", 1f },
        { "LeftArm", 1f },
        { "LeftForeArm", 0.7f },
        { "Head", 2.5f },
        { "RightArm", 0.7f },
        { "RightForeArm", 1f },
    };

    public void OnRaycastHit(Gun weapon, Vector3 direction)
    {
        gun = weapon;
        int damage = GetDamageFromHitBox(gameObject);
        health.DealDamage(damage, direction);
    }
    public void OnExplosion(int damage)
    {
        health.DealDamage(damage, new Vector3(0, 0, 0));
    }
    public void OnRaycastHitPlayer(Gun weapon)
    {
        gun = weapon;
        int damage = GetDamageFromHitBox(gameObject);
        playerHealth.TakeDamage(damage);
    }
    public void OnExplosionPlayer(int damage)
    {
        playerHealth.TakeDamage(damage);
    }
    private int GetDamageFromHitBox(GameObject hitBoxObject)
    {
        string boneName = hitBoxObject.name;

        if (damageMultiplier.ContainsKey(boneName))
            return Mathf.RoundToInt(Random.Range(gun.minimumDamage, gun.maximumDamage) * damageMultiplier[boneName]);

        return Random.Range(gun.minimumDamage, gun.maximumDamage);
    }
}