using UnityEngine;
using System;

[Serializable]
public class PlayerStance
{
    public Stance playerStance;
    public PlayerStance playerStandStance;
    public PlayerStance playerCrouchStance;
    public float camHeight;

    public enum Stance
    {
        Idle,
        Walking,
        Jumping,
        Crouching,
        Climbing
    }
}
