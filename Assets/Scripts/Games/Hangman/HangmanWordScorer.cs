using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class HangmanWordScorer : MonoBehaviour {


	void Start () {
		StartCoroutine ("LoadData");
	}

	IEnumerator LoadData() {

		string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, "wordsByFrequency.txt");

		string result = null;

		if (filePath.Contains ("://")) {
			WWW www = new WWW (filePath);
			yield return www;
			result = www.text;
		} else
			result = System.IO.File.ReadAllText (filePath);


		var difficulty = new List<char[]> () {
			
			new char[] { 'e' },
			new char[] { 't' },
			new char[] { 'a', 'i', 'n', 'o', 's' },
			new char[] { 'h' },
			new char[] { 'r' },
			new char[] { 'd' },
			new char[] { 'l' },
			new char[] { 'u' },
			new char[] { 'c', 'm' },
			new char[] { 'f' },
			new char[] { 'w', 'y' },
			new char[] { 'g', 'p' },
			new char[] { 'b', 'v' },
			new char[] { 'k' },
			new char[] { 'q' },
			new char[] { 'j', 'x' },
			new char[] { 'z' },

		};



		var words = result.Split ('\n');
		var difficultyMap = new Dictionary<int, List<string>> ();
		var index = 0;

		foreach (var w in words) {
			if (string.IsNullOrEmpty(w) ||  w.Length < 3)
				continue;

			var word = w.TrimEnd ();

			if (word.IndexOf ('#') != -1) {
				index++;
				continue;
			} else {
				
				var score = 0;
				var chars = new List<char> ();
				foreach (var c in word) {
					for (var i = 0; i < difficulty.Count; i++) {
						if (Array.IndexOf (difficulty [i], c) != -1) {
							score += i;
							break;
						}
					}
					if (!chars.Contains (c))
						chars.Add (c);
				}
				score += chars.Count;
				score += index;

				if (!difficultyMap.ContainsKey (score)) {
					difficultyMap.Add (score, new List<string> ());
				}
				difficultyMap [score].Add (word);
			}
				
		}

		var sr = File.CreateText("hangman.txt");
		for (var i = 0; i < 1000; i++) {
			if (difficultyMap.ContainsKey (i)) {
				var list = difficultyMap [i];
				foreach (var w in list) {
					sr.WriteLine (i + "|" + w);
				}
			}
		}

		sr.Close();
	}
}
