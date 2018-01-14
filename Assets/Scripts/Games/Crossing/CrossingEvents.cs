using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossingEvents  {

	public delegate void Event_GameLoaded ();

	public static event Event_GameLoaded OnGameLoaded;

	public static void GameLoaded () {
		if (OnGameLoaded != null)
			OnGameLoaded ();
	}
}
