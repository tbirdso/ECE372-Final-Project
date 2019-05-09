using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeListen : MonoBehaviour {

	public PICMessageListener listener;
	public float SCALE_X;
	public float SCALE_Y;

	// Use this for initialization
	void Start () {

		listener.ButtonMove += new PICMessageListener.ButtonHandler (MoveToButton);
		listener.JoystickMove += new PICMessageListener.JoystickHandler (MoveToJPos);

	}

	public void MoveToButton(int bid, bool state) {

		float new_z = ((bid=='1') ? 1 : Mathf.Abs(transform.position.z - 1));

		transform.position = new Vector3 (transform.position.x, transform.position.y, new_z);
	}
		

	public void MoveToJPos(int jid, Vector2 pos) {
		const int RANGE_MAX = 128;
		const float DELTA_MIN = (float)0.01;

		float new_x = SCALE_X*(float)(pos.x / RANGE_MAX - 0.5);
		float new_y = SCALE_Y*(float)(pos.y / RANGE_MAX - 0.5);

		if (Mathf.Abs(transform.position.x - new_x) > DELTA_MIN ||
			Mathf.Abs(transform.position.y - new_y) > DELTA_MIN) {

			transform.position = new Vector3 (new_x, new_y, transform.position.z);

		}
	}
}
