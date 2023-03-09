/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderTrigger : MonoBehaviour
{
    public float climbSpeed = 6f;
    public Transform topOfLadder;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerMotor>().SetLadderInfo(climbSpeed, topOfLadder);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<PlayerMotor>().ClearLadderInfo();
        }
    }
}
*/