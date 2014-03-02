package net.co.java.entity;

import java.util.HashMap;
import java.util.Map.Entry;

import net.co.java.guild.Guild;
import net.co.java.guild.GuildRank;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemInstance.EquipmentInstance;
import net.co.java.item.ItemInstance.Mode;
import net.co.java.packets.GeneralData;
import net.co.java.packets.ItemUsage;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.packets.GeneralData.SubType;
import net.co.java.server.Server.GameServer.Client;
import net.co.java.server.Server.Map.Location;

public class Player extends Entity {
	

	private Client client;
	private int gold = 0;
	private int cps = 0;
	private int experience = 0;
	private int strength = 1;
	private int dexterity = 1;
	private int vitality = 1;
	private int spirit = 1;
	
	protected Guild guild = null;	                
	protected GuildRank guildRank = GuildRank.None; 

	private int pkPoints = 0;
	private int profession = 15;
	private int rebornCount = 0;
	private String spouse;
	
	public final Inventory inventory = new Inventory();
	
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

	/**
	 * @return the amount of remaining attribute points.
	 * Attribute points are calculated based on your reborn path
	 * and current level. 
	 */
	public int getAttributePoints() {
		// Reborn characters start at level 15 and level attribute
		// point assignment starts at level 15 as well
		if ( level < 15 )
			return 0;
		// 52 Attribute points by default and 3 attribute points per level after level 15
		int ap = 52 + 3 * (level - 15);
		if(rebornCount > 1) {
			// After rebirth, players will receive 30 attribute points
			ap += 30;
			// In addition, more bonus attribute points (55 at most) will
			// be awarded if you were reborn at higher level.
			int fstRbLvl = 130;
			boolean fstRbWater = false;
			if (fstRbWater) {
				// Water taoists can reborn at level 110 and therefore
				// have different stats
				ap += ((fstRbLvl - 110) / 2);
			} else {
				ap += fstRbLvl - 120;
			}
			// Second reborn additional attribute points
			if(rebornCount > 2) {
				int secRbLvl = 130;
				boolean secRbWater = false;
				if (secRbWater) {
					// Water taoists can reborn at level 110 and therefore
					// have different stats
					ap += ((secRbLvl - 110) / 2);
				} else {
					ap += secRbLvl - 120;
				}
			}
		}
		// Remove the assigned points
		ap -= strength + dexterity + vitality + spirit;
		return ap;
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

	public String getSpouse() {
		return spouse == null ? "None" : spouse;
	}

	public void setSpouse(String spouse) {
		this.spouse = spouse;
	}

	@Override
	public int getMaxHP() {
		int hp = vitality * 24;
		hp += strength * 3;
		hp += dexterity * 3;
		hp += spirit * 3;
		
		if(profession >= 10 && profession <= 15) {
			if ( level >= 110 ) {
				hp *= 1.15;
			} else if ( level >= 100 ) {
				hp *= 1.12;
			} else if ( level >= 70 ) {
				hp *= 1.1;
			} else if ( level >= 40 ) {
				hp *= 1.08;
			} else if ( level >= 15 ) {
				hp *= 1.05;
			}
		}
		
		for ( EquipmentInstance eq : inventory.getEquipments()) {
			hp += eq.enchant;
		}
		
		return hp;
	}
	
	public int getMaxMana() {
		int mana = spirit * 5;
		if(profession >= 100) {
			if ( level >= 110 ) {
				mana *= 6;
			} else if ( level >= 100 ) {
				mana *= 5;
			} else if ( level >= 70 ) {
				mana *= 4;
			} else if ( level >= 40 ) {
				mana *= 3;
			}
		}
		return mana;
	}
	
	/**
	 * @return the {@code Client} instance for this {@code User},
	 * or null if the player is not online
	 */
	public Client getClient() {
		return client;
	}
	
	public PacketWriter characterInformation() {
		String spouseName = getSpouse();
		int size = 71 + this.name.length() + spouseName.length();
		
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
		.putUnsignedShort(this.getAttributePoints())
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
		.putUnsignedByte(spouseName.length())
		.putString(spouseName);
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

	public class Inventory {
		
		private static final int CAPACITY = 40;
		public static final int INVENTORY = 0;
		public static final int HELM = 1;
		public static final int NECKLACE = 2;
		public static final int ARMOR = 3;
		public static final int RIGHT_HAND = 4;
		public static final int LEFT_HAND = 5;
		public static final int RING = 6;
		public static final int TALISMAN = 7;
		public static final int BOOTS = 8;
		public static final int GARMENT = 9;
		// public static final int ATTACK_TALISMAN = 10;
		// public static final int DEFENSE_TALISMAN = 11;
		// public static final int STEED = 12;	
		private int position = 0;
		private final ItemInstance[] items = new ItemInstance[CAPACITY];
		private final EquipmentInstance[] equipments = new EquipmentInstance[10];
		
		/**
		 * Add an item to the Players inventory and update the client through
		 * an ItemInformation Packet
		 * @param item to be added to the inventory
		 * @return true if the item is successfully added to the inventory,
		 * false if the inventory was full
		 */
		public boolean addItem(ItemInstance item) {
			if(position < CAPACITY) {
				items[position++] = item;
				// Send a packet to the client to add the item to the inventory
				item.new ItemInformationPacket(Mode.DEFAULT, INVENTORY).send(client);
				return true;
			}
			return false;
		}
		
		/**
		 * Remove an item from the Players inventory and update the client through
		 * an ItemUsage packet
		 * @param item to be removed from the inventory
		 * @return true if the item is successfully removed from the inventory,
		 * false if the inventory did not contain the item
		 */
		public boolean removeItem(ItemInstance item) {
			for ( int i = 0; i < position; i++ ) {
				if (items[i] == item ) {
					for ( int j = i + 1; i < position; i++ ) {
						items[j-1] = items[j];
					}
					position--;
					// Send a packet to the client to remove the item from the inventory
					new ItemUsage(item.uniqueIdentifier, INVENTORY, ItemUsage.Mode.RemoveInventory).build().send(client);
					return true;
				}
			}
			return false;
		}
		
		/**
		 * Equip an item. If the inventory contains the item, remove it from the
		 * inventory first. The item which used the equipment slot before this
		 * equip interaction, is transfered to the inventory. 
		 * Send a ItemInformation packet such that the newly equipped item
		 * appears in the client
		 * @param slot
		 * @param equipment
		 * @return true if successfully equipped the item, or false if not.
		 */
		public boolean equip(int slot, EquipmentInstance equipment) {
			if ( slot > 0 && slot < 11 ) {
				if ( contains(equipment) ) {
					// Remove the item from the inventory
					removeItem(equipment);
				}
				unequip(slot);
				equipments[slot] = equipment;
				// Send a packet to the client to equip the item at the equipment slot
				equipment.new ItemInformationPacket(Mode.DEFAULT, slot).send(client);
				return true;
			} else {
				// Item can't be equipped
				return false;
			}
		}
	
		/**
		 * Unequip an item, and transfer the item to the inventory.
		 * @param slot
		 * @return true if the inventory is not full
		 */
		public boolean unequip(int slot) {
			// Slot should be an equipment slot and inventory should not be full
			if ( slot > 0 && slot < 11 && !isFull()) {
				EquipmentInstance oldItem = equipments[slot];
				if ( oldItem != null ) {
					addItem(oldItem);
					equipments[slot] = null;
					// Send a packet to the client to remove the item from the equipment slot
					new ItemUsage(oldItem.uniqueIdentifier, slot, ItemUsage.Mode.RemoveEquipment).build().send(client);
				}
				return true;
			}
			return false;
		}
	
		/**
		 * @param item
		 * @return true if the inventory contains the given item
		 */
		public boolean contains(ItemInstance item) {
			for ( int i = 0; i < position; i++ )
				if ( items[i] == item )
					return true;
			return false;
		}
		
		/**
		 * @return true if the inventory is empty
		 */
		public boolean isEmpty() {
			return position == 0;
		}
		
		/**
		 * @return true if the inventory is full
		 */
		public boolean isFull() {
			return position == CAPACITY;
		}
		
		/**
		 * @return an array of items in the inventory
		 */
		public ItemInstance[] getItems() {
			ItemInstance[] result = new ItemInstance[position];
			for ( int i = 0; i < position; i++ )
				result[i] = items[i];
			return result;
		}
		
		public EquipmentInstance[] getEquipments() {
			int amount = 0;
			for ( int i = 0; i < equipments.length; i++ )
				if ( equipments[i] != null )
					amount++;
			EquipmentInstance[] result = new EquipmentInstance[amount];
			for ( int i = 0, k = 0; i < equipments.length; i++ )
				if ( equipments[i] != null )
					result[k++] = equipments[i];
			return result;
		}
		
	}
	
	private final HashMap<Proficiency, Integer> proficiencies = new HashMap<Proficiency, Integer>();
	
	private static final int[] PROF_LEVEL_EXP = {
        1200, 68000, 250000, 640000, 1600000,
        4000000, 10000000, 22000000, 40000000, 90000000, 95000000, 142500000, 213750000,
        320625000, 480937500, 721406250, 1082109375, 1623164063, 2100000000, 2100000000
    };
	
	public void setProficiencyExp(Proficiency p, int value) {
		if ( value > 0 )
			proficiencies.put(p, value);
	}
	
	public int getProficiencyExp(Proficiency p) {
		return proficiencies.get(p); 
	}
	
	public int getProficiencyLvl(int exp) {
		for ( int i = PROF_LEVEL_EXP.length - 1; i >= 0;  i-- ) {
			if ( exp >= PROF_LEVEL_EXP[i] ) {
				return i + 1;
			}
		}
		return 0;
	}
	
	public void sendProficiencies() {
		for ( Entry<Proficiency, Integer> entry : proficiencies.entrySet() ) {
			int exp = entry.getValue();
			int lvl = getProficiencyLvl(exp);
			new PacketWriter(PacketType.PROFICIENCY, 16)
			.putUnsignedInteger(entry.getKey().prof)
			.putUnsignedInteger(lvl)
			.putUnsignedInteger(exp)
			.send(this);
		}
	}
	
}
