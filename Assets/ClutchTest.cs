using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClutchTest : MonoBehaviour {
    public HandSide whichHand;

    private OVRInput.Controller controller;
    public Vector3 referenceSensorPosition;
    public Quaternion referenceSensorRotation;
    public Vector3 referenceVirtualPosition;
    public Quaternion referenceVirtualRotation;
    public bool triggerState;
    public bool movingController = false;
    private OVRInput.RawButton thisTrigger;
    private Rigidbody rb;
    public Vector3 showSensorPosition;
    public float sensorYChange;
    public Vector3 sensorPositionChange;

    public float referenceSensorY;
    public float referenceVirtualY;

    private void Start()
    {
        if (whichHand == HandSide.Left)
        {
            controller = OVRInput.Controller.LTouch;
            thisTrigger = OVRInput.RawButton.LIndexTrigger;
        }
        else
        {
            controller = OVRInput.Controller.RTouch;
            thisTrigger = OVRInput.RawButton.RIndexTrigger;
        }
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        //For debugging:
        showSensorPosition = OVRInput.GetLocalControllerPosition(controller);

        //Check if the trigger/clutch is held down, and determine if this is a new change
        if (OVRInput.Get(thisTrigger))
        {
            if (triggerState == false)
            {
                triggerState = true;
             //   referenceSensorY = OVRInput.GetLocalControllerPosition(controller).y;
             //   referenceVirtualY = transform.position.y;

                referenceSensorPosition = OVRInput.GetLocalControllerPosition(controller);
                referenceVirtualPosition = transform.position;
            }
            else
            {
                MoveController();
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
        movingController = true;
    }

    public enum HandSide
    {
        Right,
        Left
    }
}
