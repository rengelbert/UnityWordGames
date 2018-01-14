using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleGrid : MonoBehaviour {

	public int ROWS = 4;
	public int COLUMNS = 3;
	public GameObject gridTileGO;
	public static float GRID_TILE_SIZE;
	private List<GridTile> tiles;
	private List<List<GridTile>> gridTiles;


	void Start () {
		BuildGrid ();
	}

	public void BuildGrid ()
	{
		if (tiles != null && tiles.Count != 0) {
			foreach (var t in tiles)
				Destroy (t.gameObject);
			//reset scale
			transform.localScale = Vector2.one;
			transform.localPosition = Vector2.zero;
		}

		//the grid as a one dimensional list of tiles
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
}