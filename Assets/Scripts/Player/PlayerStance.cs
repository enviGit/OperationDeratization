using System;
using UnityEngine;

[Serializable]
public class PlayerStance
{
    [Header("References")]
    public Stance playerStance;

    [Header("Player stance")]
    public float camHeight;
    public enum Stance
    {
        Idle,
        Walking,
        Crouching,
        Running
    }
}
