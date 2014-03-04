package net.co.java.entity;

public interface Spawnable {

	Location getLocation();
	
	boolean inView(Spawnable spawnable);
	
}
