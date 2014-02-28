package net.co.java.model;

import java.util.HashMap;

import net.co.java.entity.Player;
import net.co.java.item.EquipmentSlot;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemPrototype;

/**
 * Administrators of this private server should be able to use a database of their own preference,
 * for instance using MySQL, PostgreSQL, SQLLite, using Java serialization or static mock data.
 * This interface is a stub for models that provide access to the data model.
 * @author Jan-Willem Gmelig Meyling
 *
 */
public interface Model {
	
	/**
	 * Check if the account credentials are correct
	 * @param server
	 * @param accountName
	 * @param password
	 * @return true if the user is authorized
	 */
	boolean isAuthorised(String server, String accountName, String password) throws AccessException;
	
	/**
	 * Create an AuthorizationPromise
	 * @param accountName
	 * @return
	 * @throws AccessException
	 */
	AuthorizationPromise createAuthorizationPromise(String accountName) throws AccessException;
	
	/**
	 * Get an AuthorizationPromise
	 * @param accountName
	 * @return
	 * @throws AccessException
	 */
	AuthorizationPromise getAuthorizationPromise(Long identity) throws AccessException;
	
	/**
	 * Load a user when the identity is sent for the first time to the game server
	 * @param promise
	 * @return
	 * @throws AccessException
	 */
	public Player loadPlayer(AuthorizationPromise promise) throws AccessException;
	
	/**
	 * Get the player object for a given identity
	 * @param identity
	 * @return Player
	 */
	Player getPlayer(Long identity);
	
	/**
	 * @param player
	 * @return the inventory for a player
	 */
	ItemInstance[] getInventory(Player player) throws AccessException;
	
	/**
	 * @param player
	 * @return the equipments for a player
	 */
	HashMap<EquipmentSlot, ItemInstance.EquipmentInstance> getEquipments(Player player) throws AccessException;
	
	/** 
	 * @param staticID
	 * @return get an ItemPrototype
	 */
	ItemPrototype getItemPrototype(long staticID) throws AccessException;
	
	/**
	 * @param id
	 * @return an ItemInstance
	 */
	ItemInstance getItemInstance(long id) throws AccessException;
	
}
