using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScramblePanel : MonoBehaviour, IInputHandler {

	public List<TileButton> tiles;

	private TileButton selectedTile;

	private List<TileButton> selectedTiles;


	void Awake () {
		selectedTiles = new List<TileButton> ();
	}

	public void ShowWord (string word) {

		ClearPanel ();

		var scrambledWord = ScrambleDictionary.Instance.ScrambleWord (word);
		var scrambledTiles = new TileButton[word.Length];

		for (var i = 0; i < word.Length; i++) {
			scrambledTiles [i] = tiles [i];
			scrambledTiles [i].SetTileData (scrambledWord [i]);

			if (Random.Range (0, 10) > 5) {
				tiles [i].transform.Rotate (Vector3.forward * Random.Range (-45, 46));
			} else {
				tiles [i].transform.Rotate (Vector3.back * Random.Range (-45, 46));
			}
		}

		scrambledTiles = Utils.Scramble<TileButton> (scrambledTiles);


		Utils.StaggerAndCall<TileButton> (this, 0.06f, (TileButton go) => { go.gameObject.SetActive(true); }, new List<TileButton> (scrambledTiles));
	}

	private void ClearPanel () {
		foreach (var tile in tiles)
			tile.gameObject.SetActive (false);
	}


	public void HandleTouchDown (Vector2 touch) 	{
		selectedTile = TileCloseToPoint (touch);

		if (selectedTile != null) {
			selectedTile.Select(true);
			selectedTiles.Add (selectedTile);
			SubmitTile();
		}
	}
		
	public void HandleTouchUp (Vector2 touch) {
		if (selectedTile == null) {
			return;
		}

		if ( selectedTiles.Count > 2 ) {			
			SubmitWord();
		}
		ClearSelection ();

		selectedTile = null;
	}

	public void HandleTouchMove (Vector2 touch) {
		
		if (selectedTile == null)
			return;


		var nextTile = TileCloseToPoint (touch);


		if (nextTile != null && nextTile != selectedTile && nextTile.touched) {

			selectedTile = nextTile;

			selectedTile.Select(true);

			if (!selectedTiles.Contains(selectedTile)) selectedTiles.Add (selectedTile);

			SubmitTile ();

		}
	}

	private TileButton TileCloseToPoint (Vector2 point){
		var t = Camera.main.ScreenToWorldPoint (point);
		t.z = 0;

		var minDistance = 0.5f;
		TileButton closestTile = null;
		foreach (var tile in tiles) {
			if (!tile.gameObject.activeSelf)
				continue;
			var distanceToTouch = Vector2.Distance (tile.transform.position, t);
			if (distanceToTouch < minDistance) {
				minDistance = distanceToTouch;
				closestTile = tile;
			}
		}
			
		return closestTile;
	}

	private void SubmitTile () {

		char[] word = new char[selectedTiles.Count];
		for (var i = 0; i < selectedTiles.Count; i++) {
			var tile = selectedTiles [i];
			word [i] = tile.TypeChar;
		}
		var s = new string (word);
		Debug.Log("SUBMIT TILES: " + s);
		ScrambleGameEvents.TileSelected (s);
	}

	private void SubmitWord () {

		char[] word = new char[selectedTiles.Count];
		for (var i = 0; i < selectedTiles.Count; i++) {
			var tile = selectedTiles [i];
			word [i] = tile.TypeChar;
		}
		var s = new string (word);
		Debug.Log("SUBMIT WORD: "+ s);
		ScrambleGameEvents.WordSelected (s, selectedTiles);
	}


	private void ClearSelection () {
		foreach (var t in selectedTiles) {
			t.Select (false);
		}

		if (selectedTile != null)
			selectedTile.Select (false);

		selectedTiles.Clear ();
	}
}