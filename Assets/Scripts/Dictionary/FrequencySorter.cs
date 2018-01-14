using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;


public class FrequencySorter : MonoBehaviour {

	private string[] fileList = new string[] {"words/1k.txt","words/2k.txt","words/10k.txt","words/20k.txt","words/50k.txt","words/300k.txt", };
	public char[] badChars = new char[] {' ', '.', '-', '\'', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '˜', '`' };

	HashSet<string> names;

	void Start () {


		var namesTxt = File.ReadAllText (System.IO.Path.Combine (Application.streamingAssetsPath, "words/allNames.txt"));
		var namesData = namesTxt.Split ('\n');
		names = new HashSet<string> ();

		foreach (var line in namesData) {
			var name = line.TrimEnd ();
			if (string.IsNullOrEmpty (name))
				continue;
			names.Add (name);
		}

		StartCoroutine ("SortByFrequency");
	}


	IEnumerator SortByFrequency() {

		var sets = new List< HashSet<string> > ();
		string path = null;

		var setIndex = 0;
		while (setIndex < fileList.Length) {

			sets.Add (new HashSet<string> ());

			path = fileList [setIndex];
			string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, path);
			string result = null;
			if (filePath.Contains ("://")) {
				WWW www = new WWW (filePath);
				yield return www;
				result = www.text;
			} else
				result = System.IO.File.ReadAllText (filePath);

			var words = result.Split ('\n');

			foreach (var w in words) {
				var word = w.TrimEnd ();
				if (string.IsNullOrEmpty (word))
					continue;
				if (word.IndexOfAny (badChars) != -1)
					continue;
				if (names.Contains(word.ToLower())) continue;
					
				var previousIndex = setIndex - 1;
				var unique = true;


				while (previousIndex >= 0) {
					if (sets [previousIndex].Contains (word)) {
						unique = false;
						break;
					}
					previousIndex--;
				}
				if (unique) sets [setIndex].Add (word);
			}

			setIndex++;

			//cool off
			yield return new WaitForSeconds (0.5f);

		}

		//now we have words sorted by frequency

		var sr = File.CreateText("wordsByFrequency.txt");

		setIndex = 0;

		while (setIndex < sets.Count) {
			var wSet = sets [setIndex];

			foreach (var w in wSet) {
				sr.WriteLine ( w );
			}

			//add separator
			sr.WriteLine ( "###" );

			setIndex++;	
		}

		sr.Close();

		Debug.Log ("ALL DONE");

	}
}

