using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Pololu.Usc;
using Pololu.UsbWrapper;
using System.Threading;
using System.Timers;
using UnityEngine;
using UnityEngine.UI;
using Assets;
/* Copyright (C) 2017 Chris Edwards - All Rights Reserved
 * Unauthorized copying of this file is prohibited without permission.
 * Proprietary and confidential
 * Written by Chris Edwards <chris45215@gmail.com>, July 2017
 */

public class ArmController : MonoBehaviour
{
    public string ConnectionStatus;
    public bool activateMovement;
    public bool hideRobot;
    private Usc usc = null;
    public VirtualServoSet virtualServoSet;
    private JointFeedback jointFeedback;

    public bool OneUpdate;
    public ushort ServoMoveSpeed = 60;
    [Tooltip("If using PuppetMaster, then collisions between the robot arm and the puppet avatar should be ignored.")]
    public bool UsingPuppetMaster;

    private List<int> ServoConnectionIndexes = new List<int>();


    public ArmController()
    {
    }

    /// <summary>
    /// Awake is always called first, before Start()
    /// </summary>
    public void Awake()
    {
        virtualServoSet = gameObject.GetComponents<VirtualServoSet>().First(); //gameObject.AddComponent(typeof(VirtualServoSet)) as VirtualServoSet;
    }

    /// <summary>
    /// Start is called after Awake but before the first frame.
    /// </summary>
    void Start()
    {
        GameObject robotConnection = GameObject.Find("RobotConnection");
        usc = robotConnection.GetComponent<WorldManager>().GetUsc();

        virtualServoSet.BuildVirtualServoSet(gameObject);

        if (hideRobot)
        {
            HideRobot();
        }

        PutServoIndexesInList();
        if (gameObject.activeInHierarchy) { SetServoSpeeds(); }

        if (UsingPuppetMaster)
        {
            Physics.IgnoreLayerCollision(8, 9);
            Physics.IgnoreLayerCollision(10, 8);
            Physics.IgnoreLayerCollision(10, 9);
        }
    }


    void FixedUpdate()
    {
        //Ensure that the script isn't run on an arm that is supposed to be turned off.
        if (!gameObject.activeInHierarchy)
        {
            return;
        }

        //Find and set the relative angles of the servos - calling this here to ensure it is always executed immediately before the
        //real servos are set.
        virtualServoSet.RecordRelativeAngles();

        if (usc == null)
        {
            // Display a message in the position box
            ConnectionStatus = "(disconnected)";

            // Try connecting to a device.
            try
            {
                Debug.LogError("Error in FixedUpdate, usc == null: ");
            }
            catch (Exception e2)
            {
                usc = null;
                Debug.LogError("Error in FixedUpdate when usc=null: " + e2.ToString());
            }
        }
        else
        {// Update the device.
            try
            {
                ConnectionStatus = "connected";
                if (activateMovement)
                {
                    MoveRobot(virtualServoSet);
                    //Handle any input for the claw
                    ControlGrasper();
                }
                else
                {
                    if (OneUpdate == true)
                    {
                        MoveRobot(virtualServoSet);
                        ControlGrasper();
                        OneUpdate = false;
                    }
                }
            }
            catch (Exception e2)
            {// If any exception occurs, log it, set usc to null, and keep trying..
                usc = null;
                Debug.LogError("Error in FixedUpdate when usc has a value: " + e2.ToString());
            }
        }
    }



    private void PutServoIndexesInList()
    {
        foreach(VirtualServo tempServo in virtualServoSet.dictVirtualServoSet.Values)
        {
            ServoConnectionIndexes.Add(tempServo.ConnectionIndex);
        }
    }

    

    #region updating
   

    private void ControlGrasper()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //Open grasper
            virtualServoSet.grasperController.OpenCommand();
        }
        if (Input.GetKey(KeyCode.LeftControl))
        {
            //Close grasper
            virtualServoSet.grasperController.CloseCommand();
        }
    }

    public void ForceGrasperOpen()
    {
        virtualServoSet.grasperController.OpenCommand();
    }
    public void ForceGrasperClosed()
    {
        if (virtualServoSet.DOF7Angle <= 0)
        {
            virtualServoSet.grasperController.CloseCommand();
        }
    }

    
    void MoveRobot(VirtualServoSet inVirtual)
    {
        int brokeHere = 0;
        try
        {
            //Send real servo to the current angle of its virtual twin.
            for (int i = 0; i < ServoConnectionIndexes.Count; i++) //probably should use virtualServoSet.dictVirtualServoSet.count instead, for the sake of clarity/consistency
            {
                float destinationInMicroseconds = 0;
                try
                {
                    AngleUtils.SetRealServoToAngle(usc, inVirtual.GetServo(i));
                }
                catch (System.OverflowException ex)
                {
                    VirtualServo.ServoAngle glitchedDestinationAngle = inVirtual.GetServo(i).CurrentRotation;
                    Debug.Log("Error in the destination angle in RunOneUpdate: servo with connection index " + i + " was told to rotate to angle " + glitchedDestinationAngle.asDegree + " or in microseconds " + destinationInMicroseconds + ". With error: " + ex.ToString());
                }
                catch (Exception ex)
                {
                    Debug.Log("Error in RunOneUpdate: " + ex.ToString());
                }
            }
            brokeHere = 1;
            //Set the grasper's opening angle as well
            if (inVirtual.grasperController.maximumOpening == 0)
            {
                Debug.LogError("The Grasper's maximum opening has been set as 0. This will produce an infinite value error, as another value is divided by this.");
            }
            float fractionOpen = Math.Abs(inVirtual.grasperController.GetGrasperOpening() / inVirtual.grasperController.maximumOpening);

            float desiredPosition = inVirtual.grasperController.fullClosedMicroseconds + (inVirtual.grasperController.microsecondRange * fractionOpen);
            byte grasperConnectionIndex = inVirtual.grasperController.ConnectionIndex;
            AngleUtils.SetRealServoToMicrosecond(usc, grasperConnectionIndex, desiredPosition, false);
        }
        catch(Exception ex)
        {
            Debug.LogError("Error running the consolidated RunOneUpdate function. " + ex.ToString());
        }
    }

    /*
    /// <summary>
    /// Averages the angles of the two arms of the grasper. They are expected to be equal, opposite angles; but that may not always be the case.
    /// </summary>
    public float GetGrasperAngle(VirtualServoSet inVirtual)
    {
     //   AngleUtils.ServoAngle leftAnglePart = new AngleUtils.ServoAngle(AngleUtils.AngleType.Degree, inVirtual.grasperController.LeftGrasper.transform.localEulerAngles.y);
     //   AngleUtils.ServoAngle rightAnglePart = new AngleUtils.ServoAngle(AngleUtils.AngleType.Degree, inVirtual.grasperController.RightGrasper.transform.localEulerAngles.y);

        float leftAnglePart = inVirtual.grasperController.LeftGrasper.transform.localEulerAngles.y;
        float rightAnglePart = inVirtual.grasperController.RightGrasper.transform.localEulerAngles.y;

        float averagedAngle = (Math.Abs(leftAnglePart) + Math.Abs(rightAnglePart)); // / 2.0f;

        if (averagedAngle < 0 || averagedAngle > 180)
        {
            Debug.LogError("Grasper angle is: " + averagedAngle);
            //Set the angle to a value within the limits.
            if (averagedAngle < 0)
            { averagedAngle = 0; }
            else
            { averagedAngle = 180; }
        }

        return averagedAngle * -1;
       // return new AngleUtils.ServoAngle(AngleUtils.AngleType.Degree, averagedAngle * -1); //Notice the -1, so clockwise opens the grasper
    }
    */

    private void SetServoSpeeds()
    {
        foreach (int connectionIndex in ServoConnectionIndexes)
        {
            usc.setSpeed((byte) connectionIndex, ServoMoveSpeed);
        }
        //Set the grasper speed to the maximum
        usc.setSpeed((byte)ServoConnectionIndexes.Last(), 100);
    }

    /// <summary>
    /// Hide all the meshes of the robot arm, using a depth-first-search to find all meshes.
    /// </summary>
    private void HideRobot()
    {
        Stack<GameObject> parts = new Stack<GameObject>();
        parts.Push(gameObject);
        while (parts.Any())
        {
            GameObject part = parts.Pop();
            //Disable any mesh renderers in the part
            if (part.GetComponent<MeshRenderer>())
            {
                part.GetComponent<MeshRenderer>().enabled = false;
            }

            //Add the part's children to the stack of objects to be checked
            //Unity's Transform implements IEnumerable to return the children, so ForEach works directly on it.
            foreach(Transform child in part.transform)
            {
                parts.Push(child.gameObject);
            }
        }
    }

    #endregion
}
