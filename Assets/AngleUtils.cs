using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pololu.Usc;
using Pololu.UsbWrapper;

/* Copyright (C) 2017 Chris Edwards - All Rights Reserved
 * Unauthorized copying of this file is prohibited without permission.
 * Proprietary and confidential
 * Written by Chris Edwards <chris45215@gmail.com>, August 2017
 */

public class AngleUtils {

    public static ushort maxMicrosecond = 10000;
    public static ushort minMicrosecond = 2000;
    public static int microsecondRange = maxMicrosecond - minMicrosecond;
    public static int maxAngle = 90;
    public static int minAngle = -90;
    public static int angleRange = maxAngle - minAngle;

    /*
    public class ServoAngle
    {
        public float asMicrosecond;
        public float asDegree;

        public ServoAngle()
        { }

        
        
        public ServoAngle(AngleType inType, float degreesOffset)
        {
            if (inType == AngleType.Degree)
            {
                float adjustedAngle = NormalizeDegrees(degreesOffset);
                this.asDegree = adjustedAngle;
                this.asMicrosecond = AngleToServoMicrosecond(adjustedAngle);
            }
            else
            {
                this.asDegree = ServoMicrosecondsToAngle(degreesOffset);
                this.asMicrosecond = degreesOffset;
            }
        }
        
        
        public static ServoAngle operator +(ServoAngle saLeft, ServoAngle saRight)
        {
            ServoAngle newAngle = new ServoAngle(AngleType.Degree, saLeft.asDegree + saRight.asDegree);
            return newAngle;
        }
        

        public void Set(AngleType inType, float degreesOffset)
        {
            if (inType == AngleType.Degree)
            {
                float adjustedAngle = NormalizeDegrees(degreesOffset);

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
    */

    /// <summary>
    /// Sets the input degrees to a value between -180 and +180
    /// </summary>
    /// <param name="inDegrees"></param>
    /// <returns></returns>
    public static float NormalizeDegrees(float inDegrees)
    {
        float adjustedAngle = inDegrees;
        while (adjustedAngle > 180)
        {
            adjustedAngle -= 360.0f;
        }
        while (adjustedAngle < -180)
        {
            adjustedAngle += 360.0f;
        }
        return adjustedAngle;
    }

    /*
    /// <summary>
    /// Converts servo microseconds to angle
    /// </summary>
    public static float ServoMicrosecondsToAngle(VirtualServo inServo, float inMicrosecond)
    {
        //return (mapValue - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        return ((inMicrosecond - minMicrosecond) / (maxMicrosecond - minMicrosecond) * (maxAngle - minAngle)) + minAngle;
    }
    */

    /*
    /// <summary>
    /// Converts angle to servo microseconds
    /// </summary>
    public static float AngleToServoMicrosecond(VirtualServo inServo, float inAngle)
    {
        //Control for cases where the Unity rotation is more than 180 degrees.
        float adjustedAngle = NormalizeDegrees(inAngle);

        float checkForError = ((adjustedAngle - minAngle) / (maxAngle - minAngle) * (maxMicrosecond - minMicrosecond)) + minMicrosecond;
        if (checkForError < 3000)
        {
            int stopHere = 0;
        }

        //return (mapValue - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
        return ((adjustedAngle - minAngle) / (maxAngle - minAngle) * (maxMicrosecond - minMicrosecond)) + minMicrosecond;
    }
    */

    /*
    /// <summary>
    /// Primary function for communicating to the real servo controller; sets the designated servo to the input angle
    /// </summary>
    public static void SetRealServoToAngle(Usc inUsc, byte servoNumber, VirtualServo.ServoAngle inAngle, bool inReversal)
    {
        float destinationMicroseconds = 0;
        if (inReversal == false)
        {
            destinationMicroseconds = inAngle.asMicrosecond;
        }
        else
        {
            destinationMicroseconds = CalculateReversedServoMicroseconds(inAngle);
        }

        //Set the real servo to the desired angle
        if (destinationMicroseconds > minMicrosecond && destinationMicroseconds < maxMicrosecond)
        {
            inUsc.setTarget((byte)servoNumber, System.Convert.ToUInt16(destinationMicroseconds));
        }
        else
        {
            Debug.LogError("Servo number " + servoNumber + " is being told to move to invalid position " + destinationMicroseconds);
        }
    }
    */

    public static float CalculateReverseServoMicrosecond(VirtualServo inServo)
    {
        float destinationMicroseconds = 0;
        destinationMicroseconds = inServo.CurrentRotation.asMicrosecond;

        //Reverse, if needed
        if (inServo.reverseMovement)
        {
            float microsecondMidpoint = (inServo.maxMicrosecond + inServo.minMicrosecond) / 2;
            float microsecondsFromMidpoint = microsecondMidpoint - inServo.CurrentRotation.asMicrosecond;
            destinationMicroseconds = microsecondMidpoint + microsecondsFromMidpoint;
            //destinationMicroseconds = inServo.maxMicrosecond - inServo.CurrentRotation.asMicrosecond;
        }

        return destinationMicroseconds;
    }

    public static void SetRealServoToAngle(Usc inUsc, VirtualServo inServo)
    {
        float destinationMicroseconds = 0;
        /*
        if (inServo.reverseMovement == false)
        {
            destinationMicroseconds = inServo.CurrentRotation.asMicrosecond;
        }
        else
        {
            destinationMicroseconds = CalculateReversedServoMicroseconds(inServo.CurrentRotation);
        }
        */

        destinationMicroseconds = inServo.CurrentRotation.asMicrosecond;

        //Reverse, if needed
        /*
        if (inServo.reverseMovement)
        {
            float microsecondMidpoint = (inServo.maxMicrosecond + inServo.minMicrosecond) / 2;
            float microsecondsFromMidpoint = microsecondMidpoint - inServo.CurrentRotation.asMicrosecond;
            destinationMicroseconds = microsecondMidpoint + microsecondsFromMidpoint;
            //destinationMicroseconds = inServo.maxMicrosecond - inServo.CurrentRotation.asMicrosecond;
        }
        */
        if (inServo.reverseMovement)
        {
            destinationMicroseconds = CalculateReverseServoMicrosecond(inServo);
        }

        //Set the real servo to the desired angle
        if (destinationMicroseconds > inServo.minMicrosecond && destinationMicroseconds < inServo.maxMicrosecond)
        {
            inUsc.setTarget((byte)inServo.ConnectionIndex, System.Convert.ToUInt16(destinationMicroseconds));
        }
        else
        {
            Debug.LogError("Servo at connection index " + inServo.ConnectionIndex + " is being told to move to invalid position " + destinationMicroseconds);
        }
    }

    public static void SetRealServoToMicrosecond(Usc inUsc, byte servoNumber, float inMicrosecond, bool inReversal)
    {
        inUsc.setTarget((byte)servoNumber, System.Convert.ToUInt16(inMicrosecond));
    }

    /*
    /// <summary>
    /// Some of the servo commands need to be reverse, depending on if the servo is mounted for clockwise or counterclockwise rotation. This 
    /// returns the microsecond value needed for a reversal of the input ServoAngle
    /// </summary>
    public static float CalculateReversedServoMicroseconds(VirtualServo.ServoAngle inAngle)
    {
        return AngleToServoMicrosecond(-1f * inAngle.asDegree);
    }
    */

    public static float NumberRangeMap(float fromMin, float fromMax, float toMin, float toMax, float mapValue)
    {
        return (mapValue - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }
}
