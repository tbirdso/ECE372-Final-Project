using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalInteract : MonoBehaviour {

	public SpawnGoal gameManager;

	void OnTriggerEnter(Collider c) {

		gameManager.makeGoal ();

		Destroy (gameObject);
	}
}
