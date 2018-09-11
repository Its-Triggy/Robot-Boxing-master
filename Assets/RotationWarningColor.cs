using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script changes the object's color when it approaches or exceeds its rotation limit.
/// </summary>
public class RotationWarningColor : MonoBehaviour {
    private Renderer mainRenderer;
    /// <summary>
    /// Reference to the virtual servo - naming it with "...Ref" to make it clear that the virtualServo does not really belong to this object.
    /// </summary>
    private VirtualServo virtualServoRef;
    private Material originalColor;
    public Material warningColor;
    /// <summary>
    /// Any additional child objects that should have the same color change
    /// </summary>
    public GameObject[] additionalMeshes;
    private Renderer[] additionalMeshesRenderers;


    private tntHingeLink hingeLink = null;

    // Use this for initialization
    void Start () {
		if (GetComponentInParent<tntHingeLink>() != null)
        {
            hingeLink = GetComponentInParent<tntHingeLink>();
        }
        if (GetComponentInParent<VirtualServo>() != null)
        {
            virtualServoRef = GetComponentInParent<VirtualServo>();
        }

        //instructions for getting/setting the shader are at https://docs.unity3d.com/ScriptReference/Material-shader.html (though it 
        // doesn't include some aspects I want to use to make it more automatic)

        if (additionalMeshes.Length > 0)
        {
            additionalMeshesRenderers = new Renderer[additionalMeshes.Length];

            for (int i = 0; i < additionalMeshes.Length; ++i)
            {
                additionalMeshesRenderers[i] = additionalMeshes[i].GetComponent<Renderer>();
            }
        }

        //If this object has a mesh render, use it as the primary mesh and as the source of the default material. Otherwise, use the first 'additional' mesh
        if (gameObject.GetComponent<Renderer>() != null)
        {
            mainRenderer = gameObject.GetComponent<Renderer>();
            originalColor = mainRenderer.material;
        }
        else
        {
            if (additionalMeshes.Length > 0)
            {
                mainRenderer = additionalMeshesRenderers[0];
                originalColor = mainRenderer.material;
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        //This code only affects visual aspects, so it is only needed on frames, rather than physics steps.

        if (warningColor == null)
        {
            Debug.Log("A Rotation Warning Color script is on object " + gameObject.name + " but the warning material has not been set. Please either remove the script from this object, or set the warning material.");
            return;
        }

        //WARNING - this code may not appropriately handle cases where the joint has rotated 360 degrees and thus returns to 0, 
        //or rotations beyond -180 or +180 which may be returned as their complementary form with an opposite sign. But, such cases may be
        //handled intrinsically, as the VirtualServo calculations should give an appropriate microsecond command.
        if (hingeLink != null && virtualServoRef != null)
        {
            //if the angle tracked in VirtualServo is beyond the limits of the hinge, change it's color to display the warning.
            if (virtualServoRef.CurrentRotation.asDegree < hingeLink.m_dofData[0].m_limitLow || virtualServoRef.CurrentRotation.asDegree > hingeLink.m_dofData[0].m_limitHigh)
            {
                mainRenderer.material = warningColor;

                //Also apply the change to any child components that were added to the warning list.
                if (additionalMeshesRenderers.Length > 0)
                {
                    for (int i = 0; i < additionalMeshesRenderers.Length; ++i)
                    {
                        additionalMeshesRenderers[i].material = warningColor;
                    }
                }
            }
            else
            {
                //Keep the shader at its original value otherwise. This implementation checks to see if the shader needs to be reset before invoking it, rather than always
                //invoking a reset, because the invocation may consume more resources.
                if (mainRenderer.material != originalColor)
                {
                    mainRenderer.material = originalColor;

                    //Also apply the change to any child components that were added to the warning list.
                    if (additionalMeshesRenderers.Length > 0)
                    {
                        for (int i = 0; i < additionalMeshesRenderers.Length; ++i)
                        {
                            additionalMeshesRenderers[i].material = originalColor;
                        }
                    }
                }
            }
        }
	}
}
