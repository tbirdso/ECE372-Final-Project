using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGoal : MonoBehaviour {

	public GameObject prefab;
	public Vector3 min_spawn;
	public Vector3 max_spawn;

	void Start () {
		Random.InitState (3720);

		for (int i = 0; i < 3; i++) {
			makeGoal ();
		}
	}

	public void makeGoal() {
		Debug.Log ("Let's make a goal.");

		float x_pos = Random.Range (min_spawn.x, max_spawn.x);
		float y_pos = Random.Range (min_spawn.y, max_spawn.y);
		float z_pos = Random.Range (min_spawn.z, max_spawn.z);

		GameObject newGoal = Instantiate (prefab,
			new Vector3 (x_pos, y_pos, z_pos),
			Quaternion.identity);

		newGoal.GetComponent<GoalInteract>().gameManager = this;
	}
}
