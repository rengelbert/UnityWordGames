using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;


public class Scramble : MonoBehaviour {

	public Text spelling;
	public Text scrambledWord;
	public Text wordsList;

	private ScramblePanel tiles;

	private List<string> selectedWords;
	private List<string> wordsFound;
	private Dictionary<int, List<MappedWord>> mappedWords;


	private string selectedWord;
	private int totalWords;


	internal struct MappedWord {
		public string word;
		public bool found;
		public MappedWord (string word, bool found = false) {
			this.word = word;
			this.found = found;
		}
	}

	void Awake () {
		tiles = GetComponent<ScramblePanel> ();
		selectedWords = new List<string> ();
		wordsFound = new List<string> ();
		mappedWords = new Dictionary<int, List<MappedWord>> ();

		ScrambleGameEvents.OnGameLoaded += HandleGameLoaded;
		ScrambleGameEvents.OnTileSelected += HandleTileSelected;
		ScrambleGameEvents.OnWordSelected += HandleWordSelected;

		ScrambleDictionary.Instance.Initialize();
	}

	void HandleGameLoaded () {
		NewRound ();	
	}

	void NewRound () {
		
		SelectWord ();
	}

	void HandleTileSelected (string word) {
		spelling.text = word.ToUpper();
	}

	void HandleWordSelected (string word, List<TileButton> tiles) {
		//animate tiles?
//		foreach (var tile in tiles) {
//			tile.Select (false);
//		}

		if (!wordsFound.Contains (word)) {
			
			if (word == selectedWord) {
				scrambledWord.text = selectedWord.ToUpper();
				wordsFound.Add (selectedWord);
				totalWords--;
			} else {

				if (ScrambleDictionary.Instance.IsValidWord (word)) {
					wordsFound.Add (word);

					//check if word is in the map
					if (mappedWords.ContainsKey (word.Length)) {

						var list = mappedWords [word.Length];
						var placed = false;

						for (var i = 0; i < list.Count; i++) {
							var w = list [i];
							if (w.word == word) {
								placed = true;
								w.found = true;
								totalWords--;
								list [i] = w;

							}
						}
							
						if (!placed) {

							//the word found is not in our list, so add it to it if possible
							for (var i = 0; i < list.Count; i++) {
								var w = list [i];
								if (w.found == false) {
									placed = true;
									w.word = word;
									w.found = true;
									list [i] = w;
									totalWords--;
									break;
								}
							}

							if (!placed) ShowBonusWord(word);
						}

						ShowMappedWords ();
					} else {
						//this is a bonus word!
						ShowBonusWord(word);

					}
				}
			}
		}

		spelling.text = "";

		if (totalWords <= 0) {
//			start new round
			Utils.DelayAndCall (this, 1, () => {
				ScramblePlayerState.Instance.SetGameWord (null);
				NewRound();
			});
		}
	}



	void SelectWord () {

		var word = ScramblePuzzleData.Instance.GetWord ();

		if (selectedWords.Contains (word)) {
			SelectWord ();
		} else {
			
			selectedWord = word;
			selectedWords.Add (selectedWord);

			//load board
			tiles.ShowWord(selectedWord);

			//load word tiles
			var i = 0;
			scrambledWord.text = "";

			while (i < selectedWord.Length) {
				scrambledWord.text += "_"; 
				i++;
			}

			var words = ScrambleDictionary.Instance.WordsFromChars (selectedWord.ToCharArray ());
		
			totalWords = words.Count;

			mappedWords.Clear ();

			foreach (var w in words) {
				if (w != selectedWord) {

					Debug.Log (w);
					if (mappedWords.ContainsKey (w.Length)) {						
						mappedWords [w.Length].Add (new MappedWord(w));
					} else {
						
						mappedWords.Add (w.Length, new List<MappedWord> () { new MappedWord(w) });
					}
				}
			}

			ShowMappedWords ();

			Debug.Log (selectedWord);
		}
	}


	void ShowBonusWord (string word) {
		//handle display of bonus word here
		Debug.Log ("BONUS");
	}
		

	void ShowMappedWords () {
		
		var sb = new StringBuilder ();
		var words = new List<string> ();
		var i = 3;
		while (i < 13) {
			if (mappedWords.ContainsKey (i)) {
				var list = mappedWords [i];
				foreach (var w in list) {
					if (w.found) {
						words.Add (w.word.ToUpper());
					} else {
						var dash = "";
						foreach (var c in w.word) {
							dash += "_";
						}
						words.Add (dash);
					}
				}
			}
			i++;
		}

		for (i = 0; i < words.Count; i++) {
			sb.Append (words [i]);
			if (i != words.Count - 1) {
				sb.Append (", ");
			}
		}

		wordsList.text = sb.ToString ();
	}
}
