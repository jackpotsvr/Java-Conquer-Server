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
	public boolean hasCharacter(String server, String username) {
		// TODO Auto-generated method stub
		return false;
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
	public long getIdentity(String character) throws AccessException {
		Long identity = this.createPlayerIdentity();
		Player player = new Player(identity, character, null, 500);
		player.setMesh(381004);
		player.setHairstyle(315);
		player.setGold(1111);
		player.setCps(215);
		player.setExperience(34195965);
		player.setStrength(51);
		player.setDexterity(50);
		player.setVitality(50);
		player.setSpirit(50);
		player.setAttributePoints(200);
		player.setHP(500);
		player.setPkPoints(10);
		player.setMana(120);
		player.setLevel(130);
		player.setRebornCount(0);
		player.setProfession(15);
		this.players.put(identity, player);
		return identity;
	}

}
