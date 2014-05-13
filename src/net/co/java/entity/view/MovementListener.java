package net.co.java.entity.view;

import net.co.java.entity.Entity;
import net.co.java.entity.Location;

public interface MovementListener {
	
	void movedTo(Entity entitiy, Location location);
	
}
