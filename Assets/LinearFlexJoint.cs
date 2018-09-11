using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinearFlexJoint : MonoBehaviour {
    public Transform target;
    private Rigidbody rb;
    private Vector3 heading;

	// Use this for initialization
	void Start () {
        rb = transform.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        heading = target.position - transform.position;
        rb.AddTorque(Vector3.Cross(heading, rb.velocity), ForceMode.Force);
    }
}
