package net.co.java.entity;

import net.co.java.server.Server.Map.Location;

public interface Spawnable {

	public Location getLocation();
	
	public boolean inView(Spawnable spawnable);
	
}
