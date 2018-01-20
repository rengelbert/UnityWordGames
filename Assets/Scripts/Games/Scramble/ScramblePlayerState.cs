using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ScramblePlayerState : MonoBehaviour {

	[Serializable]
	public class ScrambleState {
		public int gameLevel;
		public string gameWord;
		public int wordYield;
	}


	private static ScramblePlayerState _instance = null;    

	public static ScramblePlayerState Instance
	{
		get {
			if(_instance == null)
			{
				GameObject instanceGo = new GameObject("PlayerState");
				_instance = instanceGo.AddComponent<ScramblePlayerState> ();
			}

			return _instance;
		}
	}


	public static string PLAYER_STATE_DATA = "PLAYER_STATE_DATA";

	private ScrambleState playerState;

	void Awake() {

		if (_instance == null)
			_instance = this;

		else if (_instance != this)
			Destroy(gameObject);    
	}


	void OnApplicationPause	() {
		SaveState ();
	}

	void OnApplicationQuit () {
		SaveState ();
	}

	void Start () {

		if (string.IsNullOrEmpty (PlayerPrefs.GetString (PLAYER_STATE_DATA))) {

			CreateState ();
	
		} else {
			playerState = JsonUtility.FromJson<ScrambleState> ( PlayerPrefs.GetString (PLAYER_STATE_DATA) );
		}


		ScrambleGameEvents.GameLoaded ();
	}

	void CreateState () {
		playerState = new ScrambleState ();
		playerState.gameLevel = 1;
		playerState.gameWord = string.Empty;
		playerState.wordYield = 3;
	}

	void SaveState () {
		if (playerState == null)
			return;
		PlayerPrefs.SetString (PLAYER_STATE_DATA, JsonUtility.ToJson(playerState) );
		PlayerPrefs.Save();
	}


	public int GetGameLevel () {
		return playerState.gameLevel;
	}

	public string GetGameWord () {
		return playerState.gameWord;
	}

	public int GetWordYield () {
		return playerState.wordYield;
	}

	public void SetGameLevel (int level) {
		playerState.gameLevel = level;
	}

	public void SetGameWord (string word) {
		playerState.gameWord = word;
	}

	public void SetWordYield (int wordYield) {
		playerState.wordYield = wordYield;
	}

	public void ClearData () {
		
		CreateState ();

	}
}

