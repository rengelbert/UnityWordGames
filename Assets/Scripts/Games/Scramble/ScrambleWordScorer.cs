using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading;

public class ScrambleWordScorer: MonoBehaviour {

	private HashSet<string> allWords;
	private Dictionary<char, List<string>> wordsByFirstChar;
	private Dictionary<int, List<WordScore>> wordYieldData;

	private Thread wordYieldThread;


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

		allWords = new HashSet<string> ();
		wordsByFirstChar = new Dictionary<char, List<string>> ();

		var index = 0;

		foreach (var w in words) {

			if (string.IsNullOrEmpty(w))
				continue;

			var word = w.TrimEnd ();

			if (word.IndexOf ('#') != -1) {
				index++;
				continue;
			} else {
				if (index < 5) {
					allWords.Add (word);

					var c = word [0];
					if (!wordsByFirstChar.ContainsKey (c)) {
						wordsByFirstChar.Add (c, new List<string> ());
					}
					wordsByFirstChar [c].Add (word);
				}
			}
		}

		GetWordsYieldData ();
	}

	void GetWordsYieldData () {
		Debug.Log ("START");
		wordYieldData = new Dictionary<int, List<WordScore>> ();
		wordYieldThread = new Thread(YieldSearchWorker);
		wordYieldThread.Start ();
	}


	public void YieldSearchWorker()	{
		try{
			FindWordsData() ;
		} finally {
			NotifyCompleted ();
		}
	}


	void FindWordsData () {

		Debug.Log ("START JOB");

		foreach (var word in allWords) {
			if (word.Length < 13) {
				var words = WordsFromChars (word.ToCharArray ());

				if (!wordYieldData.ContainsKey (word.Length)) {
					wordYieldData.Add (word.Length, new List<WordScore> ());
				}
				var ws = new WordScore ();
				ws.word = word;
				ws.wordYield = words.Count;
				wordYieldData [word.Length].Add ( ws );
			}
		}
					
	}

	void NotifyCompleted () {

		var sr = File.CreateText("wordYield.txt");
		for (var i = 0; i < 13; i++) {
			if (wordYieldData.ContainsKey (i)) {
				foreach (var word in wordYieldData[i]) {
					if (word.wordYield > 2) {
						sr.WriteLine (word.word+"|"+word.wordYield);
					}
				}
				sr.WriteLine ("\n\n");
			}
		}
		sr.Close();
		Debug.Log ("JOB DONE");
	}

	HashSet<string> WordsFromChars (char[] chars){

		//no Distinct :(
		//collect all unique chars in array
		var firstChars = new List<char> ();
		foreach (var c in chars) {
			if (!firstChars.Contains (c))
				firstChars.Add (c);
		}

		var result = new HashSet<string> ();
		var i = 0;

		//loop through every word that begins with one of those chars
		foreach (var first in firstChars) {
			if (wordsByFirstChar.ContainsKey (first)){

				var list = wordsByFirstChar [first];

				foreach (var word in list) {

					if (word.Length <= chars.Length && !result.Contains (word) ) {

						var sourceChars = new char[chars.Length];
						Array.Copy (chars, sourceChars, chars.Length);
						var cIndex = Array.IndexOf (sourceChars, first);
						if (cIndex != -1)
							sourceChars [cIndex] = '-';

						var wordChars = word.ToCharArray ();

						var match = true;

						for (var j = 1; j < wordChars.Length; j++) {

							var index = Array.IndexOf (sourceChars, wordChars [j]);

							if (index != -1) {
								sourceChars [index] = ' ';
							} else {
								match = false;
								break;
							}
						}

						if (match)
							result.Add (word);
					}
				}
			}
		}

		return result;
	}

	public struct WordScore {
		public int wordYield;
		public string word;
	}
}
