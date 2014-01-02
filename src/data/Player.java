package data;

import java.util.HashMap;

import data.Entity.GuildRank;
import packets.OutgoingPacket;
import packets.PacketType;

public class Player extends Entity {
	

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
	
	private HashMap<Integer, Item> inventory = new HashMap<Integer, Item>();
	private HashMap<EquipmentSlot, Equipment> equipment = new HashMap<EquipmentSlot, Equipment>();
	
	public Player(long identity, String name, Location location, int HP) {
		super(identity, 223, 315, name, location, HP);
		// TODO Auto-generated constructor stub
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
		return "Firetao250"; // TODO
	}

	public void setSpouse(Player spouse) {
		this.spouse = spouse;
	}

	public HashMap<Integer, Item> getInventory() {
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
	}

	@Override
	public int getMaxHP() {
		// TODO Auto-generated method stub
		return 0;
	}	

	@Override
	public OutgoingPacket spawn() {
		return new OutgoingPacket(PacketType.ENTITY_SPAWN_PACKET, new byte[82+name.length()]) {{
			this.putUnsignedInteger(identity);
			this.putUnsignedInteger(mesh);
			this.setOffset(20); // TODO ulong status flags?
			this.putUnsignedShort(0); // Guild ID
			this.setOffset(23);
			this.putUnsignedByte((short) guildRank.rank); // Guild rank
			this.putUnsignedInteger(0); // garment 24
			this.putUnsignedInteger(0); // helm 28
			this.putUnsignedInteger(0); // arm 32
			this.putUnsignedInteger(0); // rw 36
			this.putUnsignedInteger(0); // lw 40
			this.setOffset(48);
			this.putUnsignedShort(HP); // health 48
			this.putUnsignedShort(0); // mob lvl 50
			this.putUnsignedShort(location.getxCord()); // 52
			this.putUnsignedShort(location.getyCord()); // 54
			this.putUnsignedShort(hairstyle); //56
			this.putUnsignedByte(4); // direction 58
			this.putUnsignedByte(0x01); // action 59
			this.putUnsignedByte(1); // reborn //60
			this.setOffset(62);
			this.putUnsignedByte(130); // level
			this.setOffset(80);
			this.putUnsignedByte(1);
			this.putUnsignedByte(name.length());
			this.putString(name);
		}};
	}

}
