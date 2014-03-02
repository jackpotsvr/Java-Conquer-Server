package net.co.java.model;

import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

import net.co.java.entity.Player;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemPrototype;
import net.co.java.item.ItemPrototype.EquipmentPrototype;

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
	protected abstract ItemPrototype fetchItemPrototype(long id) throws AccessException;
	
	public Player getPlayer(Long id) {
		return players.get(id);
	}

	@Override
	public ItemPrototype getItemPrototype(long staticID) throws AccessException {
		if (itemPrototypes.containsKey(staticID)) {
			return itemPrototypes.get(staticID);
		}
		ItemPrototype proto = fetchItemPrototype(staticID);
		itemPrototypes.put(staticID, proto);
		return proto;
	}
	
	@Override
	public EquipmentPrototype getEquipmentPrototype(long staticID) throws AccessException {
		return (EquipmentPrototype) getItemPrototype(staticID);
	}

	@Override
	public ItemInstance getItemInstance(long id) {
		return itemInstances.get(id);
	}
	
	@Override
	public AuthorizationPromise getAuthorizationPromise(Long identity) {
		return authPromises.remove(identity);
	}

}
