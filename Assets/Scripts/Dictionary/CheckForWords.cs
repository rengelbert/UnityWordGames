using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class CheckForWords : MonoBehaviour {
	

	private HashSet<string> allWords;
	private Dictionary<int, List<string>> wordsByLength;
	private Dictionary<char, List<string>> wordsByFirstChar;
	private Dictionary<string, List<string>> wordSortedChars;

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

		allWords = new HashSet<string> ();
		wordsByLength = new Dictionary<int, List<string>> ();
		wordsByFirstChar = new Dictionary<char, List<string>> ();
		wordSortedChars = new Dictionary<string, List<string>> ();

		var words = wordData.Split ('\n');
		foreach (var w in words) {
			if (string.IsNullOrEmpty(w))
				continue;
			var word = w.TrimEnd ();
			if (word.IndexOf ('#') != -1) {
				continue;
			} else {
				//all words
				if (!allWords.Contains (word)) {
					allWords.Add (word);
				}

				//by length
				if (!wordsByLength.ContainsKey (word.Length)) {
					wordsByLength.Add (word.Length, new List<string> ());
				}
				wordsByLength [word.Length].Add (word);

				//by first char
				var c = word [0];
				if (!wordsByFirstChar.ContainsKey (c)) {
					wordsByFirstChar.Add (c, new List<string> ());
				}
				wordsByFirstChar [c].Add (word);

				//by sorted chars
				var sortedWord = w.ToCharArray();
				System.Array.Sort(sortedWord);

				var sortedString = new string (sortedWord);

				if (!wordSortedChars.ContainsKey (sortedString))
					wordSortedChars.Add (sortedString, new List<string>());

				wordSortedChars[sortedString].Add(word);
			}
		}

	}

	public bool IsAWord (string word) {
		return allWords.Contains (word);

		//by length
		/*
		if (wordsByLength.ContainsKey (word.Length)) {
			return wordsByLength [word.Length].Contains (word);
		}
		*/

		//by first char
		/*
		if (wordsByFirstChar.ContainsKey (word[0])) {
			return wordsByFirstChar [word[0]].Contains (word);
		}
		*/

	}

	public HashSet<string> WordsFromChars (char[] chars){
		
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

	public string HasAWord (char[] chars) {
		System.Array.Sort(chars);
		var key = new string (chars);
		if (wordSortedChars.ContainsKey(key)) {
			var list = wordSortedChars[key];
			return list[ UnityEngine.Random.Range(0, list.Count)];
		}
		return null;
	}

	public List<string> HasWords (char[] chars){

		var result = new List<string> ();
		var i = 0;

		foreach (var word in allWords) {
			if (word.Length >= chars.Length) {
				var wordChars = word.ToCharArray ();
				var match = true;
				for (var j = 0; j < chars.Length; j++) {
					var index = Array.IndexOf (wordChars, chars [j]);
					if (index != -1) {
						wordChars [index] = ' ';
					} else {
						match = false;
						break;
					}
				}
				if (match)
					result.Add (word);
			}
		}

		return result;
	}


	public HashSet<string> MatchesAPattern (char[] chars){
		var result = new HashSet<string> ();
		var list = wordsByLength [chars.Length];

		foreach (var word in list) {
			var match = true;
			for (var i = 0; i < word.Length; i++) {
				if (chars [i] != '-' && word [i] != chars [i]) {
					match = false;
					break;
				}
			}
			if (match)
				result.Add (word);
		}

		return result;
	}



}
