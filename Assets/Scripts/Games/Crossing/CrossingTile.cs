using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossingTile : GridTile {

	public enum GRID_TYPE{
		WORD_GRID,
		PANEL_GRID
	}
		
	public enum TILE_TYPE {
		EMPTY,
		GAP,
		PLACED,
		CLUE,
		TEMPORARY,
		BUTTON
	}

	public SpriteRenderer outline;

	[HideInInspector]
	public GRID_TYPE gridType;

	[HideInInspector]
	public TILE_TYPE tileType;

	public Color wrongColor = Color.red;
	public Color placedColor = Color.white;
	public Color gapColor = Color.blue;
	private Vector2 localPosition;

	void Start () {
		localPosition = transform.localPosition;
	}
		

	public bool IsMovable () {

		if (tileType == TILE_TYPE.PLACED)
			return true;
		if (tileType == TILE_TYPE.BUTTON)
			return true;
		
		return false;
	}

	public void ShowTemporary() {
		gameObject.SetActive (true);
		outline.gameObject.SetActive (false);
		tileBg.SetActive (true);
		SetColor (gapColor, Color.white);
		tileType = TILE_TYPE.TEMPORARY;
	}

	public void ShowGap() {
		gameObject.SetActive (true);
		outline.gameObject.SetActive (true);
		tileBg.SetActive (false);
		foreach (var c in charsGO)
			c.SetActive (false);
		outline.color = Color.blue;
		tileType = TILE_TYPE.GAP;
	}

	public void ShowFixed () {
		gameObject.SetActive (true);
		outline.gameObject.SetActive (false);
		tileBg.SetActive (true);
		SetColor (placedColor, Color.black);
		tileType = TILE_TYPE.CLUE;
	}

	public void ShowPlaced () {
		gameObject.SetActive (true);
		outline.gameObject.SetActive (false);
		tileBg.SetActive (true);
		SetColor (gapColor, Color.white);
		tileType = TILE_TYPE.PLACED;
	}

	public void ShowButton () {
		gameObject.SetActive (true);
		outline.gameObject.SetActive (false);
		tileBg.SetActive (true);
		SetColor (gapColor, Color.white);
		tileType = TILE_TYPE.BUTTON;
	}


	public void ShowWrong () {

		if (tileType != TILE_TYPE.PLACED)
			return;
		
		gameObject.SetActive (true);
		outline.gameObject.SetActive (false);
		tileBg.SetActive (true);
		SetColor (wrongColor, Color.white);
	}

	public void ResetPosition () {
		transform.localPosition = localPosition;
	}

}
