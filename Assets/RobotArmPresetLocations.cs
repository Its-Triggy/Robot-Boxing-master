using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RobotArmStartLocations
{
    //Values have the "* 4" because the Pololu Maestro Control Center shows the microsecond values, but the
    //servos actually receive values in quarter-microsecond increments. 
    public static float startPositionDOF1 = 1500 * 4;
    public static float startPositionDOF2 = 826 * 4;
    public static float startPositionDOF3 = 1500 * 4;
    public static float startPositionDOF4 = 1500 * 4;
    public static float startPositionDOF5 = 1500 * 4;
    public static float startPositionDOF6 = 1413 * 4;
}
