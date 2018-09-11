using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Collection of the VIRTUAL servos 
/// </summary>
public class VirtualServoSet : MonoBehaviour {

    GameObject fullArm;
    public GameObject DOF7;
    public float DOF7Angle;
    public GameObject grasper;
    public IGrasper grasperController;

    /// <summary>
    /// Collection of the virtual servos, indexed by ArmLocationIndex (NOT the connection index), starting with 0
    /// </summary>
    public Dictionary<int, VirtualServo> dictVirtualServoSet = new Dictionary<int, VirtualServo>();

    private List<int> reversalList = new List<int>(); 
    
    public VirtualServoSet()
    {
    }

    public void BuildVirtualServoSet(GameObject inArm)
    {
        try
        {
            //This should get all DOF-tagged children even though they are nested under each other
            Transform[] allChildren;
            allChildren = inArm.GetComponentsInChildren<Transform>();
            List<GameObject> listOfChildrenObjects = new List<GameObject>(); ;
            foreach (Transform child in allChildren) //var child in inArm. GetComponentsInChildren<VirtualServo>(false))
            {
                if (child.tag == "DOF" && child.gameObject.activeInHierarchy)
                {
                    listOfChildrenObjects.Add(child.gameObject);
                }
            }
            //Now add those servos to the dictionary, in order
            foreach (GameObject vs in listOfChildrenObjects) //(GameObject vs in properlySortedArrayOfChildren)
            {
                int errorline = 0;
                int testIndex = 0;
                try
                {
                    VirtualServo testServo = vs.GetComponent<VirtualServo>();
                    errorline = 1;
                    testIndex = vs.GetComponent<VirtualServo>().armLocationIndex;
                    errorline = 2;
                    dictVirtualServoSet.Add(vs.GetComponent<VirtualServo>().armLocationIndex, vs.GetComponent<VirtualServo>()); //was vs.connectionIndex, I changed it on 12/26/2017. dictVirtualServoSet is primarily from the virtual (not physical) perspective; hence the index should be that virtual index
                    errorline = 3;
                }
                catch(Exception ex)
                {
                    int thisIsError = errorline;
                    Debug.Log("Error inside the last loop of BuildVirtualServoSet at logpoint " + errorline + " and at armLocationIndex " + testIndex);
                    Debug.Log("Error inside the last loop of BuildVirtualServoSet with armLocationIndex: " + vs.GetComponent<VirtualServo>().armLocationIndex + ". Message: " + ex.ToString());
                }
            }
        }
        catch(Exception ex)
        {
            Debug.Log("Error building the virtual servo set, in function BuildVirtualServoSet: " + ex.ToString());
        }
    }

    void Start ()
    {
        fullArm = gameObject;

        grasper = DOF7.gameObject;
        try
        {
            grasperController = grasper.GetComponent<IGrasper>();
        }
        catch(Exception ex)
        {
            Debug.LogError("Unable to find an AngleGrasperController in VirtualServoSet. Error: " + ex.ToString());
        }
    }

    private void Awake()
    {
    }

    public VirtualServo GetServo(int inNum)
    {
        if (!dictVirtualServoSet.ContainsKey(inNum))
        {
            Debug.Log("Error inside GetServo(): the specified key " + inNum + " is not in the dictionary");
        }
        return dictVirtualServoSet[inNum];
    }

    /// <summary>
    /// read and record (to pCurrentRotation) the relative angles of the servos
    /// </summary>
    public void RecordRelativeAngles()
    {
        if (dictVirtualServoSet.Count == 0)
        {
            bool isItActive = fullArm.activeInHierarchy;
            Debug.LogError("Error inside ReadRelativeAngles: dictVirtualServoSet hasn't been populated yet, or it's size is 0."); // And it's parent arm is: " + fullArm.GetComponent<ArmController>().nameForDebugging + " with activity status: " + isItActive);
            return;
        }
        //For the first DOF: compare it's designated axis to the same axis of the parent arm (we are assuming that it's axis is Y).
        //For subsequent DOF: compare it's designated axis to that same axis of the preceding DOF
        dictVirtualServoSet[0].CurrentRotation.Set(VirtualServo.ServoAngle.AngleType.Degree, dictVirtualServoSet[0].transform.localEulerAngles.y);
        int debugger = 0;
        try
        {
            for (int i = 1; i < dictVirtualServoSet.Count; i++)
            {
                debugger = i;
                //We need to use check which axis the DOF uses, and pick the equation implementation made for that angle
                float angle = 0;
                
                //Need to do it with quaternions; as otherwise the angle calcs go nuts when they all deviate.
                //Find the rotation needed to move from one quaternion to another by multiplying their inverses (or their negative, which I think is the same)
                //Quaternion quatDifference = Quaternion.Inverse(dictVirtualServoSet[i - 1].transform.rotation) * dictVirtualServoSet[i].transform.rotation; //Quaternion.Inverse(dictVirtualServoSet[i].transform.rotation);
                Quaternion quatDifference = GetQuatDifference(i);

                if (dictVirtualServoSet[i].rotationAxis == DOF.RotationAxis.x)
                {
                    angle = quatDifference.eulerAngles.x;
                }
                if (dictVirtualServoSet[i].rotationAxis == DOF.RotationAxis.y)
                {
                    angle = quatDifference.eulerAngles.y;
                }
                if (dictVirtualServoSet[i].rotationAxis == DOF.RotationAxis.z)
                {
                    angle = quatDifference.eulerAngles.z;
                }
                
                //Now finally set that angle
                dictVirtualServoSet[i].CurrentRotation.Set(VirtualServo.ServoAngle.AngleType.Degree, angle);
            }
        }
        catch(Exception ex)
        {
            Debug.Log("Error inside ReadRelativeAngles() at value " + debugger + ". Perhaps some DOF's do not have the DOF tag. Otherwise, " + ex.ToString());
        }
    }

    /// <summary>
    /// Find the rotation difference between this DOF and the one it connects to - including the possibility of a forced comparison
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    private Quaternion GetQuatDifference(int i)
    {
        Quaternion quatDifference;

        //If there is no comparison override (needed for lever'ed DOFs), use the simple calculation
        if (dictVirtualServoSet[i].forcedRelativeAngle == null)
        {
            quatDifference = Quaternion.Inverse(dictVirtualServoSet[i - 1].transform.rotation) * dictVirtualServoSet[i].transform.rotation;
        }
        else
        {
            try
            {
                quatDifference = Quaternion.Inverse(dictVirtualServoSet[i].forcedRelativeAngle.transform.rotation) * dictVirtualServoSet[i].transform.rotation;
            }
            catch(Exception ex)
            {
                Debug.LogError("Unable to find the quat difference at " + i + " when there is an override: " + ex.ToString());
                quatDifference = Quaternion.identity;
            }
        }

        return quatDifference;
    }


    private GameObject TryToGetDOF(string dofName)
    {
        Transform[] children = GetComponentsInChildren<Transform>();

        foreach (Transform child in children)
        {
            if (child.gameObject.name == dofName)
            {
                return child.gameObject;
            }
        }

        return null;
    }

    private void TryToAdd<T>(List<T> inList, T inChild)
    {
        try
        {
            inList.Add(inChild);
        }
        catch(Exception ex)
        {
            Debug.LogError("Unable to add item to a list, inside VirtualServoSet class: " + ex.ToString());
        }
    }

    // Update is called once per frame
    void Update () {
    }

    /// <summary>
    /// Returns the angle for the servo that is at the input DOF index (NOT the connection index)
    /// </summary>
    /// <param name="inServoNum"></param>
    /// <returns></returns>
    public VirtualServo.ServoAngle GetCurrentRotation(int inServoNum)
    {
        return GetServo(inServoNum).CurrentRotation;
    }

    /// <summary>
    /// TODO: change the linkages so that I use the HingeJoint component's .angle property, rather than needing to specifically identify the axis in code
    /// </summary>
    void FixedUpdate()
    {
        /*
        if (gameObject.activeInHierarchy)
        {
            ReadRelativeAngles();
        }
        */
    }

    /*
    private int ReversalBoolToInt(int inIndex)
    {
        if (dictVirtualServoSet[inIndex].reverseMovement == true)
        {
            return -1;
        }
        return 1;
    }
    */
}



public enum Direction
{
    Clockwise,
    CounterClockwise
};
