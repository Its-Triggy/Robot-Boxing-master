using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PIDController : MonoBehaviour {
    //Implementing from the post https://forum.unity.com/threads/rigidbody-lookat-torque.146625/#post-1005645
    //Read the full post before applying and editing further

    public float Kp = 1; //Proportional gain - is basically the magnitude
    public float Ki = 0; //Integral gain - is good for counteracting a variable amount of drag
    public float Kd = 0.1f; // Derivitive gain - is like a damper, it's the measure of how quickly we are already approaching the target, and it slows the object as it gets closer to the target and reduces overshoots.
    /*To tune the PID: 
        1: Increase the P gain until the response to a disturbance is steady oscillation.
        2: Increase the D gain until the the oscillations go away (i.e. it's critically damped).
        3: Repeat steps 2 and 3 until increasing the D gain does not stop the oscillations.
        4: Set P and D to the last stable values.
        5: Increase the I gain until it brings you to the setpoint with the number of oscillations desired (normally zero but a quicker response can be had if you don't mind a couple oscillations of overshoot)
        If the oscillations grow bigger and bigger, I need to reduce P. If the thing begins to jitter/chatter (or vibrate faster than the oscillations), reduce the D gain.
    */
    private float prevError;
    private float P, I, D;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

    public PIDController(float pFactor, float iFactor, float dFactor)
    {
        this.Kp = pFactor;
        this.Ki = iFactor;
        this.Kd = dFactor;
    }

    public float GetOutput(float currentError, float dt)
    {
        P = currentError;
        I += P * dt;
        D = (P - prevError) / dt;
        prevError = currentError;

        return P * Kp + I * Ki + D * Kd;
    }


    //To use a PID to calculate force, it's far better to use a variation termed "backwards" PID, as explained at http://digitalopus.ca/site/pd-controllers/ (which is the inspiration and a guide for the code)
    public Vector3 BackwardsPIDPosition(Vector3 currentPosition, Vector3 desiredPosition, Vector3 currentVelocity,  Vector3 desiredVelocity)
    {
        //Based on http://digitalopus.ca/site/pd-controllers/
        float dt = Time.fixedDeltaTime;
        float g = 1 / (1 + Kd * dt + Kp * dt * dt);
        float ksg = Kp * g;
        float kdg = (Kd + Kp * dt) * g;
        Vector3 Pt0 = currentPosition;
        Vector3 Vt0 = currentVelocity;
        Vector3 F = (desiredPosition - Pt0) * ksg + (desiredVelocity - Vt0) * kdg;
        //rigidbody.AddForce(F);
        return F;
    }

    
    public Vector3 BackwardsPIDRotation(Quaternion currentRotation, Quaternion desiredRotation, Vector3 currentAngularVelocity) //, Vector3 desiredVelocity)//Vector3 currentVelocity, Vector3 Pdes, Vector3 Vdes)
    {
        //Based on http://digitalopus.ca/site/pd-controllers/
        //Quaternion desiredRotation = Quaternion.Euler(45f, 45f, 34f);
        //kp = (6f * frequency) * (6f * frequency) * 0.25f;
        //kd = 4.5f * frequency * damping;
        float dt = Time.fixedDeltaTime;
        float g = 1 / (1 + Kd * dt + Kp * dt * dt);
        float ksg = Kp * g;
        float kdg = (Kd + Kp * dt) * g;
        Vector3 x;
        float xMag;
        Quaternion q = desiredRotation * Quaternion.Inverse(currentRotation);
        q.ToAngleAxis(out xMag, out x);
        x.Normalize();
        x *= Mathf.Deg2Rad;
        Vector3 pidv = Kp * x * xMag - Kd * currentAngularVelocity;

        return pidv;
    }
    
    


        /*
         * Another version of a PID controller, and a file with a usage example, is at: https://forum.unity.com/threads/pid-controller.68390/
         * The code is:
          public class PID {
        public float pFactor, iFactor, dFactor;

        float integral;
        float lastError;


        public PID(float pFactor, float iFactor, float dFactor) {
            this.pFactor = pFactor;
            this.iFactor = iFactor;
            this.dFactor = dFactor;
        }


        public float Update(float setpoint, float actual, float timeFrame) {
            float present = setpoint - actual;
            integral += present * timeFrame;
            float deriv = (present - lastError) / timeFrame;
            lastError = present;
            return present * pFactor + integral * iFactor + deriv * dFactor;
        }
    }
         * 
         * 
         * 
         */



    }
