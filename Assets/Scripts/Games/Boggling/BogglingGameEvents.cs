using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class BogglingGameEvents {

	public delegate void Event_SelectTile (string word);
	public delegate void Event_SelectWord (string word, List<GridTile> tiles);
	public delegate void Event_GameLoaded ();

	public static event Event_SelectTile OnTileSelected;
	public static event Event_SelectWord OnWordSelected;
	public static event Event_GameLoaded OnGameLoaded;


	public static void TileSelected (string word) {
		if (OnTileSelected != null)
			OnTileSelected (word);
	}

	public static void WordSelected (string word, List<GridTile> tiles) {
		if (OnWordSelected != null)
			OnWordSelected (word, tiles);
	}

	public static void GameLoaded () {
		if (OnGameLoaded != null)
			OnGameLoaded ();
	}
}