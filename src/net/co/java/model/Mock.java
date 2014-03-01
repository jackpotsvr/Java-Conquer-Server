package net.co.java.model;

import java.io.File;
import java.io.FileNotFoundException;
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
 * The Mock model is a model using mock data and is mainly for
 * developing and testing purposes.
 * @author Jan-Willem Gmelig Meyling
 *
 */
public class Mock extends AbstractModel {
	
	public Mock() throws FileNotFoundException {
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
	
	@Override
	public boolean isAuthorised(String server, String username, String password) {
		return true;
	}

	@Override
	public ItemInstance[] getInventory(Player player) {
		return new ItemInstance[] { EquipmentInstance.get(2342239l) };
	}

	@Override
	public HashMap<EquipmentSlot, EquipmentInstance> getEquipments(Player player) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public ItemPrototype getItemPrototype(long staticID) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	public ItemInstance getItemInstance(long id) {
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
		String characterName = "Jackpotsvr";
		AuthorizationPromise promise = new AuthorizationPromise(identity, accountName, characterName);
		this.authPromises.put(identity, promise);
		return promise;
	}

	@Override
	public Player loadPlayer(AuthorizationPromise promise) throws AccessException {
		Player player = new Player(promise.getIdentity(), promise.getCharacterName(), null, 500);
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
		this.players.put(promise.getIdentity(), player);
		return player;
	}

	@Override
	public boolean createCharacter(Character_Creation_Packet ip)
			throws AccessException {
		// TODO Auto-generated method stub
		return false;
	}

	@Override
	public void setInventory(Player player) throws AccessException {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void addItemPrototype(long item_sid) throws AccessException {
		// TODO Auto-generated method stub
		
	}

	@Override
	public void loadEquipment(Player player) throws AccessException {
		// TODO Auto-generated method stub
		
	}

}
