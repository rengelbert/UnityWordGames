using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DuckSpawn : MonoBehaviour {

	public GameObject duckGO;
	public Text ducksCount;
	int ducks = 0;


	void Awake () {
		GameEvents.DucksUpdated += HandleDuckUpdate;
		HandleDuckUpdate (0);
	}

	void OnDestroy () {
		GameEvents.DucksUpdated -= HandleDuckUpdate;
	}

	void HandleDuckUpdate (int cnt) {
		ducks += cnt;
		ducksCount.text  = "" + ducks;
	}


	void Start () {

		var i = 0;
		while (i < 10) {

			var duck = Instantiate (duckGO) as GameObject;
			duck.transform.position = new Vector2 (Random.Range (-3, 3), 5);
			duck.transform.parent = transform;
			GameEvents.OnDucksUpdated(1);
			i++;
		}
	}
}
