/// <summary>
/// Intention: TestMovement class (which contains the user-set connectons and offsets) should 
/// tell VirtualServoSet to build the servos; TestMovements holds that reference for it's VirtualServoSet obect.
/// VirtualServoSet then builds and services the individual servos.
/// </summary>
/// 

/* Copyright (C) 2017 Chris Edwards - All Rights Reserved
 * Unauthorized copying of this file is prohibited without permission.
 * Proprietary and confidential
 * Written by Chris Edwards <chris45215@gmail.com>, August 2017
 */

public class VirtualServo : DOF
{

    //public Dimension dimension;
    //public AngleSet<int, AngleUtils.AngleType> currentRotation;
    private ServoAngle pCurrentRotation;
    public UnityEngine.Quaternion localQuat;
    public float showCurrentRotation = 0;
    public float showCurrentMicrosecond = 0;

    public UnityEngine.GameObject forcedRelativeAngle;

    public float minAngle;
    public float minMicrosecond;
    public float neutralAngle;
    public float maxAngle;
    public float maxMicrosecond;

    private float tempMicrosecondCalc = 0;


    /// <summary>
    /// the game object that the script is attached to
    /// </summary>
    private UnityEngine.GameObject dofObject;


    public ServoAngle CurrentRotation
    {
        get
        {
            return pCurrentRotation;
        }
        set
        {
            pCurrentRotation = value;
        }
    }

    public void FixedUpdate()
    {
        localQuat = gameObject.transform.rotation;
        showCurrentRotation = pCurrentRotation.asDegree;

        tempMicrosecondCalc = pCurrentRotation.asMicrosecond;
        if (reverseMovement)
        {
            tempMicrosecondCalc = AngleUtils.CalculateReverseServoMicrosecond(this);
        }
        //showCurrentMicrosecond = pCurrentRotation.asMicrosecond;
        showCurrentMicrosecond = tempMicrosecondCalc;
    }

    public VirtualServo()
    {
        pCurrentRotation = new ServoAngle(this);
    }

    public void Awake()
    {
        dofObject = transform.parent.gameObject;
        pCurrentRotation = new ServoAngle(this);
        localQuat = gameObject.transform.rotation;
    }

    public VirtualServo(byte inConnectionIndex, int inOffset)
    {
        ConnectionIndex = inConnectionIndex;
        angleOffset = inOffset;
        dofObject = transform.parent.gameObject;
    }

    

    public class ServoAngle
    {
        public float asMicrosecond;
        public float asDegree;
        public enum AngleType { Microsecond, Degree };
        public VirtualServo parent;


        public ServoAngle(VirtualServo inParent)
        {
            parent = inParent;
        }



        public ServoAngle(VirtualServo inParent, AngleType inType, float degreesOffset)
        {
            if (inType == AngleType.Degree)
            {
                parent = inParent;
                float adjustedAngle = AngleUtils.NormalizeDegrees(degreesOffset);
                this.asDegree = adjustedAngle;
                this.asMicrosecond = AngleToServoMicrosecond(adjustedAngle);
            }
            else
            {
                parent = inParent;
                this.asDegree = ServoMicrosecondsToAngle(degreesOffset);
                this.asMicrosecond = degreesOffset;
            }
        }


        /// <summary>
        /// Converts angle to servo microseconds
        /// </summary>
        public float AngleToServoMicrosecond(float inAngle)
        {
            //Control for cases where the Unity rotation is more than 180 degrees.
            float adjustedAngle = AngleUtils.NormalizeDegrees(inAngle);

            float checkForError = ((adjustedAngle - parent.minAngle) / (parent.maxAngle - parent.minAngle) * (parent.maxMicrosecond - parent.minMicrosecond)) + parent.minMicrosecond;
            if (checkForError < parent.minAngle)
            {
                int stopHere = 0;
            }

            //return (mapValue - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
            return ((adjustedAngle - parent.minAngle) / (parent.maxAngle - parent.minAngle) * (parent.maxMicrosecond - parent.minMicrosecond)) + parent.minMicrosecond;
        }

        /// <summary>
        /// Converts servo microseconds to angle
        /// </summary>
        public float ServoMicrosecondsToAngle(float inMicrosecond)
        {
            //return (((inMicrosecond - 6300) / 6600) * 180); //Updated to use the range 3000 - 9600 for aprox 180 degree rotation

            //return (mapValue - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
            return ((inMicrosecond - parent.minMicrosecond) / (parent.maxMicrosecond - parent.minMicrosecond) * (parent.maxAngle - parent.minAngle)) + parent.minAngle;
        }

        /*
        public static ServoAngle operator +(ServoAngle saLeft, ServoAngle saRight)
        {
            ServoAngle newAngle = new ServoAngle(AngleType.Degree, saLeft.asDegree + saRight.asDegree);
            return newAngle;
        }
        */

        public void Set(AngleType inType, float degreesOffset)
        {
            if (inType == AngleType.Degree)
            {
                float adjustedAngle = AngleUtils.NormalizeDegrees(degreesOffset);

                asDegree = adjustedAngle;
                asMicrosecond = AngleToServoMicrosecond(adjustedAngle);
            }
            else
            {
                asDegree = ServoMicrosecondsToAngle(degreesOffset);
                asMicrosecond = degreesOffset;
            }
        }
    }

}
