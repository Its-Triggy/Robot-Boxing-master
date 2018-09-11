using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;


public abstract class DOF : MonoBehaviour
{
    public bool reverseMovement;
    public float angleOffset;
    //public ushort DOFSpeed;
    public RotationAxis rotationAxis;

    /// <summary>
    /// index for the connection on the Pololu/Arduino/whatever board
    /// </summary>
    public byte ConnectionIndex;

    /// <summary>
    /// Location on the arm - 0 is the base rotation, 1 is the main DOF, etc
    /// </summary>
    public byte armLocationIndex;


    public DOF()
    {

    }
    

    public DOF(byte inConnectionIndex, int inOffset)
    {
        ConnectionIndex = inConnectionIndex;
        angleOffset = inOffset;
    }

    public enum RotationAxis
    {
        x,
        y,
        z
    }
}


