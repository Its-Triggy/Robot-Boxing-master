using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldRotation : MonoBehaviour {
    public float X, Y, Z;
    public float QX, QY, QZ, QW;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        QW = transform.rotation.w;
        QX = transform.rotation.x;
        QY = transform.rotation.y;
        QZ = transform.rotation.z;

        X = transform.rotation.eulerAngles.x;
        Y = transform.rotation.eulerAngles.y;
        Z = transform.rotation.eulerAngles.z;
    }
}
