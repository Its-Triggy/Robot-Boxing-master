using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LinearGrasperController : IGrasper {

    public float TorqueAmount;
   // public float fullOpenPosition { get; set; }
   // public float fullClosedPosition { get; set; }
   // public int _connectionIndex { get; set; }
    //public int ConnectionIndex;
    public GameObject LeftGrasper;
    private Rigidbody LeftRigid;
    public GameObject RightGrasper;
    private Rigidbody RightRigid;
    HingeJoint leftGrasperJoint;
    HingeJoint rightGrasperJoint;
    JointSpring leftJointSpring;
    JointSpring rightJointSpring;

    // Use this for initialization
    void Start () {
        LeftRigid = LeftGrasper.GetComponent<Rigidbody>();
        RightRigid = RightGrasper.GetComponent<Rigidbody>();
        leftGrasperJoint = LeftGrasper.GetComponent<HingeJoint>();
        rightGrasperJoint = RightGrasper.GetComponent<HingeJoint>();
        leftJointSpring = leftGrasperJoint.spring;
        rightJointSpring = rightGrasperJoint.spring;

        microsecondRange = fullOpenMicroseconds - fullClosedMicroseconds;
    }

    // Update is called once per frame
    void Update () {
		
	}

    //For the graspers, 1525*4 is fully closed and 992*4 is fully open

    public override void OpenCommand()
    {
        //If there is any amount that the jaws can open more
        if (Vector3.Angle(LeftRigid.transform.localEulerAngles, RightRigid.transform.localEulerAngles) <= 90)
        {
            LeftRigid.AddRelativeTorque(Vector3.up * TorqueAmount * -1);
            RightRigid.AddRelativeTorque(Vector3.up * TorqueAmount * 1);
        }
        else
        {
            float difference = Vector3.Angle(LeftRigid.transform.localEulerAngles, RightRigid.transform.localEulerAngles);
        }
    }

    public override void CloseCommand()
    {
        //If there is any amount that the jaws can close more
        //if (Mathf.Abs(LeftRigid.transform.localEulerAngles.y) + Mathf.Abs(RightRigid.transform.localEulerAngles.y) >= 1)
        if (Vector3.Angle(LeftRigid.transform.localEulerAngles, RightRigid.transform.localEulerAngles) >= 0)
        {
            LeftRigid.AddRelativeTorque(Vector3.up * TorqueAmount * 1);
            RightRigid.AddRelativeTorque(Vector3.up * TorqueAmount * -1);
        }
        else
        {
            float difference = Vector3.Angle(LeftRigid.transform.localEulerAngles, RightRigid.transform.localEulerAngles);
        }
    }

    /// <summary>
    /// Set's the hingejoint's spring.targetPosition to the desired opening percentage (assuming this works correctly)
    /// </summary>
    /// <param name="inNum"></param>
    public override void MoveToPercentage(float inNum)
    {
        float desiredPosition = inNum;

        //Now, address the joint and use the hingejoint's spring's targetPosition to set this desired position - let the spring's torque do the rest.
        //NOTE: Unity doesn't allow direct modification of spring target positions
        JointSpring leftSpr = leftGrasperJoint.spring;
        leftSpr.targetPosition = -1 * desiredPosition;
        leftGrasperJoint.spring = leftSpr;
        //leftJointSpring.targetPosition = desiredPosition;

        JointSpring rightSpr = rightGrasperJoint.spring;
        rightSpr.targetPosition = desiredPosition;
        rightGrasperJoint.spring = rightSpr;
        //rightJointSpring.targetPosition = -1 * desiredPosition;
    }


    /// <summary>
    /// Averages the distance of the two arms of the grasper. They are expected to be equal, opposite angles; but that may not always be the case.
    /// </summary>
    public override float GetGrasperOpening()
    {
        Vector3 leftPart = LeftGrasper.transform.position;
        Vector3 rightPart = RightGrasper.transform.position;
        Vector3 LineBetweenJaws = leftPart - rightPart;
        float DistanceBetweenJaws = LineBetweenJaws.magnitude;
        return DistanceBetweenJaws;
    }
}
