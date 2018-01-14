using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangmanPanel : MonoBehaviour, IInputHandler {

	public GameObject tileGO;

	public GameObject container;

	private List<TileButton> tiles;

	private TileButton selectedTile;


	void Awake () {
		tiles = new List<TileButton> ();
	}

	public void ShowPanel (char[] chars) {

		ClearPanel ();

		container.transform.localScale = Vector2.one;
		container.transform.position = Vector2.zero;

		for (var i = 0; i < chars.Length; i++) {
			var go = Instantiate (tileGO) as GameObject;
			var tile = go.GetComponent<TileButton> ();
			tile.SetTileData (chars [i]);
			tile.SetColor (Color.black, Color.white);
			tile.transform.parent = container.transform;

			tile.transform.position = new Vector2 (
				(i * 3.5f),
				-1.5f
			);

			tiles.Add (tile);
		}

		var size = tiles [1].transform.position.x - tiles [0].transform.position.x;
		var scale = 1.0f;
		var panelWidth = (chars.Length -1) * size;
		var stageWidth = 3.5f;

		if (panelWidth > stageWidth ) {
			scale = stageWidth / panelWidth;
			container.transform.localScale = new Vector2(scale, scale);
		}

		container.transform.localPosition = new Vector2 ((panelWidth * scale) * -0.5f , -0.5f);

	}

	private void ClearPanel () {
		foreach (var tile in tiles)
			Destroy (tile.gameObject);
		tiles.Clear ();
	}


	public void HandleTouchDown (Vector2 touch) 	{

		if (selectedTile != null) {
			selectedTile.Select(false);
		}

		selectedTile = TileCloseToPoint (touch);

		if (selectedTile != null) {
			selectedTile.Select(true);
		}
	}
		
	public void HandleTouchUp (Vector2 touch) {
		if (selectedTile != null) {
			selectedTile.Select(false);
			SubmitTile ();
		}
		selectedTile = null;
	}

	public void HandleTouchMove (Vector2 touch) {
	}

	private TileButton TileCloseToPoint (Vector2 point){
		var t = Camera.main.ScreenToWorldPoint (point);
		t.z = 0;

		var minDistance = 0.6f;
		TileButton closestTile = null;
		foreach (var tile in tiles) {
			var distanceToTouch = Vector2.Distance (tile.transform.position, t);
			if (distanceToTouch < minDistance) {
				minDistance = distanceToTouch;
				closestTile = tile;
			}
		}
			
		return closestTile;
	}

	private void SubmitTile () {
		HangmanGameEvents.LetterSelected (selectedTile.TypeChar);
	}



}
