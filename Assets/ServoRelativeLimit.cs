using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServoRelativeLimit : MonoBehaviour
{
    //public VirtualServo relativeServo;
    public GameObject relativeObject;
    public DOF.RotationAxis ComparisonAxis;


    /// <summary>
    /// This component will be set to match the (world) rotation of the Relative Object; use this value to designate a number of degrees offset from that angle (It's best to leave it at 0 if all components have the same starting location).
    /// </summary>
    public float ResetToDegreesOffFromTargetObject;
    public float minRelativeAngle;
    public float maxRelativeAngle;

    // Use this for initialization
    void Start()
    {
        /*
        if (relativeServo != null)
        {
            relativeObject = relativeServo.gameObject;
        }
        */
    }

    private void FixedUpdate()
    {
        //Early exit condition
        if (relativeObject == null)
        {
            return;
        }

        if (ComparisonAxis == DOF.RotationAxis.x)
        {
            if (gameObject.transform.eulerAngles.x > relativeObject.transform.eulerAngles.x + maxRelativeAngle)
            {
                Vector3 newRotation = new Vector3(relativeObject.transform.eulerAngles.x + ResetToDegreesOffFromTargetObject, relativeObject.transform.eulerAngles.y, relativeObject.transform.eulerAngles.z);
                gameObject.transform.Rotate(newRotation);
            }
            if (gameObject.transform.eulerAngles.x < relativeObject.transform.eulerAngles.x + minRelativeAngle)
            {
                Vector3 newRotation = new Vector3(relativeObject.transform.eulerAngles.x + ResetToDegreesOffFromTargetObject, relativeObject.transform.eulerAngles.y, relativeObject.transform.eulerAngles.z);
                gameObject.transform.Rotate(newRotation);
            }
        }

        if (ComparisonAxis == DOF.RotationAxis.y)
        {
            if (gameObject.transform.eulerAngles.y > relativeObject.transform.eulerAngles.y + maxRelativeAngle)
            {
                Vector3 newRotation = new Vector3(relativeObject.transform.eulerAngles.x, relativeObject.transform.eulerAngles.y + ResetToDegreesOffFromTargetObject, relativeObject.transform.eulerAngles.z);
                gameObject.transform.Rotate(newRotation);
            }
            if (gameObject.transform.eulerAngles.y < relativeObject.transform.eulerAngles.y + minRelativeAngle)
            {
                Vector3 newRotation = new Vector3(relativeObject.transform.eulerAngles.x, relativeObject.transform.eulerAngles.y + ResetToDegreesOffFromTargetObject, relativeObject.transform.eulerAngles.z);
                gameObject.transform.Rotate(newRotation);
            }
        }

        if (ComparisonAxis == DOF.RotationAxis.z)
        {
            if (gameObject.transform.eulerAngles.z > relativeObject.transform.eulerAngles.z + maxRelativeAngle)
            {
                Vector3 newRotation = new Vector3(relativeObject.transform.eulerAngles.x, relativeObject.transform.eulerAngles.y, relativeObject.transform.eulerAngles.z + ResetToDegreesOffFromTargetObject);
                gameObject.transform.Rotate(newRotation);
            }
            if (gameObject.transform.eulerAngles.z < relativeObject.transform.eulerAngles.z + minRelativeAngle)
            {
                Vector3 newRotation = new Vector3(relativeObject.transform.eulerAngles.x, relativeObject.transform.eulerAngles.y, relativeObject.transform.eulerAngles.z + ResetToDegreesOffFromTargetObject);
                gameObject.transform.Rotate(newRotation);
            }
        }


        /*
        //If this must be greater than the other DOF
        if (ThisComparedToOther == RelativeOptions.GreaterThan)
        {
            if (ComparisonAxis == DOF.RotationAxis.x)
            {
                if (gameObject.transform.eulerAngles.x < relativeObject.transform.eulerAngles.x)
                {
                    Vector3 newRotation = new Vector3(relativeObject.transform.eulerAngles.x + expectedDifference, relativeObject.transform.eulerAngles.y, relativeObject.transform.eulerAngles.z);
                    gameObject.transform.Rotate(newRotation);
                }
            }
        }
        else if (ThisComparedToOther == RelativeOptions.LessThan)
        {
            if (ComparisonAxis == DOF.RotationAxis.x)
            {
                if (gameObject.transform.eulerAngles.x < relativeObject.transform.eulerAngles.x)
                {
                    Vector3 newRotation = new Vector3(relativeObject.transform.eulerAngles.x - expectedDifference, relativeObject.transform.eulerAngles.y, relativeObject.transform.eulerAngles.z);
                    gameObject.transform.Rotate(newRotation);
                }
            }
        }
        */
    }

    // Update is called once per frame
    void Update()
    {

    }

    public enum RelativeOptions
    {
        LessThan,
        GreaterThan
    };
}
