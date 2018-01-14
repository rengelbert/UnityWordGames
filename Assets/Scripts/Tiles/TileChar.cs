using UnityEngine;
using System;
using System.Collections;

public class TileChar : MonoBehaviour {

	public static char[] chars = new char[] {'a','b','c','d','e','f','g','h','i','j','k','l',
		'm','n','o','p','q','r','s','t','u','v','w','x','y','z'};

	public static char[] vowels = new char[] {'a','e','i','o','u'};

	public static char[] consonants = new char[] {'b','c','d','f','g','h','j','k','l',
		'm','n','p','q','r','s','t','v','w','x','y','z'};
	
	public GameObject[] charsGO;
	
	public GameObject tileBg;

	[HideInInspector]
	public int type;


	public char TypeChar {
		get { return chars [type]; }
		private set{}
	}

	public void SetTileData (char c, bool display = true) 
	{
		charsGO [type].SetActive (false);

		var index = Array.IndexOf (chars, c);
	
		charsGO [index].SetActive (true);

		type = index;

		tileBg.GetComponent<Renderer> ().material.color =  Color.black;

		charsGO [index].GetComponent<Renderer> ().material.color = Color.white;

		gameObject.SetActive (display);
	}

	public void SetColor (Color tileBgColor, Color tileCharColor) {
		tileBg.GetComponent<Renderer> ().material.color = tileBgColor;
		charsGO [type].GetComponent<Renderer> ().material.color = tileCharColor;
	}

		
}
