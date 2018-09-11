using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pololu.Usc;

namespace Assets
{
    interface IDOFMovement
    {
        void SetRealDOFToAngle(Usc inUsc, byte servoNumber, VirtualServo.ServoAngle inAngle);

    }
}
