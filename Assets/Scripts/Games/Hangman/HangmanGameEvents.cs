using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HangmanGameEvents {

	public delegate void Event_SelectLetter (char letter);
	public delegate void Event_PuzzleDataLoaded ();
	public delegate void Event_GameLoaded ();

	public static event Event_SelectLetter OnLetterSelected;
	public static event Event_PuzzleDataLoaded OnPuzzleDataLoaded;
	public static event Event_GameLoaded OnGameLoaded;


	public static void LetterSelected (char letter) {
		if (OnLetterSelected != null)
			OnLetterSelected (letter);
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
