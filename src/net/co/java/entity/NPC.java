package net.co.java.entity;

public class NPC implements Spawnable {
	
	private Location location;

	public NPC(Location location) {
		this.location = location;
	}

	@Override
	public Location getLocation() {
		return location;
	}

	@Override
	public boolean inView(Spawnable spawnable) {
		return location.inView(spawnable.getLocation());
	}

}
