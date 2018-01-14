using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstCharSorter : MonoBehaviour {

	private Dictionary<char, HashSet<string>> wordsByFirstChar;

	void Start () {
		StartCoroutine ("LoadWordData");
	}

	IEnumerator LoadWordData() {
		string dictionaryPath = System.IO.Path.Combine (Application.streamingAssetsPath, "wordsByFrequency.txt");

		string result = null;

		if (dictionaryPath.Contains ("://")) {
			WWW www = new WWW (dictionaryPath);
			yield return www;
			result = www.text;
		} else
			result = System.IO.File.ReadAllText (dictionaryPath);

		ProcessWordData (result);
	}

	void ProcessWordData (string wordData) {
	
		wordsByFirstChar = new Dictionary<char, HashSet<string>> ();
	
		var words = wordData.Split ('\n');
		foreach (var w in words) {
			if (string.IsNullOrEmpty(w))
				continue;
			var word = w.TrimEnd ();
			if (word.IndexOf ('#') != -1) {
				continue;
			} else {
				var c = word [0];
				if (!wordsByFirstChar.ContainsKey (c)) {
					wordsByFirstChar.Add (c, new HashSet<string> ());
				}
				wordsByFirstChar [c].Add (word);
			}
		}
	}
}
