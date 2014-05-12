package net.co.java.model;

import java.io.File;
import java.io.FileNotFoundException;
import java.util.InputMismatchException;
import java.util.NoSuchElementException;
import java.util.Scanner;

import net.co.java.entity.Location;
import net.co.java.entity.Monster;
import net.co.java.entity.Player;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemInstance.EquipmentInstance;
import net.co.java.item.ItemPrototype.EquipmentPrototype;
import net.co.java.item.ItemPrototype;
import net.co.java.packets.Character_Creation_Packet;
import net.co.java.server.Map;
import net.co.java.skill.Skill;
import net.co.java.skill.WeaponProficiency;
import net.co.java.skill.WeaponType;

/**
 * The Mock model is a model using mock data and is mainly for
 * developing and testing purposes.
 * 
 * @author Jan-Willem Gmelig Meyling
 *
 */
public class Mock extends AbstractModel {
	
	private Player player;
	
	public Mock() throws FileNotFoundException {
		createSomeStuff();
	}
	
	private void createSomeStuff() throws FileNotFoundException{
		System.out.println("Creating the magical world of Conquer Online");
		// We spawn a BullMessenger in Twin City for testing purposes here
		new Monster(new Location(Map.CentralPlain, 370, 344, 4), 400001, "BullMessenger",  112, 117, 55000).spawn();
		new Monster(new Location(Map.CentralPlain, 373, 344, 4), 524000, "BullMessenger",  112, 117, 55000).spawn();
		new Monster(new Location(Map.CentralPlain, 376, 344, 4), 475000, "BullMessenger",  112, 117, 55000).spawn();
		new Monster(new Location(Map.CentralPlain, 379, 344, 4), 499999, "BullMessenger",  112, 117, 55000).spawn();
		// Load the item data
		readItemPrototypes(new File("ini/COItems.txt"));
	}
	
	@Override
	public boolean isAuthorised(String server, String username, String password) {
		return true;
	}

	@Override
	public AuthorizationPromise createAuthorizationPromise(String accountName)
			throws AccessException {
		Long identity = this.createPlayerIdentity();
		String characterName = null;
		if (player != null ) {
			characterName = player.getName();
		} else  if(accountName.equalsIgnoreCase("jackpotsvr")) {
			player = new Player(identity, accountName, new Location(Map.CentralPlain, 382, 341), 0);
			player.setLevel(1);
			player.setStrength(1);
			player.setDexterity(1);
			player.setSpirit(1);
			player.setVitality(1);
			player.setProfession(15);
			player.setMesh((38000 + 1003));
			player.setGold(0);
			player.setCps(0);
			player.setRebornCount(0);
			player.setHP(player.getMaxHP());
			player.setExperience(0);
			characterName = accountName;
		}
		AuthorizationPromise promise = new AuthorizationPromise(identity, accountName, characterName);
		this.authPromises.put(identity, promise);
		return promise;
	}

	@Override
	public boolean createCharacter(Character_Creation_Packet ip)
			throws AccessException {
		player = new Player(ip.getIdentity(), ip.getCharacterName(), new Location(Map.CentralPlain, 382, 341), 0);
		player.setLevel(1);
		player.setStrength(1);
		player.setDexterity(1);
		player.setSpirit(1);
		player.setVitality(1);
		player.setProfession(ip.getProffession());
		player.setMesh((38000 + ip.getBody()));
		player.setGold(0);
		player.setCps(0);
		player.setRebornCount(0);
		player.setHP(player.getMaxHP());
		player.setExperience(0);
		return true;
	}
	
	@Override
	protected Player fetchPlayer(AuthorizationPromise promise)
			throws AccessException {
		Player player = this.player;
		this.player = null;
		player.setGold(1111);
		player.setCps(215);
		player.setStrength(180);
		player.setDexterity(50);
		player.setVitality(50);
		player.setSpirit(50);
		player.setHP(5000);
		player.setLevel(130);
		player.setProfession(15);
		return player;
	}

	@Override
	protected void fetchInventory(Player hero) throws AccessException {
		ItemInstance item = new EquipmentInstance(2342239l, this.getEquipmentPrototype(480029l))
			.setFirstSocket(EquipmentInstance.Socket.SuperFury)
			.setSecondSocket(EquipmentInstance.Socket.SuperRainbowGem)
			.setDura(1500).setBless(3).setPlus(7).setEnchant(172);
		itemInstances.put(2342239l, item);
		hero.inventory.addItem(item);
		ItemInstance item2 = new EquipmentInstance(2342240l, this.getEquipmentPrototype(480029l))
			.setFirstSocket(EquipmentInstance.Socket.SuperDragon)
			.setSecondSocket(EquipmentInstance.Socket.SuperPhoenix)
			.setDura(1500).setBless(3).setPlus(7).setEnchant(172);
		itemInstances.put(2342240l, item2);
		hero.inventory.addItem(item2);
	}

	@Override
	protected void fetchEquipment(Player hero) throws AccessException {
		EquipmentInstance LeftBlade = new EquipmentInstance(1l, this.getEquipmentPrototype(410339l))
			.setFirstSocket(EquipmentInstance.Socket.SuperFury)
			.setSecondSocket(EquipmentInstance.Socket.SuperRainbowGem)
			.setDura(5000).setBless(3).setPlus(9).setEnchant(169);
		EquipmentInstance RightBlade = new EquipmentInstance(2l, this.getEquipmentPrototype(410339l))
			.setFirstSocket(EquipmentInstance.Socket.SuperDragon)
			.setSecondSocket(EquipmentInstance.Socket.SuperPhoenix)
			.setDura(5000).setBless(2).setPlus(9).setEnchant(172);
		EquipmentInstance Armor = new EquipmentInstance(3l, this.getEquipmentPrototype(135999l))
			.setFirstSocket(EquipmentInstance.Socket.SuperDragon)
			.setSecondSocket(EquipmentInstance.Socket.SuperPhoenix)
			.setDura(5000).setBless(4).setPlus(9).setEnchant(154);
		
		itemInstances.put(LeftBlade.uniqueIdentifier, LeftBlade);
		itemInstances.put(RightBlade.uniqueIdentifier, RightBlade);
		itemInstances.put(Armor.uniqueIdentifier, Armor);
		
		hero.inventory.equip(Player.Inventory.LEFT_HAND, LeftBlade);
		hero.inventory.equip(Player.Inventory.RIGHT_HAND, RightBlade);
		hero.inventory.equip(Player.Inventory.ARMOR, Armor);
	}

	@Override
	protected void fetchSkill(Player hero) throws AccessException {
		hero.setSkill(Skill.FAST_BLADE, 4, 0);
		hero.setSkill(Skill.SCENT_SWORD, 4, 0);
		hero.setSkill(Skill.RAGE, 8, 0);
	}

	@Override
	protected void fetchProficiency(Player hero) throws AccessException {
		hero.setProficiency(WeaponType.BLADE, 20, 65000);
		hero.setProficiency(WeaponType.SWORD, 12, 65000);
	}

	@Override
	protected ItemPrototype fetchItemPrototype(long id) {
		// TODO Auto-generated method stub
		return null;
	}

	@Override
	protected ItemInstance fetchItemInstance(long id) throws AccessException {
		// TODO Auto-generated method stub
		return null;
	}

	private void readItemPrototypes(File file) throws FileNotFoundException {
		Scanner sc = new Scanner(file);		
		if(sc.hasNext()&&sc.next().equalsIgnoreCase("[Items]")) {
			while(sc.hasNext()) {
				try {
					constructItemPrototype(sc.next());
				} catch (Exception e) {	}
			}
		}
		
		sc.close();
		System.out.println("Loaded " + itemPrototypes.size() + " item prototypes");
	}

	/**
	 * 
	 * @param string element
	 * @return a new {@code ItemPrototype} instance
	 * @throws InputMismatchException
     * @throws NoSuchElementException if input is exhausted
	 */
	private ItemPrototype constructItemPrototype(String string) {
		String[] split = string.split("=");
		Long id = Long.valueOf(split[0]);
		boolean isEquipment = ItemPrototype.isEquipment(id);
		Scanner sc = new Scanner(split[1]);
		sc.useDelimiter("-[^\\w]*");
		
		String name = sc.next();
		int classReq = sc.nextInt();
		int profReq = sc.nextInt();
		int lvlReq = sc.nextInt();
		int sexReq = sc.nextInt();
		int strReq = sc.nextInt();
		int agiReq = sc.nextInt();
		int worth = sc.nextInt();
		int minAtk = sc.nextInt();
		int maxAtk = sc.nextInt();
		int defence = sc.nextInt();
		int mDef = sc.nextInt();
		int mAttack = sc.nextInt();
		int dodge = sc.nextInt();
		int agility = sc.nextInt();
		int CPWorth = sc.nextInt();
		
		int maxDura = 0;
		if(sc.hasNextInt()) {
			maxDura = sc.nextInt() * 100;
		}
	
		sc.close();
		
		ItemPrototype item = isEquipment ? new EquipmentPrototype(id, name, maxDura, worth,
				CPWorth, classReq, profReq, lvlReq, sexReq, strReq, agiReq,
				minAtk, maxAtk, defence, mDef, mAttack, dodge, agility)
				: new ItemPrototype(id, name, maxDura, worth, CPWorth);
		itemPrototypes.put(id, item);
		return item;
	}

	@Override
	public void setProficiency(Player hero, WeaponProficiency wp)
			throws AccessException {
		// TODO Auto-generated method stub
		
	}

	@Override
	protected void fetchNPCs() throws AccessException {
		// TODO Auto-generated method stub
		
	}

}
