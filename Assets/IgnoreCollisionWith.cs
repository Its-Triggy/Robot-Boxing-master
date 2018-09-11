using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreCollisionWith : MonoBehaviour {
    public Transform ignorePiece;

	// Use this for initialization
	void Start ()
    {
        Physics.IgnoreCollision(ignorePiece.GetComponent<Collider>(), GetComponent<Collider>());
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
