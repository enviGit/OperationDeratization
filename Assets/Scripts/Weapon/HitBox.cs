using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    public EnemyHealth health;
    private Gun gun;
    public int damageToPlayer = 0;
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
        { "Spine2", 1f }, //Upper Chest
        { "LeftArm", 0.75f },
        { "LeftForeArm", 0.7f },
        { "Head", 2.5f },
        { "RightArm", 0.75f },
        { "RightForeArm", 0.7f },
    };
    private string[] bonePrefixes = { "mixamorig9:", "mixamorig4:", "mixamorig10:", "mixamorig:" };

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
        if(gun == null) 
            gun = weapon;
        /*if (damageMultiplier.ContainsKey(gameObject.name) || ContainsAnyPrefix(gameObject.name))
            damageToPlayer = GetDamageFromHitBox(gameObject);
        if (damageToPlayer != 0)
            playerHealth.TakeDamage(damageToPlayer);*/
        if (playerHealth != null && (damageMultiplier.ContainsKey(gameObject.name) || ContainsAnyPrefix(gameObject.name)))
        {
            damageToPlayer = GetDamageFromHitBox(gameObject);
            playerHealth.TakeDamage(damageToPlayer);
        }

        //Debug.Log(gameObject);
    }
    public void OnExplosionPlayer(int damage)
    {
        playerHealth.TakeDamage(damage);
    }
    private int GetDamageFromHitBox(GameObject hitBoxObject)
    {
        string boneName = GetCleanBoneName(hitBoxObject.name);

        if (damageMultiplier.ContainsKey(boneName))
        {
            float distance = Vector3.Distance(hitBoxObject.transform.position, transform.position);
            float damageMultiplierAtDistance = 0;

            if (gun.gunType == GunType.Shotgun)
                damageMultiplierAtDistance = 1 / (1 + 0.1f * distance * distance);
            else
                damageMultiplierAtDistance = 1 / (1 + 0.05f * distance);

            float damageMultiplierAtHitbox = damageMultiplier[boneName];

            return Mathf.RoundToInt(Random.Range(gun.minimumDamage, gun.maximumDamage) * damageMultiplierAtDistance * damageMultiplierAtHitbox);
        }
        else
            return Random.Range(gun.minimumDamage, gun.maximumDamage);
    }
    private string GetCleanBoneName(string fullName)
    {
        foreach (string prefix in bonePrefixes)
        {
            if (fullName.StartsWith(prefix))
                return fullName.Substring(prefix.Length);
        }

        return fullName;
    }
    private bool ContainsAnyPrefix(string fullName)
    {
        foreach (string prefix in bonePrefixes)
        {
            if (fullName.StartsWith(prefix))
                return true;
        }

        return false;
    }
}