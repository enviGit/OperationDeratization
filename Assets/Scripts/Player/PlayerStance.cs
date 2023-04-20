using UnityEngine;
using System;

[Serializable]
public class PlayerStance
{
    public Stance playerStance;
    public float camHeight;

    public enum Stance
    {
        Idle,
        Walking,
        Crouching,
    }
}
