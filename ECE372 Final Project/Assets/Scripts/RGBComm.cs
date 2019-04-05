/**
 * Ardity (Serial Communication for Arduino + Unity)
 * Author: Daniel Wilches <dwilches@gmail.com>
 *
 * This work is released under the Creative Commons Attributions license.
 * https://creativecommons.org/licenses/by/2.0/
 */

using UnityEngine;
using System.Collections;

/**
 * Sample for reading using polling by yourself, and writing too.
 */
public class RGBComm : BasicArdComm
{
	// Initialization
	protected override void Start()
	{
		base.Start ();

		Debug.Log("Press A or Z to execute some actions");
	}

	// Executed each frame
	protected override void Update()
	{
		//---------------------------------------------------------------------
		// Send data
		//---------------------------------------------------------------------

		// If you press one of these keys send it to the serial device. A
		// sample serial device that accepts this input is given in the README.
		if (Input.GetKeyDown(KeyCode.A))
		{
			Debug.Log("Sending #");
			serialController.SendSerialMessage("#");
		}

		if (Input.GetKeyDown(KeyCode.Z))
		{
			Debug.Log ("Sending #FF0000");
			serialController.SendSerialMessage ("#FF0000#");
		}

		if (Input.GetKeyDown (KeyCode.J)) {
			Debug.Log ("Sending #00FFFF");
			serialController.SendSerialMessage ("#00FFFF#");
		}

		if (Input.GetKeyDown (KeyCode.K)) {
			Debug.Log ("Sending #FFFF00");
			serialController.SendSerialMessage ("#FFFF00");
		}

		if (Input.GetKeyDown (KeyCode.L)) {
			Debug.Log ("Let there be light");
			serialController.SendSerialMessage ("#FFFFFF");
		}

		//---------------------------------------------------------------------
		// Receive data
		//---------------------------------------------------------------------

		base.Update ();
	}

	public string makeByteStr(byte val) {
		char[] cArr = new char[2];
		cArr [0] = (char)val;
		cArr [1] = '\0';
		return cArr.ToString ();
	}
}
