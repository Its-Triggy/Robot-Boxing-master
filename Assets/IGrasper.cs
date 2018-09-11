using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


//interface IGrasper
public abstract class IGrasper : DOF
{
    public float fullOpenMicroseconds;// { get; set; }
    public float fullClosedMicroseconds;// { get; set; }
    public float maximumOpening;
    public float microsecondRange;
   // int _connectionIndex { get; set; }

    public abstract void OpenCommand();
    public abstract void CloseCommand();
    public abstract void MoveToPercentage(float inNum);
    public abstract float GetGrasperOpening();
}
