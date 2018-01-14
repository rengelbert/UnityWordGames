using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class ScrambleDictionary : MonoBehaviour {

	private static ScrambleDictionary _instance = null;    


	public static ScrambleDictionary Instance
	{
		get
		{
			if(_instance == null)
			{
				GameObject instanceGo = new GameObject("GameDictionary");
				_instance = instanceGo.AddComponent<ScrambleDictionary> ();
			}

			return _instance;
		}
	}

	[HideInInspector]
	public HashSet<string> allWords;
	public Dictionary<char, HashSet<string>> wordsCharMap;


	public void Initialize () {
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

		var words = result.Split ('\n');

		//collect words
		allWords = new HashSet<string> ();
		wordsCharMap = new Dictionary<char, HashSet<string>> ();

		var index = 0;
		foreach (var w in words) {
			if (string.IsNullOrEmpty(w) ||  w.Length < 3)
				continue;

			var word = w.TrimEnd ();

			if (word.IndexOf ('#') != -1) {
				index++;
				continue;
			} else {
				if (index < 5) {
					var c = word [0];
					if (!wordsCharMap.ContainsKey (c)) {
						wordsCharMap.Add (c, new HashSet<string> ());
					}
					wordsCharMap [c].Add (word);
				}
			}	
			allWords.Add (word);
		}

		ScrambleGameEvents.DictionaryLoaded ();
		ScramblePuzzleData.Instance.LoadData ();
	}


	public bool IsValidWord (string word){
		return allWords.Contains(word);
	}
		

	public char[] ScrambleWord (string word) {
		return Utils.Scramble<char> (word.ToCharArray ());
	}


	public HashSet<string> WordsFromChars (char[] chars){
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
			
			if (wordsCharMap.ContainsKey (first)){

				var list = wordsCharMap [first];

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
								sourceChars [index] = '-';
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
}
