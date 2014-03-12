package net.co.java.model;

import net.co.java.entity.Player;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemInstance.EquipmentInstance;
import net.co.java.item.ItemPrototype;
import net.co.java.item.ItemPrototype.EquipmentPrototype;
import net.co.java.packets.Character_Creation_Packet;
import net.co.java.skill.WeaponProficiency;

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
	 * @return AuthorisationPromise from AuthServer to GameServer
	 * @throws AccessException
	 */
	AuthorizationPromise createAuthorizationPromise(String accountName) throws AccessException;
	
	/**
	 * Get an AuthorizationPromise
	 * @param identity
	 * @return AuthorisationPromise for a given Identity
	 * @throws AccessException
	 */
	AuthorizationPromise getAuthorizationPromise(Long identity) throws AccessException;
	
	/**
	 * Load a user when the identity is sent for the first time to the game server
	 * @param promise
	 * @return Player instance for a given AuthorisationPromise
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
	 * Add a new character in the model
	 * @param ip
	 * @return true if successfully created a character
	 * @throws AccessException if there was an error connecting to the model
	 */
	boolean createCharacter(Character_Creation_Packet ip) throws AccessException;
	
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

	/**
	 * @param staticID
	 * @return an ItemInstance
	 */
	EquipmentPrototype getEquipmentPrototype(long staticID) throws AccessException;

	/**
	 * 
	 * @param id
	 * @return
	 * @throws AccessException
	 */
	EquipmentInstance getEquipmentInstance(long id) throws AccessException;

	/**
	 * 
	 * @param hero
	 * @param wp
	 * @throws AccessException
	 */
	void setProficiency(Player hero, WeaponProficiency wp) throws AccessException;
	
}
