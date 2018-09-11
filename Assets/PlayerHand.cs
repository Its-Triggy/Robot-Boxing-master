using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour {
    public HandSide whichHand;

    private OVRInput.Controller controller;
    public Vector3 referenceSensorPosition;
    public Quaternion referenceSensorRotation;
    public Vector3 referenceVirtualPosition;
    public Quaternion referenceVirtualRotation;
    public bool triggerState;
    public bool movingController = false;
    private OVRInput.RawButton thisClutch;
    private OVRInput.RawAxis1D thisTrigger;
    private Rigidbody rb;
    public Vector3 showSensorPosition;
    public Vector3 sensorPositionChange;
    public Quaternion sensorRotationChange;
    public float triggerAmount;
    public bool trackRotation;
    public bool alwaysFollow;

    private void Start()
    {
        if (whichHand == HandSide.Left)
        {
            controller = OVRInput.Controller.LTouch;
            thisClutch = OVRInput.RawButton.LHandTrigger;
            thisTrigger = OVRInput.RawAxis1D.LIndexTrigger; //OVRInput.RawButton.LIndexTrigger;
        }
        else
        {
            controller = OVRInput.Controller.RTouch;
            thisClutch = OVRInput.RawButton.RHandTrigger;
            thisTrigger = OVRInput.RawAxis1D.RIndexTrigger;
        }
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update() { }
    private void FixedUpdate()
    {
        //For debugging:
        showSensorPosition = OVRInput.GetLocalControllerPosition(controller);

        if (alwaysFollow)
        {
            transform.localPosition = OVRInput.GetLocalControllerPosition(controller);
            transform.localRotation = OVRInput.GetLocalControllerRotation(controller);
        }


        //Check if the trigger/clutch is held down, and determine if this is a new change
        if (OVRInput.Get(thisClutch))
        {
            if (triggerState == false)
            {
                triggerState = true;
                referenceSensorPosition = OVRInput.GetLocalControllerPosition(controller);
                referenceVirtualPosition = transform.position;

                referenceSensorRotation = OVRInput.GetLocalControllerRotation(controller);
                referenceVirtualRotation = transform.rotation;

                /*
                referenceVirtualPosition = transform.position; //new Vector3(transform.position.x, transform.position.y, transform.position.z);
                referenceVirtualRotation = transform.localRotation;
                Vector3 tempContPos = OVRInput.GetLocalControllerPosition(controller);
                referenceSensorPosition = OVRInput.GetLocalControllerPosition(controller); //new Vector3(tempContPos.x, tempContPos.y, tempContPos.z);
                referenceSensorRotation = OVRInput.GetLocalControllerRotation(controller);
                */
            }
            else
            {
                MoveController();
                SetJaws();
            }
        }
        else
        {
            triggerState = false;
            movingController = false;
        }
    }

    void MoveController()
    {
        sensorPositionChange = OVRInput.GetLocalControllerPosition(controller) - referenceSensorPosition;
        rb.MovePosition(referenceVirtualPosition + sensorPositionChange);

        //To get the difference between quaternions, need to multiply one by the inverse of another.
        if (trackRotation)
        {
            sensorRotationChange = OVRInput.GetLocalControllerRotation(controller) * Quaternion.Inverse(referenceSensorRotation);
            rb.MoveRotation(referenceVirtualRotation * sensorRotationChange);
        }
        movingController = true;
    }

    void SetJaws()
    {
        triggerAmount = 1.0f - OVRInput.Get(thisTrigger);
    }

    public enum HandSide
    {
        Right,
        Left
    }
}
