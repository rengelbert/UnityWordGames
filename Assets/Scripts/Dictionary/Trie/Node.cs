using UnityEngine;
using System.Collections;

public class Node {

	public bool isWord;

	private int LETTERS_IN_ALPHABET = 26;
	private Node[] children;

	public Node()
	{
		isWord = false;
		children = new Node[LETTERS_IN_ALPHABET];
	}
	
	public Node(bool key)
	{
		isWord = key;
		children = new Node[LETTERS_IN_ALPHABET];
	}
	
	public bool Insert(string key)
	{


		//If the key is empty, this node is a key
		if (key.Length == 0)
		{
			if (isWord)
				return false;
			else
			{
				isWord = true;
				return true;
			}

			return false;
		}
		else
		{
			//otherwise, insert in one of its child

			int childNodePosition = key[0] - 'a';
		
			if ( children[childNodePosition] == null)
			{
				
				children[childNodePosition] = new Node();
			
				children[childNodePosition].Insert(key.Substring(1));
				return true;
			}
			else
			{
				return children[childNodePosition].Insert(key.Substring(1));
			}
			return false;
		}
	}
	

	public bool ContainPrefix(string prefix)
	{
		//If the prefix is empty, return true
		if (prefix.Length == 0)
		{
			return true;
		}
		else
		{//otherwise, check in one of its child
			int childNodePosition = prefix[0] - 'a';
			return children[childNodePosition] != null && children[childNodePosition].ContainPrefix(prefix.Substring(1));
		}
	}

	public bool ContainWord(string word)
	{
		//If the prefix is empty, return true
		if (word.Length == 0)
		{
			return isWord;
		}
		else
		{//otherwise, check in one of its child
			int childNodePosition = word[0] - 'a';
			return children[childNodePosition] != null && children[childNodePosition].ContainWord(word.Substring(1));
		}
	}
}
