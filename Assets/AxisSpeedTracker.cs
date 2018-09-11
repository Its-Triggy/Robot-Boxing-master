using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisSpeedTracker : MonoBehaviour {

    private VirtualServo thisServo;
    public int iterationsToAverage = 20;
    //public VirtualServo earlierServo;

    //Track the angles from the prior 5 frames
    //private List<double> priorAngles;
    private Queue<AngleAndDuration> priorAngles = new Queue<AngleAndDuration>();

    public double highestSpeedOverIterations;

    //VirtualServoSet determines the angles between servos with ReadRelativeAngles()

    // Use this for initialization
    void Start () {
		if (gameObject.GetComponent<VirtualServo>() != null)
        {
            thisServo = gameObject.GetComponent<VirtualServo>();
        }
	}
	
	// Update is called once per frame
	void Update () {	
	}

    private void FixedUpdate()
    {
        //Allow 2 seconds for program startup before recording - to avoid a jump to a starting position.
        if (Time.time >= 2)
        {
            AngleAndDuration newEntry = new AngleAndDuration(thisServo.CurrentRotation.asDegree, Time.fixedDeltaTime);
            //If the queue has less than the required number of members, add a new member - but don't calculate speed yet
            if (priorAngles.Count < iterationsToAverage)
            {
                priorAngles.Enqueue(newEntry);
            }
            //Otherwise, calculate the speed over the fixed updates
            else
            {
                priorAngles.Dequeue();
                priorAngles.Enqueue(newEntry);
                //Get the time across the frames
                double totalTime = 0.0;
                foreach (AngleAndDuration durations in priorAngles)
                {
                    totalTime += durations.duration;
                }
                //get the difference between first and last angle
                double angleChange = newEntry.angle - priorAngles.Peek().angle;

                //If this change (as an absolute value) is greater than the highest one thus far, set that change (again as an absolute value)
                double speed = angleChange / totalTime;
                if (Mathf.Abs((float) speed) > highestSpeedOverIterations)
                {
                    highestSpeedOverIterations = Mathf.Abs((float)speed);
                }
            }
        }
    }

    /// <summary>
    /// small datastructure to store the angle of a servo and the duration of the frame in which it had that angle
    /// </summary>
    private struct AngleAndDuration
    {
        public double angle;
        public double duration;

        public AngleAndDuration(double inAngle, double inDuration)
        {
            angle = inAngle;
            duration = inDuration;
        }
    }
}
