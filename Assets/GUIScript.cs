using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIScript : MonoBehaviour {

    public GameObject robotArm;
    public string testString;
    public Text armPositionReport;

    private ArmController testMovements;

    // Use this for initialization
    void Start () {
        testMovements = robotArm.GetComponent<ArmController>();
        armPositionReport = GameObject.Find("ArmPositionReport").GetComponentInChildren<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        armPositionReport.text = testMovements.ConnectionStatus;
        //testString = robotArm.GetComponent<ArmController>().SerialNumberText;
    }
}
