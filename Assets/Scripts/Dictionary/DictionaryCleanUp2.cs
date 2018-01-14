using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DictionaryCleanUp2 : MonoBehaviour {

	public string inputFile;
	public string outputFile;
	public int minLength = 3;
	public char[] badChars = new char[] {' ', '.', '-', '\'', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0',':'};

	private HashSet<string> names;
	void Start () {

		if (string.IsNullOrEmpty (inputFile))
			throw new Exception ("Missing Input File!");

		if (string.IsNullOrEmpty (outputFile))
			throw new Exception ("Missing Output File!");

		StartCoroutine ("LoadNamesList");
	}

	IEnumerator LoadNamesList() {

		string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, "words/names.txt");
		string result = null;
		if (filePath.Contains ("://")) {
			WWW www = new WWW (filePath);
			yield return www;
			result = www.text;
		} else
			result = System.IO.File.ReadAllText (filePath);


		names = new HashSet<string> ();

		var namesList = result.Split ('\n');


		foreach (var n in namesList) {
			var name = n.TrimEnd ();
			if (string.IsNullOrEmpty (name))
				continue;
			if (!names.Contains(name.ToLower())) 
				names.Add (name.ToLower());
		}

		StartCoroutine ("LoadDictionary");
	}

	IEnumerator LoadDictionary() {
		
		string filePath = System.IO.Path.Combine (Application.streamingAssetsPath, inputFile);
		string result = null;
		if (filePath.Contains ("://")) {
			WWW www = new WWW (filePath);
			yield return www;
			result = www.text;
		} else
			result = System.IO.File.ReadAllText (filePath);

		var words = result.Split ('\n');

		var allWords = new HashSet<string> ();

		foreach (var w in words) {
			var word = w.TrimEnd ().ToLower();
			if (string.IsNullOrEmpty (word))
				continue;
			if (word.Length < minLength)
				continue;
			if (word.IndexOfAny (badChars) != -1)
				continue;
			if (names.Contains (word)) 
				continue;

			if (!allWords.Contains(word)) 
				allWords.Add (word);
			
		}

		var sr = File.CreateText( System.IO.Path.Combine (Application.streamingAssetsPath, outputFile ));
		foreach (var w in allWords) {
			sr.WriteLine ( w );
		}
		sr.Close();
	}
}
