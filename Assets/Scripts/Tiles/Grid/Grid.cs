#define EIGHT_DIRECTIONAL
//#define TAP_SELECTION
#define DRAG_SELECTION

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Grid : MonoBehaviour, IInputHandler {


	public int ROWS = 8;

	public int COLUMNS = 6;

	public GameObject gridTileGO;

	public static float GRID_TILE_SIZE;

	[HideInInspector]
	public List<GridTile> tiles;

	private List<List<GridTile>> gridTiles;

	private GridTile selectedTile;

	private List<GridTile> selectedTiles;


	void Awake () {

		selectedTiles = new List<GridTile> ();
		selectedTile = null;
	}

	void Start () {
		if (SceneManager.GetActiveScene ().name == "Scene_Grid_Test") {
			BuildGrid ();
			ShowGridChars ();
		}
	}


	public void ShowGridChars () {
		for (var i = 0; i < tiles.Count; i++) {
			tiles [i].SetTileData (  TileChar.chars [Random.Range (0, TileChar.chars.Length)] );
		}
	}

	public void ShowGridChars (char[] chars) {
		for (var i = 0; i < tiles.Count; i++) {
			tiles [i].SetTileData (  chars[i] );
		}
	}

	public void HandleTouchDown (Vector2 touch) 	{

		selectedTile = TileCloseToPoint (touch);

		if (selectedTile != null) {
			selectedTile.Select(true);
			selectedTiles.Add (selectedTile);
		}
	}

	public void HandleTouchUp (Vector2 touch)
	{

		if (selectedTile == null) {
			#if TAP_SELECTION 
				//if no tile selected submit selected tiles 
				ClearSelection ();
			#else
			return;
			#endif
		}

		ClearSelection ();
		selectedTile = null;

	}


	public void HandleTouchMove (Vector2 touch)
	{

		#if TAP_SELECTION 
		return;
		#endif

		if (selectedTile == null)
			return;


		var nextTile = TileCloseToPoint (touch);


		if (nextTile != null && nextTile != selectedTile && nextTile.touched) 
		{

			if (nextTile.row == selectedTile.row && (nextTile.column == selectedTile.column - 1 || nextTile.column == selectedTile.column + 1))
			{
				selectedTile = nextTile;
			}
			else if (nextTile.column == selectedTile.column && (nextTile.row == selectedTile.row - 1 || nextTile.row == selectedTile.row + 1))
			{
				selectedTile = nextTile;
			}
			#if EIGHT_DIRECTIONAL
			else if (Mathf.Abs (nextTile.column - selectedTile.column) == 1 &&  Mathf.Abs (nextTile.row - selectedTile.row) == 1) 
			{
				selectedTile = nextTile;
			}
			#endif

			selectedTile.Select(true);

			if (!selectedTiles.Contains (selectedTile))
				selectedTiles.Add (selectedTile);
			else {
				//deselect tile if it's already been added to selected tiles
				var index = selectedTiles.IndexOf(nextTile);
				for(var i = selectedTiles.Count -1; i >= 0; i--)
				{
					var tile = selectedTiles[i];
					if (i > index) {
						tile.Select(false);
						selectedTiles.RemoveAt(i);
					}
				}
				selectedTile = selectedTiles[selectedTiles.Count - 1];
			}

		}
	}

	private GridTile TileCloseToPoint (Vector2 point)
	{
		var t = Camera.main.ScreenToWorldPoint (point);
		t.z = 0;

		int c = Mathf.FloorToInt ((t.x - gridTiles[0][0].transform.position.x + ( GRID_TILE_SIZE * 0.5f )) / GRID_TILE_SIZE);
		if (c < 0)
			return null;
		if (c >= COLUMNS)
			return null;

		int r =  Mathf.FloorToInt ((gridTiles[0][0].transform.position.y + ( GRID_TILE_SIZE * 0.5f ) - t.y ) /  GRID_TILE_SIZE);

		if (r < 0) return null;

		if (r >= ROWS) return null;

		if (gridTiles.Count <= r)
			return null;

		if (gridTiles[r].Count <= c)
			return null;

		if (!gridTiles[r][c].gameObject.activeSelf) return null;

		return gridTiles[r][c]; 

	}

	public void BuildGrid ()
	{
		if (tiles != null && tiles.Count != 0) {
			foreach (var t in tiles)
				Destroy (t.gameObject);

			transform.localScale = Vector2.one;
			transform.localPosition = Vector2.zero;
		}

		//a one dimensional list of tiles we can shuffle
		tiles = new List<GridTile> ();

		//the two dimensional grid
		gridTiles = new List<List<GridTile>> ();


		for (int row = 0; row < ROWS; row++) {

			var rowsTiles = new List<GridTile>();

			for (int column = 0; column < COLUMNS; column++) {

				var item = Instantiate (gridTileGO) as GameObject;
				var tile = item.GetComponent<GridTile>();
				tile.SetTilePosition(row, column);
				tile.transform.parent = gameObject.transform;

				tiles.Add (tile);

				rowsTiles.Add (tile);
			}
			gridTiles.Add(rowsTiles);
		}

		ScaleGrid ( Mathf.Abs (gridTiles [0] [0].transform.localPosition.y - gridTiles [1] [0].transform.localPosition.y));

	}

	private void ScaleGrid ( float tileSize) {

		GRID_TILE_SIZE = tileSize;

		var stageWidth = 4.0f;
		var stageHeight = 4.8f;

		var gridWidth = (COLUMNS - 1) * GRID_TILE_SIZE;
		var gridHeight = (ROWS - 1) * GRID_TILE_SIZE;

		var scale = 1.0f;


		if (gridWidth > stageWidth || gridHeight > stageHeight) {

			if (gridWidth >= gridHeight) {
				scale = stageWidth / gridWidth;
			} else {
				scale = stageHeight / gridHeight;
			}
			transform.localScale = new Vector2(scale, scale);
		}
		GRID_TILE_SIZE *= scale;
		transform.localPosition = new Vector2 ((gridWidth * scale) * -0.5f , 3.5f - 0.5f * (gridHeight * scale));

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
