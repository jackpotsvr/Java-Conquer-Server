package net.co.java.entity.view;

import java.util.List;

import net.co.java.entity.Entity;
import net.co.java.entity.Location;
import net.co.java.entity.Player;

/**
 * The View interface is a collection that contains all Entities within the
 * view range of this Entity
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public interface View {
	
	/**
	 * Add an {@code Entity} to the {@code View} for this {@code Entity}
	 * @param entity
	 */
	void add(Entity entity);
	
	/**
	 * @param entity
	 * @return true if the {@code Entity} is within the {@code View} for this {@code Enitity}
	 */
	boolean contains(Entity entity);
	
	/**
	 * Remvoe an {@code Enitity} from the {@code View} for this {@code Entity}
	 * @param entity
	 */
	void remove(Entity entity);
	
	/**
	 * @return the amount of {@code Enities} within the view of this {@code Enitity}
	 */
	int size();
	
	/**
	 * @return a list containing all {@code Entities} within the view of this {@code Entity}
	 */
	List<Entity> getEntities();
	
	/**
	 * 
	 * @return a list containing all {@code Player}s within the view of this {@code Entity}
	 */
	List<Player> getPlayers();
	
	/**
	 * Update the view based on a new location
	 * @param location
	 */
	void update(Location location);
	
}
