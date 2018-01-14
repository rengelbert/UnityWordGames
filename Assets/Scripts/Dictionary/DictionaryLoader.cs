using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class DictionaryLoader : MonoBehaviour {

	private HashSet<string> dictionary1k;
	private HashSet<string> dictionary2k;
	private HashSet<string> dictionary10k;
	private HashSet<string> dictionary20k;
	private HashSet<string> dictionary50k;
	private HashSet<string> dictionary300k;
	private HashSet<string>[] dictionaries;


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

		var words = wordData.Split ('\n');

		dictionary1k = new HashSet<string> ();
		dictionary2k = new HashSet<string> ();
		dictionary10k = new HashSet<string> ();
		dictionary20k = new HashSet<string> ();
		dictionary50k = new HashSet<string> ();
		dictionary300k = new HashSet<string> ();
		dictionaries = new HashSet<string>[] { dictionary1k, dictionary2k, dictionary10k, dictionary20k, dictionary50k, dictionary300k};


		var index = 0;

		foreach (var w in words) {

			if (string.IsNullOrEmpty(w))
				continue;

			var word = w.TrimEnd ();

			if (word.IndexOf ('#') != -1) {
				index++;
				continue;
			} else {
				dictionaries [index].Add (word);
			}
		}
	}
}

