/* By Tom Birdsong
 * Last Updated 3/2/2019
 * 
 * Using Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using System.Collections;
using UnityEngine;

public class BasicArdComm : MonoBehaviour {

	public SerialController serialController;
	protected string message;

	// Use this for initialization
	protected virtual void Start () {
		serialController = GameObject.Find("SerialController").GetComponent<SerialController>();
		
	}
	
	// Update is called once per frame
	protected virtual void Update () {

		//---------------------------------------------------------------------
		// Receive data
		//---------------------------------------------------------------------

		message = serialController.ReadSerialMessage();

		if (message == null)
			return;

		// Check if the message is plain data or a connect/disconnect event.
		if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
			Debug.Log("Connection established");
		else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
			Debug.Log("Connection attempt failed or disconnection detected");
		else
			Debug.Log("Message arrived: " + message.ToString());

	}
}
