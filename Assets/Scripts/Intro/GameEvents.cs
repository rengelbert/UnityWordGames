public class GameEvents  {
	public delegate void Event_UpdateDucks (int cnt);
	public static event Event_UpdateDucks DucksUpdated;
	public static void OnDucksUpdated (int cnt) {
		if (DucksUpdated != null)
			DucksUpdated (cnt);
	}
}