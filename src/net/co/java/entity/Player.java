package net.co.java.entity;

import java.util.HashMap;

import net.co.java.guild.Guild;
import net.co.java.guild.GuildRank;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemInstance.EquipmentInstance;
import net.co.java.item.ItemInstance.Mode;
import net.co.java.packets.ItemUsage;
import net.co.java.packets.NPC_Dialog_Packet;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.packets.UpdatePacket;
import net.co.java.server.AbstractClient;
import net.co.java.skill.Skill;
import net.co.java.skill.SkillProficiency;
import net.co.java.skill.WeaponProficiency;
import net.co.java.skill.WeaponType;

public class Player extends Entity {
	
	private static final long serialVersionUID = 8357201473468302184L;

	private AbstractClient client;
	private NPC_Dialog_Packet activeDialog;
	
	private int gold = 0, cps = 0,
			experience = 0, pkPoints = 0, profession = 15, rebornCount = 0, stamina = 100, action = 0,
			strength = 1, dexterity = 1, vitality = 1, spirit = 1;
			
	protected Guild guild = null;	                
	protected GuildRank guildRank = GuildRank.None; 
	private String spouse;
	
	public final Inventory inventory = new Inventory();	
	private final HashMap<WeaponType, WeaponProficiency> proficiencies = new HashMap<WeaponType, WeaponProficiency>();
	private final HashMap<Skill, SkillProficiency> skills = new HashMap<Skill, SkillProficiency>();


	public Player(Long identity, String name, Location location, int HP) {
		super(identity, 223, 315, name, location, HP);
	}
	
	public void setClient(AbstractClient client) {
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
	
	public int getSkillLevel(Skill skill) {
		return 2;
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

	public int getStamina() {
		return stamina;
	}

	public void setStamina(int stamina) {
		this.stamina = stamina;
	}
	
	public void setAction(int action) {
		this.action = action;
	}
	
	public int getAction() {
		return action;
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
	
	@Override
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
	public AbstractClient getClient() {
		return client;
	}
	
	public void setProficiency(WeaponType t, int level, long exp) {
		if ( level > 0 || exp > 0 )
			proficiencies.put(t, new WeaponProficiency(this, t, level, exp));
	}
	
	public HashMap<WeaponType, WeaponProficiency> getProficiencies() {
		return proficiencies;
	}

	public void setSkill(Skill s, int level, long exp) {
		if ( level > 0 || exp > 0 )
			skills.put(s, new SkillProficiency(this, s, level, exp));
	}
	
	public SkillProficiency getSkillProficiency(Skill skill) {
		return skills.get(skill);
	}
	
	public WeaponProficiency getWeaponProficiency(WeaponType t) {
		return proficiencies.get(t);
	}

	public NPC_Dialog_Packet getActiveDialog() {
		return activeDialog;
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
				if(sent) item.new ItemInformationPacket(Mode.DEFAULT, INVENTORY).send(client);
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
					for ( int j = i + 1; j < position; j++ ) {
						items[j-1] = items[j];
					}
					position--;
					// Send a packet to the client to remove the item from the inventory
					if(sent) new ItemUsage(item.uniqueIdentifier, INVENTORY, ItemUsage.Mode.RemoveInventory).build().send(client);
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
				if(sent) equipment.new ItemInformationPacket(Mode.DEFAULT, slot).send(client);
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
					if(sent) new ItemUsage(oldItem.uniqueIdentifier, slot, ItemUsage.Mode.RemoveEquipment).build().send(client);
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
		
		/**
		 * @return the equipments for this Player
		 */
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
		
		/**
		 * @param slot
		 * @return the SID for the equipment in a given slot, or 0.
		 */
		public long getEquipmentSID(int slot) {
			if(equipments[slot] != null)
				return equipments[slot].itemPrototype.identifier;
			return 0;
		}
		
		private boolean sent = false;

		/**
		 * Send the equipments and inventory contents to the client
		 */
		public void send() {
			if(!sent){
				for ( int i = 0; i < position; i++ )
					items[i].new ItemInformationPacket(Mode.DEFAULT, INVENTORY).send(client);
				for ( int i = 1; i < equipments.length; i++ )
					if ( equipments[i] != null )
						equipments[i].new ItemInformationPacket(Mode.DEFAULT, i).send(client);
				sent = true;
			}
		}
		
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

	public void sendProficiencies() {
		for ( WeaponProficiency wp : proficiencies.values())
			wp.sendSkill();
	}
	
	public void sendSkills(){
		for ( SkillProficiency skill : skills.values() )
			skill.sendSkill();
	}
	
	public void sendStamina() {
		new UpdatePacket(client.getPlayer()).setAttribute(UpdatePacket.Mode.Stamina, (long) getStamina()).build().send(client);
	}

	@Override
	public void notify(PacketWriter writer) {
		writer.send(client);
	}
	
}
