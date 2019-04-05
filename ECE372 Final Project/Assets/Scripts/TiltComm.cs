using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltComm : BasicArdComm {

	public GameObject box;

	// Use this for initialization
	protected override void Start () {
		base.Start ();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update ();

		if (base.message != null) {

			if (base.message.Contains ("0")) {
				box.SetActive (false);
			} else if (base.message.Contains ("1")) {
				box.SetActive (true);
			} else {
				Debug.Log ("message: " + base.message);
			}
		}
	}
}
