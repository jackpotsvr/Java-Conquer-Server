package net.co.java.model;

import java.util.HashMap;

import net.co.java.entity.Player;
import net.co.java.item.EquipmentSlot;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemPrototype;

/**
 * Administrators of this private server should be able to use a database of their own preference,
 * for instance using MySQL, Postgres, SQLLite, using Java serialisation or static mock data.
 * This interface is a stub for models that provide access to the data model.
 * @author Jan-Willem Gmelig Meyling
 *
 */
public interface Model {
	
	/**
	 * Check if a user is authorised
	 * @param server
	 * @param accountName
	 * @param password
	 * @return true if the user is authorised
	 */
	boolean isAuthorised(String server, String accountName, String password) throws AccessException;
	
	/**
	 * Check if a user has a character (else, character creation)
	 * @param server
	 * @param username
	 * @return true if the user has a character
	 */
	boolean hasCharacter(String server, String username) throws AccessException;
	
	/**
	 * Create a Player object and return the identity at which the Player
	 * instance is stored
	 * @param character
	 * @return
	 * @throws AccessException
	 */
	long getIdentity(String character) throws AccessException;
	
	/**
	 * Get the player object for a username
	 * @param client
	 * @param username
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
