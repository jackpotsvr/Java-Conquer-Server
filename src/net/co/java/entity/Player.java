package net.co.java.entity;

import java.util.HashMap;

import net.co.java.guild.Guild;
import net.co.java.guild.GuildRank;
import net.co.java.item.EquipmentSlot;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemInstance.EquipmentInstance;
import net.co.java.packets.GeneralData;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.packets.GeneralData.SubType;
import net.co.java.server.Server.GameServer.Client;
import net.co.java.server.Server.Map.Location;

public class Player extends Entity {
	

	private Client client;
	private int gold = 1000;
	private int cps = 215;
	private int experience = 34195965;
	private int strength = 51;
	private int dexterity = 52;
	private int vitality = 53;
	private int spirit = 54;
	private int attributePoints = 500;
	
	protected Guild guild = null;	                
	protected GuildRank guildRank = GuildRank.None; 

	private int pkPoints = 0;
	private int profession = 15;
	private int rebornCount = 0;
	private Player spouse;
	
	private static int INVENTORY_SIZE = 28;
	private ItemInstance[] inventory = new ItemInstance[INVENTORY_SIZE];
	private HashMap<EquipmentSlot, EquipmentInstance> equipment = new HashMap<EquipmentSlot, EquipmentInstance>(10);
	
	public Player(Long identity, String name, Location location, int HP) {
		super(identity, 223, 315, name, location, HP);
	}
	
	public void setClient(Client client) {
		this.client = client;
	}

	public int getGold() {
		return gold;
	}

	public void setGold(int gold) {
		this.gold = gold;
	}

	public int getCps() {
		return cps;
	}

	public void setCps(int cps) {
		this.cps = cps;
	}

	public Guild getGuild() {
		return guild;
	}

	public void setGuild(Guild guild) {
		this.guild = guild;
	}

	public int getGuildRank() {
		return guildRank.getRank();
	}

	public void setGuildRank(GuildRank guildRank) {
		this.guildRank = guildRank;
	}
	
	public int getExperience() {
		return experience;
	}

	public void setExperience(int experience) {
		this.experience = experience;
	}

	public int getStrength() {
		return strength;
	}

	public void setStrength(int strength) {
		this.strength = strength;
	}

	public int getDexterity() {
		return dexterity;
	}

	public void setDexterity(int dexterity) {
		this.dexterity = dexterity;
	}

	public int getVitality() {
		return vitality;
	}

	public void setVitality(int vitality) {
		this.vitality = vitality;
	}

	public int getSpirit() {
		return spirit;
	}

	public void setSpirit(int spirit) {
		this.spirit = spirit;
	}

	public int getAttributePoints() {
		return attributePoints;
	}

	public void setAttributePoints(int attributePoints) {
		this.attributePoints = attributePoints;
	}

	public int getPkPoints() {
		return pkPoints;
	}

	public void setPkPoints(int pkPoints) {
		this.pkPoints = pkPoints;
	}

	public int getProfession() {
		return profession;
	}

	public void setProfession(int profession) {
		this.profession = profession;
	}

	public int getRebornCount() {
		return rebornCount;
	}

	public void setRebornCount(int rebornCount) {
		this.rebornCount = rebornCount;
	}

	public Player getSpouse() {
		return spouse;
	}
	
	public String getSpouseName() {
		if ( spouse != null ) {
			return spouse.getName();
		}
		return "None";
	}

	public void setSpouse(Player spouse) {
		this.spouse = spouse;
	}

	/*	public HashMap<Integer, Item> getInventory() {
	return inventory;
}

public void setInventory(HashMap<Integer, Item> inventory) {
	this.inventory = inventory;
}

public HashMap<EquipmentSlot, Equipment> getEquipment() {
	return equipment;
}

public void setEquipment(HashMap<EquipmentSlot, Equipment> equipment) {
	this.equipment = equipment;
}*/

	@Override
	public int getMaxHP() {
		// TODO Auto-generated method stub
		return 0;
	}
	
	/**
	 * @return the {@code Client} instance for this {@code User},
	 * or null if the player is not online
	 */
	public Client getClient() {
		return client;
	}
	
	public PacketWriter characterInformation() {
		int size = 71 + this.name.length() + ((this.spouse == null) ? 4 : this.spouse.name.length());
		return new PacketWriter(PacketType.CHAR_INFO_PACKET, size)
		.putUnsignedInteger(this.identity)
		.putUnsignedInteger(this.mesh)
		.putUnsignedShort(this.hairstyle)
		.putUnsignedInteger(this.gold)
		.putUnsignedInteger(this.cps)
		.putUnsignedInteger(this.experience)
		.setOffset(46)
		.putUnsignedShort(this.strength)
		.putUnsignedShort(this.dexterity)
		.putUnsignedShort(this.vitality)
		.putUnsignedShort(this.spirit)
		.putUnsignedShort(this.attributePoints)
		.putUnsignedShort(this.HP)
		.putUnsignedShort(this.mana)
		.putUnsignedShort(this.pkPoints)
		.putUnsignedByte(this.level)
		.putUnsignedByte(this.profession)
		.setOffset(65)
		.putUnsignedByte(this.rebornCount)
		.putBoolean(true) // Display names
		.putUnsignedByte(2) // String count
		.putUnsignedByte(this.name.length())
		.putString(this.name)
		.putUnsignedByte(this.getSpouseName().length())
		.putString(this.getSpouseName());
	}
	
	public GeneralData retrieveLocation() {
		return new GeneralData(SubType.LOCATION, identity, new int[] {
			location.getMap().getMapID(),
			location.getxCord(),
			location.getyCord()
		});
	}
	
	@Override
	public PacketWriter spawn() {
		return new PacketWriter(PacketType.ENTITY_SPAWN_PACKET, 82 + name.length())
		.putUnsignedInteger(identity)
		.putUnsignedInteger(mesh)
		.setOffset(20) // TODO ulong status flags?
		.putUnsignedShort(0) // Guild ID
		.setOffset(23)
		.putUnsignedByte((short) guildRank.getRank()) // Guild rank
		.putUnsignedInteger(0) // garment 24
		.putUnsignedInteger(0) // helm 28
		.putUnsignedInteger(0) // arm 32
		.putUnsignedInteger(0) // rw 36
		.putUnsignedInteger(0) // lw 40
		.setOffset(48)
		.putUnsignedShort(HP) // health 48
		.putUnsignedShort(0) // mob lvl 50
		.putUnsignedShort(location.getxCord()) // 52
		.putUnsignedShort(location.getyCord()) // 54
		.putUnsignedShort(hairstyle) //56
		.putUnsignedByte(4) // direction 58
		.putUnsignedByte(0x0) // action 59
		.putUnsignedByte(1) // reborn //60
		.setOffset(62)
		.putUnsignedByte(130) // level
		.setOffset(80)
		.putUnsignedByte(1)
		.putUnsignedByte(name.length())
		.putString(name);
	}

}
