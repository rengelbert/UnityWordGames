using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrieSorter : MonoBehaviour {

	public string wordToTest = "apple";
	private Trie wordTrie;


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

		if (!string.IsNullOrEmpty(wordToTest)) 
			Debug.Log( IsAWord ("apple"));
	}

	void ProcessWordData (string wordData) {

		wordTrie = new Trie ();

		var words = wordData.Split ('\n');
		foreach (var w in words) {
			if (string.IsNullOrEmpty(w))
				continue;
			var word = w.TrimEnd ();
			if (word.IndexOf ('#') != -1) {
				continue;
			} else {
				wordTrie.Insert (word);
			}
		}
	}

	public bool IsAWord (string word) {
		return wordTrie.ContainWord (word);
	}
}
