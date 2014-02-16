package net.co.java.entity;

import net.co.java.server.Server.Map.Location;

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
