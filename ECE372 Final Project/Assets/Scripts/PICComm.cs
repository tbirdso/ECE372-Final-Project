using UnityEngine;
using System.Collections;

public class PICComm : BasicArdComm
{
	public int cur_count = 30;
	public const int delay_count = 50;
	public char next_send_val = 'x';

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}

	// Update is called once per frame
	protected override void Update () {

		if (cur_count++ > delay_count) {
			serialController.SendSerialMessage (next_send_val.ToString());
			Debug.Log ("Sent " + next_send_val);

			next_send_val = (next_send_val == 'x') ? 'z' : 'x';

			cur_count = 0;
		}

		//---------------------------------------------------------------------
		// Receive data
		//---------------------------------------------------------------------

		message = serialController.ReadSerialMessage();

		if (message == null) {
			Debug.Log ("No messages today.");
			return;
		}

		// Check if the message is plain data or a connect/disconnect event.
		if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_CONNECTED))
			Debug.Log("Connection established");
		else if (ReferenceEquals(message, SerialController.SERIAL_DEVICE_DISCONNECTED))
			Debug.Log("Connection attempt failed or disconnection detected");
		else
			Debug.Log("Message arrived: " + message.ToString());

		Debug.Log ("message: " + base.message);


	}
}



