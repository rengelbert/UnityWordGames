using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


public class ScramblePuzzleData : MonoBehaviour {

	private static ScramblePuzzleData _instance = null;    

	public static ScramblePuzzleData Instance {
		get {
			if(_instance == null)
			{
				GameObject instanceGo = new GameObject("PuzzleData");
				_instance = instanceGo.AddComponent<ScramblePuzzleData> ();
			}

			return _instance;
		}
	}


	private Dictionary<int, List<string>> puzzles;

	void Awake() {
		
		if (_instance == null)
			_instance = this;

		else if (_instance != this)
			Destroy(gameObject);    

	}

	public void LoadData () {
		StartCoroutine ("LoadPuzzleData");
	}

	public string GetWord () {
		
		var word = "";
		var level = ScramblePlayerState.Instance.GetGameLevel ();
		var wordYield = ScramblePlayerState.Instance.GetWordYield ();
		var gameWord = ScramblePlayerState.Instance.GetGameWord ();

	
		if (!string.IsNullOrEmpty(gameWord) && gameWord.Length > 2) {
			word = gameWord;
		} else {
			var list = puzzles [wordYield];
			word = list [UnityEngine.Random.Range (0, list.Count)];
			UpdateLevel (word);

		}

		return word;
	}


	public void UpdateLevel (string word) {
		var level = ScramblePlayerState.Instance.GetGameLevel ();
		level++;
		ScramblePlayerState.Instance.SetGameLevel (level);
		ScramblePlayerState.Instance.SetGameWord (word);
	}

	public void PuzzleSolved () {
		var wordYield = ScramblePlayerState.Instance.GetWordYield ();
		wordYield = GetNextYield (wordYield);
		ScramblePlayerState.Instance.SetWordYield (wordYield);
		ScramblePlayerState.Instance.SetGameWord (string.Empty);
	}

	private int GetNextYield (int currentYield) {
		for (var i = currentYield + 1; i < 1000; i++) {
			if (puzzles.ContainsKey (i)) {
				return i;
			}
		}
		return 1000;
	}

	IEnumerator LoadPuzzleData () {
		
		string dataPath = System.IO.Path.Combine (Application.streamingAssetsPath, "wordYieldData.txt");

		string result = null;

		if (dataPath.Contains ("://")) {
			WWW www = new WWW (dataPath);
			yield return www;
			result = www.text;
		} else
			result = System.IO.File.ReadAllText (dataPath);


		var data = result.Split ('\n');

		puzzles = new Dictionary<int, List<string>> ();

		foreach (var entry in data) {
			var e = entry.TrimEnd ();
			var d = e.Split ('|');
			if (d.Length == 2) {
				var key = Int32.Parse (d [1]);
				if (key > 2) {
					if (!puzzles.ContainsKey (key))
						puzzles.Add (key, new List<string> ());
					puzzles [key].Add (d [0]);
				}
			}
		}

		ScramblePlayerState.Instance.ClearData ();
		ScrambleGameEvents.PuzzleDataLoaded ();
	}
}
