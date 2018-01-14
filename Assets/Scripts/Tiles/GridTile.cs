using UnityEngine;
using System;
using System.Collections;


public class GridTile : TileButton {

	[HideInInspector]
	public int row;

	[HideInInspector]
	public int column;


	public void SetTilePosition ( int row, int column)
	{
		
		var size = tileBg.GetComponent<SpriteRenderer> ().bounds.size.x;
		this.column = column;
		this.row = row;
		var tilePosition = new Vector2 ( (column * size) ,  (-row * size));
		transform.position = tilePosition;

		foreach (var go in charsGO) {
			go.SetActive(false);
		}

		Select (false);
	}


}
