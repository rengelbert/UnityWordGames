using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Utils {

	public static T[] Scramble<T> (T[] array) {
		 
		// shuffle chars
		for (int t = 0; t < array.Length; t++ )
		{
			T tmp = array[t];
			int r = UnityEngine.Random.Range(t, array.Length);
			array[t] = array[r];
			array[r] = tmp;
		}
		return array;
	}

	public static List<T> Scramble<T> (List<T> list) {

		// shuffle chars
		for (int t = 0; t < list.Count; t++ )
		{
			T tmp = list[t];
			int r = UnityEngine.Random.Range(t, list.Count);
			list[t] = list[r];
			list[r] = tmp;
		}
		return list;
	}


	public static void DelayAndCall (MonoBehaviour caller, float delay, Action callBack) {
		caller.StartCoroutine (DelayAndCallRoutine (delay, callBack));
	}

	public static void StaggerAndCall<T> (MonoBehaviour caller, float delay, Action<T> callBack, List<T> items) {
		caller.StartCoroutine (StaggerAndCallRoutine<T> (delay, callBack, items));
	}

	static IEnumerator StaggerAndCallRoutine<T>  (float delay, Action<T> callBack, List<T> items) {
		var i = 0;
		while (i < items.Count) {
			callBack (items[i]);
			yield return new WaitForSeconds (delay);
			i++;
		}
	}

	static IEnumerator DelayAndCallRoutine  (float delay, Action callBack) {
		yield return new WaitForSeconds (delay);
		callBack ();
	}
}
