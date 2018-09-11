using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveFromForce : MonoBehaviour {
    public GameObject target;
    public float multiplier;

    private tntRigidBody tntRigid;
    public Vector3 direction;


	// Use this for initialization
	void Start () {
        tntRigid = gameObject.GetComponent<tntRigidBody>();
    }
	
	// Update is called once per frame
	void Update () {
        direction = target.transform.position - gameObject.transform.position;
        //direction = Vector3.up;

        tntRigid.AddForce(direction * multiplier, ForceMode.Impulse);
	}
}
