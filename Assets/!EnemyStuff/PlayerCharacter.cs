using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : MonoBehaviour {

	private GUIStyle style = new GUIStyle ();
	public int health;


	void Start () {
		health = 5;
	}

	void OnGUI() {
		style.fontSize = 20;
		style.normal.textColor = Color.white;
		GUI.Label (new Rect (20, 20, 30, 30), "Health = " + health, style);
	}

	public void Hurt(int damage) {
		health -= damage;
	}

	void Update () {

	}
}





