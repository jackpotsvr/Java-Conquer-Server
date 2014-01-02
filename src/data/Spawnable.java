package data;

public interface Spawnable {

	public Location getLocation();
	
	public boolean inView(Spawnable spawnable);
	
}
