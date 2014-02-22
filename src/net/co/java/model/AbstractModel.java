package net.co.java.model;

import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

import net.co.java.entity.Player;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemPrototype;

/**
 * The AbstractModelis an abstract Model providing caching for the identities of
 * players, items and item prototypes.
 * 
 * @author Jan-Willem Gmelig Meyling
 * 
 */
public abstract class AbstractModel implements Model {

	/** Identities mapped to players for auth -> game server redirection */
	protected final Map<Long, Player> players = Collections.synchronizedMap(new HashMap<Long, Player>());

	/** Cached item prototypes for used items */
	protected final Map<Long, ItemPrototype> itemPrototypes = Collections.synchronizedMap(new HashMap<Long, ItemPrototype>());

	/** Cached item instances for used items */
	protected final Map<Long, ItemInstance> itemInstances = Collections.synchronizedMap(new HashMap<Long, ItemInstance>());
	
	/** AuthorizationPromises */
	protected final Map<Long, AuthorizationPromise> authPromises = Collections.synchronizedMap(new HashMap<Long, AuthorizationPromise>());
	
	private volatile long INCREMENTING_IDENTITY = 0;
	
	/**
	 * @return a player identity between 1000000 and 1999999999
	 */
	public long createPlayerIdentity() {
		return (INCREMENTING_IDENTITY++ % 1999000000) + 1000000;
	}

	/**
	 * Fetch an ItemPrototype from the data model when not existing in the
	 * memory
	 * 
	 * @param id
	 *            for the ItemPrototype
	 * @return {@code ItemPrototype}
	 */
	protected abstract ItemPrototype fetchItemPrototype(long id);

	/**
	 * Fetch an ItemInstance from the data model when not existing in the memory
	 * 
	 * @param id for the ItemInstance
	 * @return {@code ItemInstance}
	 */
	protected abstract ItemInstance fetchItemInstance(long id);
	
	public Player getPlayer(Long id) {
		return players.get(id);
	}

	@Override
	public ItemPrototype getItemPrototype(long staticID) {
		if (itemPrototypes.containsKey(staticID)) {
			return itemPrototypes.get(staticID);
		}
		return fetchItemPrototype(staticID);
	}

	@Override
	public ItemInstance getItemInstance(long id) {
		if (itemInstances.containsKey(id)) {
			return itemInstances.get(id);
		}
		return fetchItemInstance(id);
	}
	
	@Override
	public AuthorizationPromise getAuthorizationPromise(Long identity) {
		return authPromises.remove(identity);
	}

}
