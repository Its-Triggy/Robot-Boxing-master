using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowAI : MonoBehaviour {
	//You may consider adding a rigid body to the zombie for accurate physics simulation
	private GameObject character;
	private Vector3 charPos;
	public float obstacleRange = 2.0f;
	//This will be the zombie's speed. Adjust as necessary.
	public float wanderSpeed = 0.5f;
	public float followSpeed = 2.0f;
	private float angle;
	public float fightDistance = 1.0f;
    bool trigger = true;
	Animator anim;

	void Start ()
	{
        //At the start of the game, the zombies will find the gameobject called wayPoint.
        character = GameObject.Find("Character");
		anim = GetComponent<Animator> ();
	}

	void Update ()
	{
        
		charPos = new Vector3(character.transform.position.x, transform.position.y+0.5f, character.transform.position.z);
        follow();
        /*
        Ray ray = new Ray (transform.position, charPos - transform.position);
		Debug.DrawRay(transform.position, charPos - transform.position);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			GameObject hitObject = hit.transform.gameObject;
			PlayerCharacter PC = hitObject.GetComponent<PlayerCharacter> ();

			if (PC != null) {
				follow ();
			}
			else {
                
				wander ();
			}
		} else {
			wander ();
		}
        */
	}
	void wander () {
		anim.Play ("Jog");
        transform.Translate (0, 0, wanderSpeed * Time.deltaTime);

		Ray ray = new Ray (transform.position, transform.forward);
		RaycastHit hit;
		if (Physics.SphereCast (ray, 0.75f, out hit) && hit.distance < obstacleRange) 
        {
			angle = Random.Range (-110, 110);
			transform.Rotate (0, angle, 0);

		}
	}
	void follow(){
        
		//Here, the boxer will follow the Character.
        if (Vector3.Magnitude(transform.position - charPos) > fightDistance) 
        {
            trigger = true;
            anim.Play ("Jog");
			transform.Translate (0, 0, followSpeed * Time.deltaTime);
            //transform.position = Vector3.MoveTowards(transform.position, wayPointPos, speed * Time.deltaTime);

		}
        //If out of range, checks checks if the enemy JUST entered range
        else if(trigger == true)
        {
			anim.Play ("Idle");
            trigger = false;
		}

		angle = Mathf.Atan2 ((transform.position.x - charPos.x), (transform.position.z - charPos.z));
		angle *= 360 / (2 * 3.1415f);

		angle = -(180 - angle);
		transform.eulerAngles = new Vector3 (0, angle, 0);
	}
}