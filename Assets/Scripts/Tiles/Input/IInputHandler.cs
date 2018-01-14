using UnityEngine;
public interface IInputHandler  {
	
	void HandleTouchDown (Vector2 touch);
	void HandleTouchUp (Vector2 touch);
	void HandleTouchMove (Vector2 touch);

}
