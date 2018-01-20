using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crossing : MonoBehaviour, IInputHandler {

	public CrossingGrid wordGrid;

	public CrossingGrid panelGrid;

	public Text wellDone;

	private Vector3 touchPosition;

	private CrossingTile selectedTile;

	private List<PuzzleWord> puzzle;

	private PuzzleWord mysteryWord;

	private int[] wordLen = new int[] { 4,4,4,5,5,5,5,5,5,5,6,6,6,6,6,6,6,6,6,6,6,6,7,7,7,7,7,7,7,7,7,7,7,7,7,7,8 };

	private int difficultyIndex = 0;

	private CrossingTile tempTileOrigin;

	void Start () {

		CrossingEvents.OnGameLoaded += CrossingEvents_OnGameLoaded;
		CrossingDictionary.Instance.Initialize ();

	}

	void CrossingEvents_OnGameLoaded ()
	{
		NewPuzzle ();
	}


	public void Refresh () {

		var buttonChars = new List<char> ();
		foreach (var p in puzzle) {
			var chars = p.word.ToCharArray ();
			for (var n = 0; n < p.wordTiles.Count; n++ ) {
				var tile = p.wordTiles [n];
				if (tile.tileType == CrossingTile.TILE_TYPE.GAP || tile.tileType == CrossingTile.TILE_TYPE.PLACED) {
					buttonChars.Add (chars [n]);
					tile.ShowGap ();
				} else if (tile.tileType == CrossingTile.TILE_TYPE.CLUE) {
					tile.SetTileData (chars [n]);
					tile.ShowFixed ();
				}
			}
		}

		panelGrid.ClearTiles ();
		panelGrid.ShowRowChars (Utils.Scramble<char> (buttonChars));
	}


	void NewPuzzle () {

		wordGrid.BuildGrid ();
		panelGrid.BuildGrid ();
		puzzle = new List<PuzzleWord> ();

		SelectMysteryWord ();
		difficultyIndex++;
	}

	void SelectMysteryWord () {
		var len = difficultyIndex >= wordLen.Length ? wordLen[wordLen.Length -1] : wordLen [difficultyIndex];

		string word = CrossingDictionary.Instance.RandomUniqueWord (len);

		Debug.Log ("MYSTERY: " + word);

		mysteryWord = new PuzzleWord (word, wordGrid.GetRowTiles(word.Length, 3) );
		var chars = mysteryWord.word.ToCharArray ();
		for (var n = 0; n < mysteryWord.wordTiles.Count; n++) {
			mysteryWord.wordTiles [n].SetTileData (chars [n]);
			mysteryWord.wordTiles [n].ShowFixed ();
			mysteryWord.wordTiles [n].SetColor (Color.white, Color.black);
		}


		SelectCrossWords ();
	}

	void SelectCrossWords () {
		var shuffledTiles = new List<CrossingTile> ();
		shuffledTiles.AddRange (mysteryWord.wordTiles);
		shuffledTiles = Utils.Scramble<CrossingTile> (shuffledTiles);

		var num = shuffledTiles.Count - 2;
		puzzle.Clear ();

		var i = 0;
		while (puzzle.Count < num) {

			var tile = shuffledTiles [i];
			var tileChar = tile.TypeChar;
			var tiles = wordGrid.GetColumnTiles (3, tile.column);
			var pattern = GetVerticalCrossPattern (tiles);
			var words = CrossingDictionary.Instance.MatchesAPattern (pattern);
			if (words != null && words.Count != 0) {
				var word = words [Random.Range (0, words.Count)];
				var puzzleWord = new PuzzleWord (word, tiles);
				puzzle.Add(puzzleWord);
			}
			i++;
			if (i == shuffledTiles.Count)
				i = 0;
		}

		var buttonChars = new List<char> ();

		foreach (var p in puzzle) {
			var chars = p.word.ToCharArray ();
			var hints = Random.Range(0, 2);
			Debug.Log ("CROSS: "+ p.word);
			for (var n = 0; n < p.wordTiles.Count; n++ ) {
				if (!p.wordTiles [n].gameObject.activeSelf) {
					
					if (hints > 0 && Random.Range (0, 10) > 4) {
						p.wordTiles [n].SetTileData (chars [n]);
						p.wordTiles [n].ShowFixed ();
						hints--; 
					} else {
						buttonChars.Add (chars [n]);
						p.wordTiles [n].ShowGap ();
					}

				}
			}
		}
			


		buttonChars = Utils.Scramble<char> (buttonChars);
		panelGrid.ShowRowChars (buttonChars);
	}

	char[] GetVerticalCrossPattern (List<CrossingTile> tiles) {
		var result = new char[tiles.Count];
		for (var i = 0; i < tiles.Count; i++) {
			if (!tiles[i].gameObject.activeSelf)
				result [i] = '-';
			else 
				result [i] = tiles[i].TypeChar;
		}
		return result;
	}

	public void HandleTouchDown (Vector2 touch) {

		ClearSelection ();

		touchPosition = Camera.main.ScreenToWorldPoint (touch);
		touchPosition.z = 0;


		//check panel grid
		var tile = panelGrid.TileCloseToPoint (touchPosition);


		if (tile == null || !tile.gameObject.activeSelf ) {
			
			//check word grid
			tile = wordGrid.TileCloseToPoint (touchPosition);
			if (tile != null && tile.gameObject.activeSelf && tile.IsMovable()) {
				//pick tile from panel
				var tempTile = Instantiate(panelGrid.gridTileGO) as GameObject;
				tempTileOrigin = tile;

				selectedTile = tempTile.GetComponent<CrossingTile> ();
				selectedTile.transform.localScale = panelGrid.transform.localScale;
				selectedTile.transform.parent = wordGrid.transform;
				selectedTile.transform.localPosition = tile.transform.localPosition;
				selectedTile.gridType = CrossingTile.GRID_TYPE.WORD_GRID;
				selectedTile.SetTileData (tile.TypeChar);
				selectedTile.ShowTemporary ();

				tile.ShowGap ();
			}

		} else {
			selectedTile = tile;
		}

		if (selectedTile != null) selectedTile.Select (true);

	}



	public void HandleTouchUp (Vector2 touch) {
		if (selectedTile == null) {
			return;
		}


		if (selectedTile.tileType == CrossingTile.TILE_TYPE.TEMPORARY) {
			
			var target = wordGrid.TileCloseToPoint(touchPosition, false);
			if (target != null && target.gameObject.activeSelf && target.tileType == CrossingTile.TILE_TYPE.GAP) {
				target.SetTileData (selectedTile.TypeChar);
				target.ShowPlaced ();
			} else {
				target = panelGrid.TileCloseToPoint(touchPosition, false);
				if (target != null && !target.gameObject.activeSelf) {
					
					target.SetTileData (selectedTile.TypeChar);
					target.ShowButton ();
				} else {
					tempTileOrigin.SetTileData (selectedTile.TypeChar);
					tempTileOrigin.ShowPlaced ();
				}
			}
			Destroy (selectedTile.gameObject);

		} else if (selectedTile.gridType == CrossingTile.GRID_TYPE.PANEL_GRID) {
			var target = wordGrid.TileCloseToPoint(touchPosition, false);
			if (target != null && target.gameObject.activeSelf && target.tileType == CrossingTile.TILE_TYPE.GAP) {
				target.SetTileData (selectedTile.TypeChar);
				target.ShowPlaced ();
				selectedTile.ResetPosition ();
				selectedTile.gameObject.SetActive (false);
			} else {
				selectedTile.ResetPosition ();
				selectedTile.ShowPlaced ();
			}

		} else {
			var target = wordGrid.TileCloseToPoint(touchPosition, false);
			if (target != null && target.gameObject.activeSelf && target.tileType == CrossingTile.TILE_TYPE.GAP) {
				target.SetTileData (selectedTile.TypeChar);
				selectedTile.ShowGap();	
			} else {
				selectedTile.ResetPosition ();
			}

		}
			
		CheckSolution ();

		ClearSelection ();

	}

	public void HandleTouchMove (Vector2 touch) {
		touchPosition = Camera.main.ScreenToWorldPoint (touch);
		touchPosition.z = 0;
	}


	private void ClearSelection () {

		if (selectedTile != null) {
			selectedTile.selected = false;
			selectedTile = null;
		}
	}


	private void ClearErrors () {
		foreach (var p in puzzle) {
			foreach (var t in p.wordTiles) {
				if (t.tileType == CrossingTile.TILE_TYPE.PLACED) {
					t.ShowPlaced ();
				}
			}
		}
	}



	private void CheckSolution () {
		ClearErrors ();

		bool allCompleted = true;
		bool allWords = true;
		foreach (var p in puzzle) {
			if (!p.IsCompleted ()) {
				allCompleted = false;
			} else {
				if (!p.IsAWord ()) {
					allWords = false;
					p.ShowErrors ();
				}
			}
		}

		if (allCompleted) {
			if (allWords) {
				//SUCCESS
				wellDone.gameObject.SetActive(true);
				Utils.DelayAndCall (this, 2, () => {
					wellDone.gameObject.SetActive(false);
					NewPuzzle();
				});
			}
		}
	}




	void Update () {
		if (selectedTile != null) {
			selectedTile.transform.position = touchPosition;
		}
	}



	private struct PuzzleWord {
		public List<CrossingTile> wordTiles;
		public string word;
		public PuzzleWord (string word, List<CrossingTile> tiles) {
			this.word = word;
			this.wordTiles = tiles;
		}

		public bool IsCompleted () {
			foreach (var t in wordTiles) {
				if (!t.gameObject.activeSelf)
					return false;
				if (t.tileType == CrossingTile.TILE_TYPE.GAP)
					return false;
			}
			return true;
		}

		public bool IsAWord () {
			if (IsCompleted ()) {
				char[] c = new char[wordTiles.Count];
				var i = 0;
				foreach (var t in wordTiles) {
					c [i] = t.TypeChar;
					i++;
				}
				var newWord = new string (c);
				return CrossingDictionary.Instance.IsValidWord (newWord);
			}
			return false;
		}

		public void ShowErrors () {
			foreach (var t in wordTiles) {
				
				t.ShowWrong ();
			}
		}

	}

}

