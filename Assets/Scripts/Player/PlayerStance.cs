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
        Crouch
    }

    public float camHeight;
    public CapsuleCollider collider;
}
