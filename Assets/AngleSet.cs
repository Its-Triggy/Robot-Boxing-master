using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Collection of all angles of the virtual or real arm
/// </summary>
//public class AngleSet
public class AngleSet <TKey,TValue> : Dictionary<TKey, TValue>
{ 
    /*
    public AngleUtils.ServoAngle DOF1 = new AngleUtils.ServoAngle();
    public AngleUtils.ServoAngle DOF2 = new AngleUtils.ServoAngle();
    public AngleUtils.ServoAngle DOF3 = new AngleUtils.ServoAngle();
    public AngleUtils.ServoAngle DOF4 = new AngleUtils.ServoAngle();
    public AngleUtils.ServoAngle DOF5 = new AngleUtils.ServoAngle();
    public AngleUtils.ServoAngle DOF6 = new AngleUtils.ServoAngle();
    public AngleUtils.ServoAngle DOF7 = new AngleUtils.ServoAngle();

    private Dictionary<int, AngleUtils.ServoAngle> dictionary = new Dictionary<int, AngleUtils.ServoAngle>();

    public AngleSet()
    {
        DOF1 = new AngleUtils.ServoAngle();
        DOF2 = new AngleUtils.ServoAngle();
        DOF3 = new AngleUtils.ServoAngle();
        DOF4 = new AngleUtils.ServoAngle();
        DOF5 = new AngleUtils.ServoAngle();
        DOF6 = new AngleUtils.ServoAngle();
        DOF7 = new AngleUtils.ServoAngle();

        dictionary.Add(0, DOF1);
        dictionary.Add(1, DOF2);
        dictionary.Add(2, DOF3);
        dictionary.Add(3, DOF4);
        dictionary.Add(4, DOF5);
        dictionary.Add(5, DOF6);
        dictionary.Add(6, DOF7);
    }

    //Treat this class like a list
    public AngleUtils.ServoAngle this[int index]
    {
        get
        {
            return dictionary[index];
        }
        set
        {
            dictionary.Add(index, value);
        }
    }
    */
}
