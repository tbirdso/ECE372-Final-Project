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

public abstract class ArdEventComms : MonoBehaviour {

	public delegate void PulseEventHandler(bool val);
	public event PulseEventHandler Pulse;

	public void OnConnectionEvent(bool connected) {
		if (connected) {
			Debug.Log("Connection established");
		} else {
			Debug.Log("Connection attempt failed or disconnection detected");
		}
	}

	public virtual void OnMessageArrived(string message) {
		Debug.Log ("Incoming message: " + message);

		if (message == "0") {
			OnPulse ();
		}
	}

	public void OnPulse() {

		PulseEventHandler handler = Pulse;

		if (handler != null)
			handler.Invoke (true);
	}

}
