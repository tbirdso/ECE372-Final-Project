using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalInteract : MonoBehaviour {

	public SpawnGoal gameManager;

	void Update() {
		Debug.Log ("Goal is awake");
	}

	void OnTriggerEnter(Collider c) {

		Debug.Log ("Found a goal!");

		gameManager.makeGoal ();

		Destroy (gameObject);
	}

	void OnColliderEnter(Collision c) {
		Debug.Log ("Found a collision.");
	}

}
