package net.co.java.server;

import java.util.List;
import java.util.concurrent.CopyOnWriteArrayList;

import net.co.java.entity.Entity;
import net.co.java.entity.NPC;
import net.co.java.packets.PacketType;

/**
 * The Map enum contains all Maps for the current Server
 * Map can be accessed by name (Map.CentralPlain) or id (1002).
 * Maps should be instantiated only once and therefore the Enum
 * singleton pattern is used. Each Map contains a Collection of
 * Entities, and has a constructor for Locations that are in this
 * Map.
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public enum Map {
	
	/* http://www.elitepvpers.com/forum/co2-programming/177758-request-map-id-complete-list.html */
	Desert(1000),
	CentralPlain(1002),
	PhoenixCastle(1011),
	BirdIsland(1015),
	ApeMoutain(1020);
	
	
	private final Integer id;
	
	private final List<Entity> entities = new CopyOnWriteArrayList<Entity>();

	/**
	 * Construct a new Map with a given id
	 * @param id
	 */
	private Map(int id) {
		this.id = Integer.valueOf(id);
		System.out.println("Initializing map: " + this.toString() + " (" + this.id + ")");
	}
	
	/**
	 * @return the id for a Map
	 */
	public int getMapID() {
		return id;
	}
	
	/**
	 * Add an {@code Entity} to this Map, for example due a windspell, portal or spawn.
	 * Send an EntitySpawn packet to surrounding players.
	 * @param entity
	 */
	public void addEntity(Entity entity) {
		if(!entities.contains(entity)) {
			entities.add(entity);
		}
	}
	
	/**
	 * Remove an {@code Entity} from this Map - death or teleport.
	 * Send an Entity Remove packet to surrounding players.
	 * @param entity
	 */
	public void removeEntity(Entity entity) {
		entities.remove(entity);
	}
	
	/**
	 * @return all entities in this map.
	 */
	public List<Entity> getEntities() {
		return entities;
	}
	
	public static Map valueOf(int id)
	{
		for ( Map mp : Map.values() ) {
			if ( mp.id == id )
				return mp;
		}
		return null;
	}
}
