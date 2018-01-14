using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllLettersSorter : MonoBehaviour {

	private Dictionary<string, HashSet<string>> wordSortedChars;
	
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
	
		wordSortedChars = new Dictionary<string, HashSet<string>> ();
	
		var words = wordData.Split ('\n');
		foreach (var w in words) {
			if (string.IsNullOrEmpty(w))
				continue;
			var word = w.TrimEnd ();
			if (word.IndexOf ('#') != -1) {
				continue;
			} else {
				var sortedWord = w.ToCharArray();
				System.Array.Sort(sortedWord);
		
				var sortedString = new string (sortedWord);

				if (!wordSortedChars.ContainsKey (sortedString))
					wordSortedChars.Add (sortedString, new HashSet<string>());
					
				wordSortedChars[sortedString].Add(word);
			}
		}
	}
}
