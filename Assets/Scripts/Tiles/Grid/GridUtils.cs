#define EIGHT_DIRECTIONAL
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public struct ProcessedGrid {
	public List<List<char>> grid;
	public List<string> words;
	public List<List<Vector2>> positions;
	public int columns;
	public int rows;
}

public class GridUtils  {

	static Hashtable results;
	static string searchKey;
	static ProcessedGrid gridToProcess;

	public static List<int> FindWord ( string w, string g, int rows, int cols) {
		
		CollectWord (w, g, rows, cols);

		if (results.ContainsKey (w)) {
			return results [w] as List<int>;
		}
		return null;
	}


	public static ProcessedGrid SolveGrid (ref List<string> dictionary, string g, int rows, int cols) {

		results = new Hashtable ();
		Debug.Log (g);

		gridToProcess = new ProcessedGrid();
		gridToProcess.columns = cols;
		gridToProcess.rows = rows;
		gridToProcess.grid = CharGrid (g, rows, cols);
		gridToProcess.positions = new List<List<Vector2>>();
		gridToProcess.words = new List<string>();


		var matchString = new StringBuilder ();
		foreach (var row in gridToProcess.grid) {
			foreach (var c in row) {
				matchString.Append(c);
			}
		}

		Regex Validator = new Regex(@"^["+matchString.ToString()+"]+$");

		foreach (var word in dictionary) {

			var length = word.Length;

			if( Validator.IsMatch(word)  ) {
				SearchWord (word);
			}
		}


		foreach (DictionaryEntry entry  in results) {
			gridToProcess.words.Add ((string)entry.Key);
			var wpositions = new List<Vector2> ();
			foreach(var v in (List<Vector2>) entry.Value) {
				wpositions.Add (v);
			}
			gridToProcess.positions.Add (wpositions);
		}

		return gridToProcess;
	}

	static void CollectWord (string word, string g, int rows, int cols) {

		results = new Hashtable ();

		gridToProcess = new ProcessedGrid();
		gridToProcess.columns = cols;
		gridToProcess.rows = rows;
		gridToProcess.grid = CharGrid (g, rows, cols);
		gridToProcess.positions = new List<List<Vector2>>();
		gridToProcess.words = new List<string>();


		searchKey = word;
		SearchWord (searchKey);

		foreach (DictionaryEntry entry  in results) {
			gridToProcess.words.Add ((string)entry.Key);

			var wpositions = new List<Vector2> ();
			foreach(var v in (List<Vector2>) entry.Value) {
				wpositions.Add (v);
			}
			gridToProcess.positions.Add (wpositions);
		}
	}

	static List<List<char>> CharGrid (string gridString, int rows, int columns) {

		var result = new List<List<char>> ();

		var index = 0;
		while (index < rows) {
			result.Add (new List<char>());
			index++;
		}

		index = 0;
		var i = 0;

		while (i < gridString.Length) {
			var c = gridString[i];
			//if your grid has a gap, represented here by a dash, replace it with a symbol which is not found in words (a number, for instance)
			//if (c == '-') c = '2';
			result[index].Add(c);
			i++;
			if (i != 0 && i % columns == 0)
				index++;
		}


		return result;
	}


	static void SearchWord(string w) {
		searchKey = w;
		// from each cell of the matrix, Search for the pattern!
		for (int i = 0; i < gridToProcess.grid.Count; i++) {
			for (int j = 0; j < gridToProcess.grid[i].Count; j++) {
				List<List<int>> board = GetTraverseBoard();
				Traverse(0, i, j, board);
			}
		}
	}

	static void Traverse(int depth, int row, int column, List<List<int>> board) {

		if (gridToProcess.grid[row][column] != searchKey[depth])
			return;

		if (board == null)
			return;

		depth++;
		if (depth == searchKey.Length) { // if the word has been found

			if (!results.ContainsKey(searchKey)) {
				board[row][column] = depth;
				results.Add (searchKey, PrintCoordinates(board));
				board[row][column] = 0;
			}
			return;
		} else { 
			board[row][column] = depth;

			if (row - 1 >= 0 && column - 1 >= 0
				&& board[row - 1][column - 1] == 0) {
				Traverse(depth, row - 1, column - 1, board);
			}

			if (column - 1 >= 0 && board[row][column - 1] == 0) {
				Traverse(depth, row, column - 1, board);
			}
			if (row + 1 < board.Count && column - 1 >= 0
				&& board[row + 1][column - 1] == 0) {
				Traverse(depth, row + 1, column - 1, board);
			}
			if (row + 1 < board.Count &&  board[row + 1].Count > column && board[row + 1][column] == 0) {
				Traverse(depth, row + 1, column, board);
			}
			if (row + 1 < board.Count && column + 1 < board[row + 1].Count
				&& board[row + 1][column + 1] == 0) {
				Traverse(depth, row + 1, column + 1, board);
			}
			if (column + 1 < board[row].Count
				&& board[row][column + 1] == 0) {
				Traverse(depth, row, column + 1, board);
			}
			if (row - 1 >= 0 && column + 1 < board[row - 1].Count 
				&& board[row - 1][column + 1] == 0) {
				Traverse(depth, row - 1, column + 1, board);
			}
			if (row - 1 >= 0 && board[row - 1][column] == 0) {
				Traverse(depth, row - 1, column, board);
			}
			board[row][column] = 0;
		}

	}

	static List<Vector2> PrintCoordinates(List<List<int>> boolBoard) {
		int start = 1;
		int end = searchKey.Length + 1;

		var result = new List<Vector2> ();

		for (int count = start; count < end; count++) {
			for (int i = 0; i < boolBoard.Count; i++) {
				bool isFound = false;
				for (int j = 0; j < boolBoard[i].Count; j++) {
					if(boolBoard[i][j] == count){
						result.Add (new Vector2(i, j));
						isFound = true;
						break;
					}
				}
				if(isFound)break;
			}
		}

		return result;
	}

	static List<List<int>> GetTraverseBoard() {
		List<List<int>> res = new List<List<int>> ();

		for (int i = 0; i < gridToProcess.grid.Count; i++) {
			var row = new List<int>();
			for (int j = 0; j < gridToProcess.grid[i].Count; j++) {
				row.Add(0);
			}
			res.Add (row);
		}
		return res;
	}
		
}
