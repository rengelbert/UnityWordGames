using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class BogglingDictionary : MonoBehaviour {

	private static BogglingDictionary _instance = null;    
	public static BogglingDictionary Instance
	{
		get
		{
			if(_instance == null)
			{
				GameObject instanceGo = new GameObject("GameDictionary");
				_instance = instanceGo.AddComponent<BogglingDictionary> ();
			}

			return _instance;
		}
	}

	[HideInInspector]
	public List<string> allWords;

	[HideInInspector]
	public List<string> commonDictionaryWords;


	public void Initialize () {
		StartCoroutine ("LoadWordData");
	}
		

	IEnumerator LoadWordData() {

		if (allWords != null && allWords.Count != 0)
			yield break;
		else {

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
			allWords = new List<string> ();
			commonDictionaryWords = new List<string> ();

			var index = 0;

			foreach (var w in words) {

				if (string.IsNullOrEmpty(w) ||  w.Length < 3)
					continue;

				var word = w.TrimEnd ();

				if (word.IndexOf ('#') != -1) {
					index++;
				} else {
					
					allWords.Add (word);
					if (index < 5) {
						commonDictionaryWords.Add (word);
					}
				}
			}

			BogglingGameEvents.GameLoaded ();
		}
	}

	public bool IsValidWord (string word)	{
		return allWords.Contains(word);
	}

	public char[] ScrambleWord (string word) {
		return Utils.Scramble<char> (word.ToCharArray ());
	}

	public char[] GetRandomChars (int len) {

		if (len == 0)
			len = 100;


		var randomString = "";
		var vRatio = 0.5f;
		var cRatio = 0.5f;

		//0.6  0.4 = 30!
		//0.5  0.5 = 30!

		var vowels = Mathf.RoundToInt (len * vRatio);
		var consonants = Mathf.RoundToInt (len * cRatio);


		var i = 0;
		while (i < vowels) {
			randomString += TileChar.vowels[UnityEngine.Random.Range(0, TileChar.vowels.Length)];
			i++;
		}

		i = 0;
		while (i < consonants) {
			randomString += TileChar.consonants[UnityEngine.Random.Range(0, TileChar.consonants.Length)];
			i++;
		}

		while (randomString.Length < len) {
			randomString += TileChar.vowels[UnityEngine.Random.Range(0, TileChar.vowels.Length)];
		}

		return ScrambleWord (randomString);
	}

}
