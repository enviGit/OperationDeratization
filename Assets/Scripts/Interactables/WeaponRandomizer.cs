using System.Collections.Generic;
using UnityEngine;

public class WeaponRandomizer : MonoBehaviour
{
    private List<Transform> weaponTransforms = new List<Transform>();

    private void Start()
    {
        GameObject[] weapons = GameObject.FindGameObjectsWithTag("Weapon");

        foreach (GameObject weapon in weapons)
            weaponTransforms.Add(weapon.transform);

        RandomizeWeapons();
    }
    private void RandomizeWeapons()
    {
        for (int i = 0; i < weaponTransforms.Count; i++)
        {
            int randomIndex = Random.Range(0, weaponTransforms.Count);
            SwapTransforms(i, randomIndex);
        }
    }
    private void SwapTransforms(int indexA, int indexB)
    {
        Transform temp = weaponTransforms[indexA];
        weaponTransforms[indexA] = weaponTransforms[indexB];
        weaponTransforms[indexB] = temp;
        Vector3 positionA = weaponTransforms[indexA].position;
        Quaternion rotationA = weaponTransforms[indexA].rotation;
        weaponTransforms[indexA].position = weaponTransforms[indexB].position;
        weaponTransforms[indexA].rotation = weaponTransforms[indexB].rotation;
        weaponTransforms[indexB].position = positionA;
        weaponTransforms[indexB].rotation = rotationA;
    }
}
