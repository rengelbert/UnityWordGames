using UnityEngine;
using System.Collections;

[RequireComponent(typeof(IInputHandler))]
public class InputController : MonoBehaviour {

	private IInputHandler handler;
	private bool touchDown;

	void Awake () {
		handler = GetComponent<IInputHandler> ();
	}

	void Update () {

		if (handler == null)
			return;
		
		if (Input.touches.Length > 0) {
			
			Touch touch = Input.touches [0];

			if (touch.phase == TouchPhase.Began) {
				handler.HandleTouchDown (touch.position);
				touchDown = true;
			} else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) {
				touchDown = false;
				handler.HandleTouchUp (touch.position);
			} else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
				handler.HandleTouchMove (touch.position);
			}
			if (touchDown)
				handler.HandleTouchMove (touch.position);	
			
			return;
		} else if (Input.GetMouseButtonDown (0)) {
			handler.HandleTouchDown (Input.mousePosition);
			touchDown = true;
		} else if (Input.GetMouseButtonUp (0)) {
			touchDown = false; 
			handler.HandleTouchUp (Input.mousePosition);
		} else {
			if (touchDown)
				handler.HandleTouchMove (Input.mousePosition);
		}

	}
}
