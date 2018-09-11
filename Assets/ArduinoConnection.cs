using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

public class ArduinoConnection : MonoBehaviour {

    /// <summary>
    /// The port the Arduino is connected to - may need to update the connection location or the baud rate
    /// </summary>
    SerialPort arduinoPort = new SerialPort("COM3", 9600);

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
