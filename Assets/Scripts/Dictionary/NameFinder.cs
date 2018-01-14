using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class NameFinder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var namesTxt = File.ReadAllText (System.IO.Path.Combine (Application.streamingAssetsPath, "words/names.txt"));
		var namesData = namesTxt.Split ('\n');
		var names = new HashSet<string> ();
		foreach (var line in namesData) {
			var name = line.TrimEnd ();
			if (string.IsNullOrEmpty (name))
				continue;
			if (!names.Contains (name))
				names.Add (name);
		}

		var allWords = new HashSet<string> ();
		var bigOne = File.ReadAllText (System.IO.Path.Combine (Application.streamingAssetsPath, "words/AllWords.txt"));
		var data = bigOne.Split ('\n');
		foreach (var line in data) {
			var word = line.TrimEnd ();
			if (string.IsNullOrEmpty (word))
				continue;
			if (!allWords.Contains (word))
				allWords.Add (word);

		}

		var properNames = new HashSet<string> ();

		foreach (var word in allWords) {
			if (word [0] >= 'A' && word [0] <= 'Z') {
				if (!allWords.Contains (word.ToLower ())) {
					if (names.Contains (word.ToLower ())) {
						properNames.Add (word.ToLower ());
					} 
				} 
			} 
		}

		var sr = File.CreateText("names.txt");
		foreach (var w in properNames) {
			sr.WriteLine (w);	
		}
		sr.Close();
	}
}