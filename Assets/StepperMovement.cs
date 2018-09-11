using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pololu.Usc;
using UnityEngine;

namespace Assets
{
    class StepperMovement : IDOFMovement
    {
        /// <summary>
        /// NOT IMPLEMENTED YET
        /// </summary>
        /// <param name="inUsc"></param>
        /// <param name="servoNumber"></param>
        /// <param name="inAngle"></param>
        public void SetRealDOFToAngle(Usc inUsc, byte servoNumber, VirtualServo.ServoAngle inAngle)
        {
            //Implement later

            if (inAngle.asMicrosecond < 0)
            {
                Debug.LogError("Error with SetRealServoToAngle for servo #" + servoNumber + ": inAngle.asMicrosecong is less than 0: " + inAngle.asMicrosecond);
            }
            
            //.................
        }
    }
}
