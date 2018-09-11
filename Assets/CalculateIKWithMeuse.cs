using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

public class CalculateIKWithMeuse : MonoBehaviour {

    //This is from http://meuse.co.jp/unity/unity%E3%81%A7%E3%82%A2%E3%83%BC%E3%83%A0%E3%83%AD%E3%83%9C%E3%83%83%E3%83%88ik/ 
    //which is demo'd at https://www.youtube.com/watch?v=9DqRkLQ5Sv8

    public GameObject target;
    private Transform tTarget;

    /*
    public Slider S_Slider;    //PositionX min=4 max=12
    public Slider L_Slider;    //PositionY min=-4 max=4
    public Slider U_Slider;    //PositionZ min=4 max=12
    public Slider R_Slider;    //RotationX min=-90 max=90
    public Slider B_Slider;    //RotationY min=-90 max=90
    public Slider T_Slider;    //RotationZ min=-90 max=90
    */
    public GameObject DOF1;
    public GameObject DOF2;
    public GameObject DOF3;
    public GameObject DOF4;
    public GameObject DOF5;
    public GameObject DOF6;
    public float DOF1Angle;
    public float DOF2Angle;
    public float DOF3Angle;
    public float DOF4Angle;
    public float DOF5Angle;
    public float DOF6Angle;
    public static double[] theta = new double[6];    //angle of the joints
    public float ThisShouldBeVisible = 0;

    private float L1, L2, L3, L4, L5, L6;    //arm length in order from base
    private float C3;

    // Use this for initialization
    void Start()
    {
        theta[0] = theta[1] = theta[2] = theta[3] = theta[4] = theta[5] = 0.0;
        L1 = 0.25f;
        L2 = 0.5f;
        L3 = 3.0f;
        L4 = 4.0f;
        L5 = 0.5f;
        L6 = 1.0f;
        C3 = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        tTarget = target.transform;

        float px, py, pz;
        float rx, ry, rz;
        float ax, ay, az, bx, by, bz;
        float asx, asy, asz, bsx, bsy, bsz;
        float p5x, p5y, p5z;
        float C1, C23, S1, S23;

        px = tTarget.position.x;
        py = tTarget.position.y;
        pz = tTarget.position.z;
        rx = tTarget.rotation.x;
        ry = tTarget.rotation.y;
        rz = tTarget.rotation.z;

        ax = Mathf.Cos(rz * 3.14f / 180.0f) * Mathf.Cos(ry * 3.14f / 180.0f);
        ay = Mathf.Sin(rz * 3.14f / 180.0f) * Mathf.Cos(ry * 3.14f / 180.0f);
        az = -Mathf.Sin(ry * 3.14f / 180.0f);

        p5x = px - (L5 + L6) * ax;
        p5y = py - (L5 + L6) * ay;
        p5z = pz - (L5 + L6) * az;

        theta[0] = Mathf.Atan2(p5y, p5x);

        C3 = (Mathf.Pow(p5x, 2) + Mathf.Pow(p5y, 2) + Mathf.Pow(p5z - L1, 2) - Mathf.Pow(L2, 2) - Mathf.Pow(L3 + L4, 2))
            / (2 * L2 * (L3 + L4));
        theta[2] = Mathf.Atan2(Mathf.Pow(1 - Mathf.Pow(C3, 2), 0.5f), C3);

        float M = L2 + (L3 + L4) * C3;
        float N = (L3 + L4) * Mathf.Sin((float)theta[2]);
        float A = Mathf.Pow(p5x * p5x + p5y * p5y, 0.5f);
        float B = p5z - L1;
        theta[1] = Mathf.Atan2(M * A - N * B, N * A + M * B);

        C1 = Mathf.Cos((float)theta[0]);
        C23 = Mathf.Cos((float)theta[1] + (float)theta[2]);
        S1 = Mathf.Sin((float)theta[0]);
        S23 = Mathf.Sin((float)theta[1] + (float)theta[2]);

        bx = Mathf.Cos(rx * 3.14f / 180.0f) * Mathf.Sin(ry * 3.14f / 180.0f) * Mathf.Cos(rz * 3.14f / 180.0f)
            - Mathf.Sin(rx * 3.14f / 180.0f) * Mathf.Sin(rz * 3.14f / 180.0f);
        by = Mathf.Cos(rx * 3.14f / 180.0f) * Mathf.Sin(ry * 3.14f / 180.0f) * Mathf.Sin(rz * 3.14f / 180.0f)
            - Mathf.Sin(rx * 3.14f / 180.0f) * Mathf.Cos(rz * 3.14f / 180.0f);
        bz = Mathf.Cos(rx * 3.14f / 180.0f) * Mathf.Cos(ry * 3.14f / 180.0f);

        asx = C23 * (C1 * ax + S1 * ay) - S23 * az;
        asy = -S1 * ax + C1 * ay;
        asz = S23 * (C1 * ax + S1 * ay) + C23 * az;
        bsx = C23 * (C1 * bx + S1 * by) - S23 * bz;
        bsy = -S1 * bx + C1 * by;
        bsz = S23 * (C1 * bx + S1 * by) + C23 * bz;

        theta[3] = Mathf.Atan2(asy, asx);
        theta[4] = Mathf.Atan2(Mathf.Cos((float)theta[3]) * asx + Mathf.Sin((float)theta[3]) * asy, asz);
        theta[5] = Mathf.Atan2(Mathf.Cos((float)theta[3]) * bsy - Mathf.Sin((float)theta[3]) * bsx,
            -bsz / Mathf.Sin((float)theta[4]));
    }

    private void UpdateChildren()
    {

    }

}
