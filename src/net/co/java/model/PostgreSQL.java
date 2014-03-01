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
import net.co.java.packets.Character_Creation_Packet;
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
		/*
		ItemPrototype.read(new File("ini/COItems.txt"));
		// Create an item
		
		new EquipmentInstance(2342239l, (EquipmentPrototype) ItemPrototype.get(480029l))
			.setFirstSocket(EquipmentInstance.Socket.SuperFury)
			.setSecondSocket(EquipmentInstance.Socket.SuperRainbowGem)
			.setDura(1500).setBless(3).setPlus(7).setEnchant(172); 
			
		*/ 
		
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
			
			conn.close();
			
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
	
	public void loadEquipment(Player player) throws AccessException
	{
		try {
			Connection conn = getConnection();
			PreparedStatement stmt = conn.prepareStatement("SELECT item_slot, a.item_id, item_sid, item_dura, item_firstsocket, item_secondsocket, "
					+ "item_plus, item_bless, item_enchant "
					+ "FROM item_possession a "
					+ "JOIN unique_items b ON (a.item_id = b.item_id) "
					+ "WHERE (character_name = ?) AND (item_slot != 0);");
			
			stmt.setString(1, player.getName());
			
			ResultSet rsItems = stmt.executeQuery(); 
			
			while (rsItems.next())
			{
				long item_sid = rsItems.getLong("item_sid"); 
				if (itemPrototypes.get(item_sid)  == null) 
				{ 
					addItemPrototype(item_sid);
				}
				
				player.getEquipmentSlots()[rsItems.getInt("item_slot")] = 
						(
								new EquipmentInstance(rsItems.getLong("item_id"), (EquipmentPrototype) ItemPrototype.get(item_sid))
								.setFirstSocket(EquipmentInstance.Socket.valueOf(rsItems.getInt("item_firstsocket")))
								.setSecondSocket(EquipmentInstance.Socket.valueOf(rsItems.getInt("item_secondsocket")))
								.setDura(rsItems.getInt("item_dura"))
								.setBless(rsItems.getInt("item_bless"))
								.setPlus(rsItems.getInt("item_plus"))
								.setEnchant(rsItems.getInt("item_enchant"))
						);
			}
			
			conn.close();
			stmt.close();
			rsItems.close();
				
		} catch (SQLException e) {
			throw new AccessException(e);
		}
		
	}
	
	public void setInventory(Player player) throws AccessException
	{
		
		try {
			Connection conn = getConnection();
			PreparedStatement stmt = conn.prepareStatement("SELECT a.item_id, item_sid, item_dura, item_firstsocket, item_secondsocket, "
					+ "item_plus, item_bless, item_enchant "
					+ "FROM item_possession a "
					+ "JOIN unique_items b ON (a.item_id = b.item_id) "
					+ "WHERE (character_name = ?) AND (item_slot = 0);");
			stmt.setString(1, player.getName());
			
			ResultSet rsItems = stmt.executeQuery(); 
			
			//EquipmentInstance.Socket(13); 
			
	
			while (rsItems.next())
			{
				long item_sid = rsItems.getLong("item_sid"); 
				if (itemPrototypes.get(item_sid)  == null) 
				{ 
					addItemPrototype(item_sid);
				}
				
				player.getInventory().add
						(
								new EquipmentInstance(rsItems.getLong("item_id"), (EquipmentPrototype) ItemPrototype.get(item_sid))
								.setFirstSocket(EquipmentInstance.Socket.valueOf(rsItems.getInt("item_firstsocket")))
								.setSecondSocket(EquipmentInstance.Socket.valueOf(rsItems.getInt("item_secondsocket")))
								.setDura(rsItems.getInt("item_dura"))
								.setBless(rsItems.getInt("item_bless"))
								.setPlus(rsItems.getInt("item_plus"))
								.setEnchant(rsItems.getInt("item_enchant"))
						);	
			}
		
			
			conn.close();
			stmt.close();
			rsItems.close();
			
		} catch (SQLException e) {
			throw new AccessException(e);
		}
		
		//itemInstances.get(arg0)
		//player.getInventory().add(x)
	}
	
	public void addItemPrototype(long item_sid) throws AccessException
	{
		// to be done 
		
		try {
			Connection conn = getConnection();
			PreparedStatement stmt = conn.prepareStatement("SELECT item_name, item_maxdura, item_worth, item_cpsworth, item_classreq, "
					+ "item_profreq, item_lvlreq, item_sexreq, item_strreq, item_agireq, item_minatk, item_maxatk, item_defence, item_mdef, "
					+ "item_mattack, item_dodge, item_agility "
					+ "FROM items "
					+ "WHERE item_sid = ?"); 
			stmt.setLong(1, item_sid);
			ResultSet rs = stmt.executeQuery();
			
			while (rs.next())
			{
				this.itemPrototypes.put
						(item_sid, 
								new EquipmentPrototype
								(
										item_sid,
										rs.getString("item_name"),
										rs.getInt("item_maxdura"),
										rs.getInt("item_worth"),
										rs.getInt("item_cpsworth"),
										rs.getInt("item_classreq"),
										rs.getInt("item_profreq"),
										rs.getInt("item_lvlreq"),
										rs.getInt("item_sexreq"),
										rs.getInt("item_strreq"),
										rs.getInt("item_agireq"),
										rs.getInt("item_minatk"),
										rs.getInt("item_maxatk"),
										rs.getInt("item_defence"),
										rs.getInt("item_mdef"),
										rs.getInt("item_mattack"),
										rs.getInt("item_dodge"),
										rs.getInt("item_agility")
								)
						);
			}
			
			rs.close();
			stmt.close();
			conn.close();
			
		} catch (SQLException e) {
			throw new AccessException(e);
		}
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
			
			conn.close();
			
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
			PreparedStatement stmt = conn.prepareStatement("SELECT * FROM characters WHERE character_name = ? AND account_username = ?");
			stmt.setString(1, promise.getCharacterName());
			stmt.setString(2, promise.getAccountName());
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
			
			conn.close();
		} catch (SQLException e) {
			throw new AccessException(e);
		}
		return player;	
	}

	@Override
	public boolean createCharacter(Character_Creation_Packet ip) throws AccessException {
		
		try {
			Connection conn = getConnection();
			
			PreparedStatement stmt = conn.prepareStatement("SELECT * FROM characters WHERE character_name = ?"); 
			stmt.setString(1, ip.getCharacterName());
			ResultSet rs = stmt.executeQuery();	
			
			if(!rs.next()) /* If query resulted in no results, character name has not been taking yet.  */
			{	
				rs.close();
				stmt = conn.prepareStatement("INSERT INTO characters "
						+ "(account_username, character_name, character_level, character_experience, character_strength, character_agility, "
						+ "character_vitality, character_spirit, character_attributepoints, character_profession, character_mesh, character_gold, character_cps, "
						+ "character_map, character_x, character_y, character_hair, character_reborn, character_curhp, character_curmp)"
						+ " VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)");
				
				stmt.setString(1, ip.getAccountName());
				stmt.setString(2, ip.getCharacterName());
				stmt.setInt(3, 1); // level 1
				stmt.setInt(4, 0);
				stmt.setInt(5, 1); // str
				stmt.setInt(6, 1); // agi
				stmt.setInt(7, 1); // vit
				stmt.setInt(8, 1); // spi
				stmt.setInt(9, 0); // rem attributes
				stmt.setInt(10, ip.getProffession());
				stmt.setInt(11, (38000 + ip.getBody())); // mesh (standard avatar = 38) 
				stmt.setInt(12, 0); // gold
				stmt.setInt(13, 0); //cps
				stmt.setInt(14, 1002); // tc
				stmt.setInt(15, 439); // x
				stmt.setInt(16, 383); // y
				stmt.setInt(17, 315); // hair
				stmt.setInt(18, 0); // rb count
				stmt.setInt(19, 100);
				stmt.setInt(20, 0);
				
				stmt.execute();
				conn.close(); 
				
				return true;
			} else {
				return false;
			}
			
					
		} catch (SQLException e) {
			throw new AccessException(e);
		}
	}

}
