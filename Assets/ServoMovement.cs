using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pololu.Usc;
using UnityEngine;

namespace Assets
{
    class ServoMovement : IDOFMovement
    {

        public void SetRealDOFToAngle(Usc inUsc, byte servoNumber, VirtualServo.ServoAngle inAngle)
        {
            if (inAngle.asMicrosecond < 0)
            {
                Debug.LogError("Error with SetRealServoToAngle for servo #" + servoNumber + ": inAngle.asMicrosecong is less than 0: " + inAngle.asMicrosecond);
            }
            inUsc.setTarget((byte)servoNumber, System.Convert.ToUInt16(inAngle.asMicrosecond));
        }
    }
}
