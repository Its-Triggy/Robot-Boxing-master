using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldPosition : MonoBehaviour {
    public Vector3 worldPosition;
    public Vector3 worldRotation;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        worldPosition = transform.position;
        worldRotation = transform.rotation.eulerAngles;
    }
}
