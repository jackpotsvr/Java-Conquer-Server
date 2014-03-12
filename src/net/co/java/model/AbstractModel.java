package net.co.java.model;

import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

import org.simpleframework.xml.Root;

import net.co.java.entity.Player;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemInstance.EquipmentInstance;
import net.co.java.item.ItemPrototype;
import net.co.java.item.ItemPrototype.EquipmentPrototype;

/**
 * The AbstractModelis an abstract Model providing caching for the identities of
 * players, items and item prototypes.
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
@Root
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
	public ItemInstance getItemInstance(long id) throws AccessException {
		ItemInstance it = itemInstances.get(id);
		if ( it == null )
			it = fetchItemInstance(id);
		return it;
	}
	
	@Override
	public EquipmentInstance getEquipmentInstance(long id) throws AccessException {
		return (EquipmentInstance) getItemInstance(id);
	}

	@Override
	public AuthorizationPromise getAuthorizationPromise(Long identity) {
		return authPromises.remove(identity);
	}

	@Override
	public Player getPlayer(Long id) {
		return players.get(id);
	}
	
	@Override
	public Player loadPlayer(AuthorizationPromise promise) throws AccessException {
		Player hero = fetchPlayer(promise);
		fetchInventory(hero);
		fetchEquipment(hero);
		fetchSkill(hero);
		fetchProficiency(hero);
		players.put(hero.getIdentity(), hero);
		return hero;
	}
	
	/**
	 * Used to load a Player instance from the model
	 * @param promise
	 * @return a Player instance based on the data in the model
	 * @throws AccessException
	 */
	protected abstract Player fetchPlayer(AuthorizationPromise promise) throws AccessException;
	
	/**
	 * Used to load the inventory content for a Player from the model
	 * @param hero
	 * @throws AccessException
	 */
	protected abstract void fetchInventory(Player hero) throws AccessException;
	
	/**
	 * Used to load the equipments for a Player from the model
	 * @param hero
	 * @throws AccessException
	 */
	protected abstract void fetchEquipment(Player hero) throws AccessException;
	
	/**
	 * Used to load the skills for a Player from the model
	 * @param hero
	 * @throws AccessException
	 */
	protected abstract void fetchSkill(Player hero) throws AccessException;
	
	/**
	 * Used to load the Weapon Proficiencies for a Player from the model
	 * @param hero
	 * @throws AccessException
	 */
	protected abstract void fetchProficiency(Player hero) throws AccessException;

	/**
	 * Fetch an ItemPrototype from the data model when not existing in the
	 * memory
	 * 
	 * @param id
	 *            for the ItemPrototype
	 * @return {@code ItemPrototype}
	 * @throws AccessException
	 */
	protected abstract ItemPrototype fetchItemPrototype(long id) throws AccessException;
	
	/**
	 * Fetch an ItemInstance from the data model when not existing in the memory
	 * @param id
	 * @return ItemInstance
	 * @throws AccessException
	 */
	protected abstract ItemInstance fetchItemInstance(long id) throws AccessException;

}
