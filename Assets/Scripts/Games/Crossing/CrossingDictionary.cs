using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossingDictionary : MonoBehaviour {

	private static CrossingDictionary _instance = null;    
	public static CrossingDictionary Instance
	{
		get
		{
			if(_instance == null)
			{
				GameObject instanceGo = new GameObject("GameDictionary");
				_instance = instanceGo.AddComponent<CrossingDictionary> ();
			}

			return _instance;
		}
	}

	[HideInInspector]
	public Dictionary<int, List<string>> allWords;

	private Dictionary<int, List<string>> wordsByLength;

	private Dictionary<int, List<string>> uniqueWordsByLength;


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
		allWords = new Dictionary<int, List<string>> ();
		wordsByLength = new Dictionary<int, List<string>> ();
		uniqueWordsByLength = new Dictionary<int, List<string>> ();

		var index = 0;

		foreach (var w in words) {

			if (string.IsNullOrEmpty(w) ||  w.Length < 3)
				continue;

			var word = w.TrimEnd ();

			if (word.IndexOf ('#') != -1) {
				index++;
			} else {

				if (!allWords.ContainsKey (word.Length)) 
					allWords.Add (word.Length, new List<string> ());
				
				allWords[word.Length].Add (word);

				if (index < 4) {
					if (!wordsByLength.ContainsKey (word.Length))
						wordsByLength.Add (word.Length, new List<string> ());
					wordsByLength [word.Length].Add (word);
				}
			}
		}

		//shuffle lists
		for (var i = 3; i < 10; i++) {
			if (wordsByLength.ContainsKey (i)) {
				var list = wordsByLength [i];
				list = Utils.Scramble<string> (list);
				wordsByLength [i] = list;
				uniqueWordsByLength.Add (i, list); 
			}
		}

	
		CrossingEvents.GameLoaded ();

	}

	public bool IsValidWord (string word)	{
		return allWords[word.Length].Contains(word);
	}

	public string RandomWord (int len, bool all = false) {
		
		if (all) {
			var alllist = allWords [len];
			return alllist [Random.Range (0, alllist.Count)];
		}
		var list = wordsByLength [len];
		return list [Random.Range (0, list.Count)];
	}

	public string RandomUniqueWord (int len) {

		if (uniqueWordsByLength.ContainsKey (len)) {
			var list = uniqueWordsByLength [len];
			var word = list [0];
			list.RemoveAt (0);
			return word;
		}
		return RandomWord (len, true);
	}
		
	public List<string> MatchesAPattern (char[] chars){

		var result = new List<string> ();

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
