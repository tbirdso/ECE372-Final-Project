using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyComm : MonoBehaviour {

	public PICMessageListener realListener;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown (KeyCode.Z)) {
			realListener.OnButtonMove (1, true);
		}
		if (Input.GetKeyUp (KeyCode.Z)) {
			realListener.OnButtonMove (1, false);
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			realListener.OnButtonMove (2, true);
		}

		if (Input.GetKeyDown (KeyCode.A)) {
			realListener.OnJoystickMove (1, new Vector2 (0, 63));
		}
		if (Input.GetKeyDown (KeyCode.W)) {
			realListener.OnJoystickMove (1, new Vector2 (63,127));
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			realListener.OnJoystickMove (1, new Vector2 (63,0));
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			realListener.OnJoystickMove (1, new Vector2 (127,63));
		}

	}
}
