using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScrambleGameEvents {

	public delegate void Event_SelectTile (string word);
	public delegate void Event_SelectWord (string word, List<TileButton> tiles);
	public delegate void Event_DictionaryLoaded ();
	public delegate void Event_PuzzleDataLoaded ();
	public delegate void Event_GameLoaded ();

	public static event Event_SelectTile OnTileSelected;
	public static event Event_SelectWord OnWordSelected;
	public static event Event_DictionaryLoaded OnDictionaryLoaded;
	public static event Event_PuzzleDataLoaded OnPuzzleDataLoaded;
	public static event Event_GameLoaded OnGameLoaded;


	public static void TileSelected (string word) {
		if (OnTileSelected != null)
			OnTileSelected (word);
	}

	public static void WordSelected (string word, List<TileButton> tiles) {
		if (OnWordSelected != null)
			OnWordSelected (word, tiles);
	}

	public static void DictionaryLoaded () {
		if (OnDictionaryLoaded != null)
			OnDictionaryLoaded ();
	}

	public static void PuzzleDataLoaded () {
		if (OnPuzzleDataLoaded != null)
			OnPuzzleDataLoaded ();
	}

	public static void GameLoaded () {
		if (OnGameLoaded != null)
			OnGameLoaded ();
	}

}
