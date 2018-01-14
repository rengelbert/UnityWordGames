using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LengthSorter : MonoBehaviour {

	private Dictionary<int, HashSet<string>> wordsByLength;

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
	
		wordsByLength = new Dictionary<int, HashSet<string>> ();
	
		var words = wordData.Split ('\n');
		foreach (var w in words) {
			if (string.IsNullOrEmpty(w))
				continue;
			var word = w.TrimEnd ();
			if (word.IndexOf ('#') != -1) {
				continue;
			} else {
				if (!wordsByLength.ContainsKey (word.Length)) {
					wordsByLength.Add (word.Length, new HashSet<string> ());
				}
				wordsByLength [word.Length].Add (word);
			}
		}
	}
}
