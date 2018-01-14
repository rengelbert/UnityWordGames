using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Linq;

public class Boggling : MonoBehaviour {

	public Text spelling;
	public Text wordsList;

	private BogglingGrid grid;
	private List<string> wordsFound;
	private List<string> selectedWords;
	private List<MappedWord> mappedWords;
	private ProcessedGrid solvedGrid;


	private Vector2[] gridSizes = 
	{   new Vector2(3,4), 
		new Vector2(3,4), 
	    new Vector2(4,5),
		new Vector2(4,5),
		new Vector2(4,5),
		new Vector2(5,6),
		new Vector2(5,6),
		new Vector2(6,7)
	};

	private int[] numWords = 
	{
		1,2,3,4,5,6,7,8
	};


	private int difficulty = 0;
	private int level = 0;
	private bool gameActive = false;

	internal struct MappedWord {
		public string word;
		public char[] wordChars;
		public bool[] revealedChars;
		public bool revealed;
		public MappedWord (string word, int difficulty, bool revealed = false) {
			this.word = word;
			this.revealed = revealed;
			this.wordChars = word.ToCharArray();
			revealedChars = new bool[wordChars.Length];
			if (difficulty < 4) {
				revealedChars[0] = true;
			} else {
				revealedChars[Random.Range( 0, wordChars.Length)] = true;
			}
		}
	}

	void Awake () {
		
		wordsFound = new List<string> ();
		selectedWords = new List<string>();
		mappedWords = new List<MappedWord> ();
		grid = GetComponent<BogglingGrid> ();

		BogglingGameEvents.OnGameLoaded += HandleGameLoaded;
		BogglingGameEvents.OnTileSelected += HandleTileSelected;
		BogglingGameEvents.OnWordSelected += HandleWordSelected;

		BogglingDictionary.Instance.Initialize ();
	}

	void HandleGameLoaded () {
		
		grid.COLUMNS = (int) gridSizes [difficulty].x;
		grid.ROWS = (int) gridSizes [difficulty].y;

		grid.COLUMNS = (int) gridSizes [difficulty].x;
		grid.ROWS = (int) gridSizes [difficulty].y;

		NewRound ();	

		level++;

		if (level % 2 == 0)
			difficulty++;

		if (difficulty > gridSizes.Length - 1)
			difficulty = gridSizes.Length - 1;


	}

	void NewRound () {

		spelling.text = "";
		wordsList.text = "";

		//create timer
		StartCoroutine ("GameTimer");

		BuildGrid ();

		gameActive = true;
	}

	void HandleTileSelected (string word) {
		if (!gameActive)
			return;
		spelling.text = word.ToUpper();
	}

	void HandleWordSelected (string word, List<GridTile> tiles) {

		if (!gameActive)
			return;
		
		//animate tiles?
		foreach (var tile in tiles) {
			tile.Select (false);
		}

		if (!wordsFound.Contains (word)) {

			wordsFound.Add (word);

			if (selectedWords.Contains (word)) {

				var totalWords = 0;

				for (var i = 0; i < mappedWords.Count; i++) {
					var mappedWord = mappedWords [i];
					if (mappedWord.revealed)
						totalWords++;
					
					if (!mappedWord.revealed && mappedWord.word.ToLower () == word.ToLower ()) {
						mappedWord.revealed = true;
						mappedWords [i] = mappedWord;
						totalWords++;
					}
				}

				if (totalWords == mappedWords.Count) {
					StopCoroutine ("GameTimer");

					gameActive = false;
					spelling.text = "Well done!";

					//start new round
					Utils.DelayAndCall (this, 2, () => {
						NewRound();
					});
				}

				ShowMappedWords ();

			} else {
				if (BogglingDictionary.Instance.IsValidWord (word)) {
					//show bonus?
				}
			}
		}
 	}

	void BuildGrid () {

		grid.BuildGrid ();
		grid.ShowGridChars (BogglingDictionary.Instance.GetRandomChars (grid.COLUMNS * grid.ROWS));


		//solve grid
		solvedGrid = GridUtils.SolveGrid (ref BogglingDictionary.Instance.commonDictionaryWords, grid.GetGridString(), grid.ROWS, grid.COLUMNS);

		//select mapped words
		selectedWords.Clear();
		mappedWords.Clear ();

		var wordList = solvedGrid.words.OrderByDescending(x=>x.Length).ThenBy(x=>x).ToList();
		var num = numWords [difficulty];
		if (num > wordList.Count)
			num = wordList.Count;

		var i = 0;
		while (i < num) {
			selectedWords.Add (wordList [i]);
			mappedWords.Add( new MappedWord (wordList [i].ToUpper(), difficulty) );
			i++;
		}
		ShowMappedWords ();
	}
		

	void ShowMappedWords () {
		var sb = new StringBuilder ();
		var line = 0;
		foreach (var w in mappedWords) {
			if (w.revealed) { 
				sb.AppendLine (w.word);
				line++;
			} else {
				if (line != 0) sb.AppendLine ("");
				line++;
				for (var i = 0; i < w.wordChars.Length; i++) {
					if (w.revealedChars [i]) {
						sb.Append (w.wordChars [i]);
					} else {
						sb.Append ("_");
					}
				}
			}
		}
		wordsList.text = sb.ToString ();
	}

	void GiveRandomHint () {

		for (var i = 0; i < mappedWords.Count; i++) {
			var mw = mappedWords [i];
			if (!mw.revealed) {
				var cnt = 0;
				foreach (var c in mw.revealedChars)
					if (c == true)
						cnt++;
				if (cnt == mw.wordChars.Length) {
					mw.revealed = true;
					mappedWords [i] = mw;
					continue;
				}
				var attempts = 0;
				while (attempts < 1000) {
					var index = Random.Range (0, mw.wordChars.Length);
					if (mw.revealedChars [index] == false) {
						mw.revealedChars [index] = true;
						mappedWords [i] = mw;
						ShowMappedWords ();
						break;
					}
					attempts++;
				}
			} 
		}

		CheckIfWon ();
	}

	void CheckIfWon () {
		var revealed = 0;

		for (var i = 0; i < mappedWords.Count; i++) {
			var mw = mappedWords [i];
			if (!mw.revealed) {
				var cnt = 0;
				foreach (var c in mw.revealedChars)
					if (c == true)
						cnt++;
				if (cnt == mw.wordChars.Length) {
					revealed++;
				}
			} else {
				revealed++;
			}
		}

		if (revealed == mappedWords.Count) {
			foreach (var w in selectedWords) {
				if (!wordsFound.Contains (w)) {
					//SHOW GAME OVER
					gameActive = false;
					spelling.text = "Game Over";
				}
			}
		}
	}

	IEnumerator GameTimer () {
		while (true) {
			yield return new WaitForSeconds (10);
			GiveRandomHint ();
		}
	}
}
