using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerStance
{
    public enum Stance
    {
        Stand,
        Crouch,
        Prone
    }

    public float camHeight;
    public CapsuleCollider collider;
}
