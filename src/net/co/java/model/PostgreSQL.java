package net.co.java.model;

import java.io.File;
import java.io.FileNotFoundException;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.HashMap;

import net.co.java.entity.Monster;
import net.co.java.entity.Player;
import net.co.java.item.EquipmentSlot;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemInstance.EquipmentInstance;
import net.co.java.item.ItemPrototype.EquipmentPrototype;
import net.co.java.item.ItemPrototype;
import net.co.java.server.Server.Map;

/**
 * The PostgreSQL model is to use the Java Conquer Server with PostgreSQL databases
 * 
 *  @author Thomas Gmelig Meyling
 */
public class PostgreSQL extends AbstractModel {
	
	private final String HOST;
	
	private final String USERNAME;
	
	private final String PASSWORD;
	
	/**
	 * Construct a new PostgreSQL model
	 * @param host
	 * @param username
	 * @param password
	 * @throws FileNotFoundException
	 */
	public PostgreSQL(String host, String username, String password) throws FileNotFoundException {
		this.HOST = host;
		this.USERNAME = username;
		this.PASSWORD = password;
		createSomeStuff();
	}
	
	private void createSomeStuff() throws FileNotFoundException{
		System.out.println("Creating the magical world of Conquer Online");
		// We spawn a BullMessenger in Twin City for testing purposes here
		Map.CentralPlain.addEntity(new Monster(Map.CentralPlain.new Location(378, 343), 564564, "BullMessenger",  112, 117, 55000));
		// Load the item data
		ItemPrototype.read(new File("ini/COItems.txt"));
		// Create an item
		new EquipmentInstance(2342239l, (EquipmentPrototype) ItemPrototype.get(480029l))
			.setFirstSocket(EquipmentInstance.Socket.SuperFury)
			.setSecondSocket(EquipmentInstance.Socket.SuperRainbowGem)
			.setDura(1500).setBless(3).setPlus(7).setEnchant(172);
	}
	
	/**
	 * @return a new SQL connection to work with
	 * @throws SQLException
	 */
	protected Connection getConnection() throws SQLException {
		return DriverManager.getConnection(HOST, USERNAME, PASSWORD);
	}

	@Override
	public boolean isAuthorised(String server, String username, String password) throws AccessException {
		try {
			Connection conn = getConnection();
			PreparedStatement stmt = conn.prepareStatement("SELECT password FROM account WHERE account_username = ?");
			stmt.setString(1, username);
			ResultSet rs = stmt.executeQuery();
			
			if(rs.next()) {
				return rs.getString("password").equals(password);
			} else {
				return false;
			}
		} catch (SQLException e) {
			// Delegate the Exception
			throw new AccessException(e);
		}
		
	}

	@Override
	public ItemInstance[] getInventory(Player player) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public HashMap<EquipmentSlot, EquipmentInstance> getEquipments(Player player) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected ItemPrototype fetchItemPrototype(long id) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected ItemInstance fetchItemInstance(long id) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public AuthorizationPromise createAuthorizationPromise(String accountName)
			throws AccessException {
		Long identity = this.createPlayerIdentity();

		String characterName;

		try {
			Connection conn = getConnection();
			PreparedStatement stmt = conn.prepareStatement("SELECT character_name FROM characters WHERE account_username = ?");
			stmt.setString(1, accountName);
			ResultSet rs = stmt.executeQuery();
			
			if(rs.next()) {
				characterName = rs.getString(1);
			} else {
				characterName = null;
			}
		} catch (SQLException e) {
				throw new AccessException(e);
		}

		AuthorizationPromise promise = new AuthorizationPromise(identity, accountName, characterName);
		this.authPromises.put(identity, promise);
		return promise;
	}

	@Override
	public Player loadPlayer(AuthorizationPromise promise) throws AccessException {
		Player player = new Player(promise.getIdentity(), promise.getCharacterName(), null, 500);
		
		try {
			Connection conn = getConnection();
			PreparedStatement stmt = conn.prepareStatement("SELECT * FROM characters WHERE character_name = ?");
			stmt.setString(1, promise.getCharacterName());
			ResultSet rs = stmt.executeQuery();

			if(rs.next()) {
				player.setLevel(rs.getInt("character_level"));
				player.setExperience(rs.getInt("character_experience"));
				player.setStrength(rs.getInt("character_strength"));
				player.setDexterity(rs.getInt("character_agility"));
				player.setVitality(rs.getInt("character_vitality"));
				player.setSpirit(rs.getInt("character_spirit"));
				player.setAttributePoints(rs.getInt("character_attributepoints"));
				player.setProfession(rs.getInt("character_profession"));
				player.setMesh(rs.getInt("character_mesh"));
				player.setGold(rs.getInt("character_gold"));
				player.setCps(rs.getInt("character_cps"));
				//player.setSpouse(rs.getString(15));
				player.setLocation(Map.CentralPlain.new Location(rs.getInt("character_x"), rs.getInt("character_y")), null);
				player.setHairstyle(rs.getInt("character_hair"));
				player.setRebornCount(rs.getInt("character_reborn"));
				player.setHP(rs.getInt("character_curhp"));
				player.setMana(rs.getInt("character_curmp"));
				this.players.put(promise.getIdentity(), player);
		
			}	
		} catch (SQLException e) {
			throw new AccessException(e);
		}
		return player;	
	}

}
