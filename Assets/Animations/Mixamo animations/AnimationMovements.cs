using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMovements : MonoBehaviour {
    public Animator animator;
    public float inputX;
    public float inputY;


	// Use this for initialization
	void Start () {
        //Get animator
        animator = this.gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        bool tempY = Input.GetKey(KeyCode.Alpha1);
        if (tempY)
        {
            inputY = 1f;
        }
        else
        {
            inputY = 0f;
        }


        animator.SetFloat("inputY", inputY);
	}
}
