using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeListen : MonoBehaviour {

	public PICMessageListener listener;

	// Use this for initialization
	void Start () {

		listener.ButtonMove += new PICMessageListener.ButtonHandler (MoveToButton);
		listener.JoystickMove += new PICMessageListener.JoystickHandler (MoveToJPos);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void MoveToButton(int bid, bool state) {

		Debug.Log ("moving to button: " + state);

		float new_z = (state ? -1 : 0);

		transform.position = new Vector3 (transform.position.x, transform.position.y, new_z);
	}
		

	public void MoveToJPos(int jid, Vector2 pos) {
		const int RANGE_MAX = 128;
		const float MULT = (float)4;
		const float DELTA_MIN = (float)0.01;

		float new_x = MULT*(float)(pos.x / RANGE_MAX - 0.5);
		float new_y = MULT*(float)(pos.y / RANGE_MAX - 0.5);

		if (Mathf.Abs(transform.position.x - new_x) > DELTA_MIN ||
			Mathf.Abs(transform.position.y - new_y) > DELTA_MIN) {

			Debug.Log ("Moving cube to " + pos.x + " " + pos.y);

			transform.position = new Vector3 (new_x, new_y, transform.position.z);

		}
	}
}
