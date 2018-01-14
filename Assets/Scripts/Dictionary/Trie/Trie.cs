using UnityEngine;

public class Trie  {

	private Node root;
	
	public Trie()
	{
		this.root = new Node();
	}

	public bool Insert(string key)
	{
		return root.Insert(key.ToLower());
	
	}
	

	public bool ContainPrefix(string prefix)
	{
		return root.ContainPrefix(prefix.ToLower());
	}
	

	public bool ContainWord(string key)
	{
		return root.ContainWord(key.ToLower());
	}
}
