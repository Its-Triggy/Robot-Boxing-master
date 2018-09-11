using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright (C) 2017 Chris Edwards - All Rights Reserved
 * Unauthorized copying of this file is prohibited without permission.
 * Proprietary and confidential
 * Written by Chris Edwards <chris45215@gmail.com>, November 2017
 */


public class DestinationMovement : MonoBehaviour {
    public GameObject robotConnection;
    private WorldManager worldManager;
    private Rigidbody rb;
    public bool LeapLeftHandRoutine;
    public bool MovePointRoutine;
    public bool OculusRoutine;
    public bool FollowOnePoint;
    public bool ForceToOnePoint;
    private Vector3 priorFrameTargetPosition;
    private Quaternion priorFrameTargetRotation;
    public float TransformMagnitudeFromPID;
    public float RotateMagnitudeFromPID;
    public float TranslationForceMultiplier;
    public float RotationForceMultiplier;
    public PIDController PositionPID;
    public PIDController RotationPID;
    public Transform robotArm;
    //private ArmController linkedArmController;
    public Transform firstMovePoint;
    public Vector3 ThumbAdjustmentAxis = new Vector3(0, 0, 0);
    public float ThumbAdjustmentAngle = 90;

    public Vector3 TryingToReach;
    private Vector3 startingPoint;

    public Transform tCurrentWaypoint;
    private MovePoint mCurrentWaypoint;
    public Transform tNextWaypoint;
    private MovePoint mNextWaypoint;
    private float durationTimer = 0;

    void Start()
    {
        if (gameObject.activeInHierarchy)
        {
            rb = GetComponent<Rigidbody>();
            //linkedArmController = robotArm.GetComponent<ArmController>();
            startingPoint = transform.position;

            if (MovePointRoutine)
            {
                tCurrentWaypoint = firstMovePoint;
                mCurrentWaypoint = firstMovePoint.GetComponent<MovePoint>();
                tNextWaypoint = mCurrentWaypoint.nextPoint;
                mNextWaypoint = tNextWaypoint.GetComponent<MovePoint>();
            }

            if (ForceToOnePoint)
            {
                if (gameObject.GetComponent<PIDController>() != null)
                {
                    priorFrameTargetPosition = firstMovePoint.position;
                    priorFrameTargetRotation = firstMovePoint.rotation;
                }
            }

            worldManager = GetWorldManager();

            transform.SetPositionAndRotation(firstMovePoint.position, firstMovePoint.rotation);
        }
    }

    private WorldManager GetWorldManager()
    {
        if (robotConnection != null)
        {
            return robotConnection.GetComponent("WorldManager") as WorldManager;
        }
        else
        {
            UnityEngine.Object[] listAllGameObjects = Resources.FindObjectsOfTypeAll(typeof(GameObject));
            foreach (UnityEngine.Object obj in listAllGameObjects)
            {
                if (obj is GameObject)
                {
                    if (((GameObject)obj).GetComponent("WorldManager") != null)
                    {
                        robotConnection = obj as GameObject;
                        return robotConnection.GetComponent("WorldManager") as WorldManager;
                    }
                }
            }
        }
        return null;
    }

    void Update () {
    }

    void FixedUpdate()
    {
        if (gameObject.activeInHierarchy)
        {
            //Should change this to use the Factory pattern, or perhaps use delegate functions, so I don't need to run through the If's every update
            if (MovePointRoutine)
            {
                TraceMovePoints();
            }
            else if (LeapLeftHandRoutine)
            {
                FollowLeftHand();
            }
            else if (OculusRoutine)
            {
                FollowOculusHand();
            }
            else if (FollowOnePoint)
            {
                FollowOnePointRoutine();
            }
            else if (ForceToOnePoint)
            {
                ForceToOnePointRoutine();
            }
            
        }
    }


    private void TraceMovePoints()
    {
        Vector3 newPosition;
        Quaternion newRotation;

        /*Steps:
         * 1: see if durationTimer is longer than the time for the current waypoint (for movement + pause)
         * 2a: If not, then keep moving or stay paused.
         * 2b: If it is, reset durationTimer and set new currentPoint and nextPoint and start moving
        Can use transform.position = Vector3.MoveTowards(...), as that will let me set a distance amount rather than a time amount
        */
        if (durationTimer <= mCurrentWaypoint.pauseDuration)
        {//Pause at this spot, but move jaws if needed
            float durationFraction = durationTimer / mNextWaypoint.travelToDuration;
            float currentWaypointStartGrasperOpening = mCurrentWaypoint.jawOpeningBeforePause;
            float currentWaypointEndPauseOpening = mCurrentWaypoint.jawOpeningAfterPause;
            float newGrasperAngle = Mathf.Lerp(currentWaypointStartGrasperOpening, currentWaypointEndPauseOpening, durationFraction);
            UpdateGrasperOpening(newGrasperAngle);
        }
        else if (durationTimer <= mNextWaypoint.travelToDuration + mCurrentWaypoint.pauseDuration)
        {//If the arm should be moving between waypoints
            float durationFraction = (durationTimer - mCurrentWaypoint.pauseDuration) / mNextWaypoint.travelToDuration;

            newPosition = Vector3.Lerp(tCurrentWaypoint.transform.position, tNextWaypoint.transform.position, durationFraction);
            newRotation = Quaternion.Lerp(tCurrentWaypoint.transform.rotation, tNextWaypoint.transform.rotation, durationFraction);

            float currentWaypointGrasperOpening = mCurrentWaypoint.jawOpeningAfterPause;
            float nextWaypointGrasperOpening = mNextWaypoint.jawOpeningBeforePause;
            float newGrasperAngle = Mathf.Lerp(currentWaypointGrasperOpening, nextWaypointGrasperOpening, durationFraction);

            rb.MovePosition(newPosition);
            rb.MoveRotation(newRotation);
            UpdateGrasperOpening(newGrasperAngle);
        }
        else
        {//If the timer has passed the durations, reset it and switch to the next waypoint
            //update the mCurrent, mNext, tCurrent, and tNext objects. Reset the durationTimer
            durationTimer = 0;
            tCurrentWaypoint = mCurrentWaypoint.nextPoint;
            mCurrentWaypoint = tCurrentWaypoint.GetComponent<MovePoint>();

            tNextWaypoint = mCurrentWaypoint.nextPoint;
            mNextWaypoint = tNextWaypoint.GetComponent<MovePoint>();
        }

        durationTimer += Time.deltaTime;
    }

    private void FollowLeftHand()
    {
        rb.MovePosition(PositionOfLeftThumbFiner());
        rb.MoveRotation(GetAdjustedLeftHandThumbRotation());
    }
    private void FollowRightHand()
    {
        rb.MovePosition(worldManager.GetLeapRightHandPalm().transform.position);
    }

    /// <summary>
    /// Move in the direction of the Oculus hand sensor - the movement is lerped across 5 iterations to improve smoothness
    /// </summary>
    private void FollowOculusHand()
    {
        Vector3 intermediate = Vector3.Lerp(transform.position, firstMovePoint.position, 0.5f);
        Quaternion newRotation = Quaternion.Lerp(transform.rotation, firstMovePoint.rotation, 0.5f);

        //Update the displayed goal location
        TryingToReach = intermediate;

        float jawOpening = firstMovePoint.GetComponent<PlayerHand>().triggerAmount; //Need to refactor this to remove GetComponent
        UpdateGrasperOpening(jawOpening);
        rb.MovePosition(intermediate);
        rb.MoveRotation(newRotation);
    }

    /// <summary>
    /// Follow whatever movepoint is set
    /// </summary>
    private void FollowOnePointRoutine()
    {
        rb.MovePosition(firstMovePoint.position);
        rb.MoveRotation(firstMovePoint.rotation);
    }

    /// <summary>
    /// Use forces to move the destination to whatever movepoint is set. This is like FollowOnePoint, except uses forces
    /// </summary>
    private void ForceToOnePointRoutine()
    {
        //Some issue makes tracking very bad at startup; the destination point flies off; use this to avoid that problem. It may be caused by 
        //the PuppetMaster or puppetmaster's Prop handling, which would make it difficult or impossible to truly solve without modifying those components.
        if (Time.fixedTime < 0.1f)
        {
            transform.SetPositionAndRotation(firstMovePoint.position, firstMovePoint.rotation);

            priorFrameTargetPosition = firstMovePoint.position;
            return;
        }

        //This was made with help from the guide at http://digitalopus.ca/site/pd-controllers/

        //Add force to move position:
        Vector3 targetVelocity = (firstMovePoint.position - priorFrameTargetPosition) / Time.fixedDeltaTime;
        Vector3 ForceToReachPosition = PositionPID.BackwardsPIDPosition(rb.position, firstMovePoint.position, rb.velocity,  targetVelocity);
        TransformMagnitudeFromPID = ForceToReachPosition.magnitude;
        rb.AddForce(ForceToReachPosition * TranslationForceMultiplier);
        //Finally, update the prior position
        priorFrameTargetPosition = firstMovePoint.position;

        //Add force to move rotation:
        Vector3 ForceToReachRotation = RotationPID.BackwardsPIDRotation(rb.rotation, firstMovePoint.rotation, rb.angularVelocity); //currentRotationalVelocity);
        Quaternion rotInertia2World = rb.inertiaTensorRotation * rb.rotation;
        Vector3 adjustedForceToReachRotation = Quaternion.Inverse(rotInertia2World) * ForceToReachRotation;
        adjustedForceToReachRotation.Scale(rb.inertiaTensor);
        adjustedForceToReachRotation = rotInertia2World * adjustedForceToReachRotation;
        RotateMagnitudeFromPID = adjustedForceToReachRotation.magnitude;
        rb.AddTorque(adjustedForceToReachRotation * RotationForceMultiplier);
    }


    private Vector3 AveragePositionOfLeftThumbAndIndexFiner()
    {
        GameObject lefthand = worldManager.GetLeapLeftHand();
        Vector3 average = Vector3.Lerp(lefthand.transform.Find("index").Find("bone3").position, lefthand.transform.Find("thumb").Find("bone3").position, 0.5f);
        return average;
    }
    private Vector3 PositionOfLeftThumbFiner()
    {
        GameObject lefthand = worldManager.GetLeapLeftHand();
        return lefthand.transform.Find("thumb").Find("bone3").position;
    }

        /// <summary>
        /// Leap's rotation of the index finger is awkward; this will give a rotation that is better adjusted.
        /// </summary>
        /// <returns></returns>
        private Quaternion GetAdjustedLeftHandThumbRotation()
    {
        Quaternion adjustedRotation = worldManager.GetLeapLeftHand().transform.Find("thumb").Find("bone3").rotation * Quaternion.AngleAxis(90, Vector3.right); //Tested and confirmed; 90 and Vector3.right will cause it to point along the thumb
        adjustedRotation = adjustedRotation * Quaternion.AngleAxis(180, Vector3.up); //Tested and confirmed; 90 and Vector3.right will cause the arm to rotate so the end effector faces forward when the thumbnail faces forward.
        return adjustedRotation;
    }


    private void UpdateGrasperOpening(float inOpening)
    {
        //linkedArmController.virtualServoSet.grasperController.MoveToPercentage(inOpening);
    }
}