package net.co.java.item;

import java.io.File;
import java.io.FileNotFoundException;
import java.util.HashMap;
import java.util.InputMismatchException;
import java.util.NoSuchElementException;
import java.util.Scanner;

/**
 * Item prototype is used for the static attribute
 * of unique item instances
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public class ItemPrototype {
	
	public final Long identifier;
	public final String name;
	public final int maxDura;
	public final int worth;
	public final int CPWorth;
	
	ItemPrototype(Long identifier, String name, int maxDura, int worth, int CPWorth) {
		this.identifier = identifier;
		this.name = name;
		this.maxDura = maxDura;
		this.worth = worth;	
		this.CPWorth = CPWorth;
		ITEM_PROTOTYPES.put(identifier, this);
	}
	
	
	
	/*
	 * 111303-118999 Hats
	 * 120003-121249 Bags/necks
	 * 130203-139999 Armor/robe
	 * 150000-152259 Rings
	 * 160013-160249 Boots
	 * 
	 * 410003-410901 Weapons/Blades
	 * 420003-420339 Weapons/Swords
	 * 421003-421339 Weapons/Backswords
	 * 422000-422040 Weapons/Attributes
	 * 430003-430339 Weapons/Hooks
	 * 440003-440339 Weapons/Whips
	 * 450003-450339 Weapons/Axe
	 * 460003-460339 Weapons/Hammer
	 * 480003-480339 Weapons/Clubs
	 * 481003-481339 Weapons/Scepter
	 * 490003-490339 Weapons/Daggers
	 * 
	 * 500003-500329 Weapons/Bows
	 * 510003-510339 Weapons/Glaives
	 * 530003-530339 Weapons/Poleaxe
	 * 540003-540339 Weapons/Longhammer
	 * 560003-560339 Weapons/Spears
	 * 561003-561339 Weapons/Wands
	 * 562000-562001 Weapons/Pickaxe&Hoe
	 * 580003-580339 Weapons/Halbert
	 * 
	 * 900300-900999 Shields
	 * 
	 * 700001-700003 Gems/Phoenix
	 * 700011-700013 Gems/Dragon
	 * 700021-700023 Gems/Fury
	 * 700031-700033 Gems/Rainbow
	 * 700041-700043 Gems/Kylin
	 * 700051-700053 Gems/Violet
	 * 700061-700063 Gems/Moon
	 * 700071-700073 Gems/Tortoise
	 * 
	 * 700102 MeteorScroll(Old)
	 * 700103 DBScroll*Old)
	 * 723253 random items
	 * 725000-790001 Skills?
	 * 
	 * 1000000-1002050 Super foods
	 * 1050000-1051000 Arrows
	 * 1060020-1060038 Gates/Windspells
	 * 1060040-1060090 Armor dye
	 * 1072010-1072059 Ores
	 * 
	 * 1088000 DragonBall
	 * 1088001 Meteor
	 * 1088002 MeteorTear
	 * 1090000-1100009 Money?
	 * 
	 */
	
	/* (non-Javadoc)
	 * @see java.lang.Object#hashCode()
	 */
	@Override
	public int hashCode() {
		final int prime = 31;
		int result = 1;
		result = prime * result
				+ ((identifier == null) ? 0 : identifier.hashCode());
		return result;
	}

	/* (non-Javadoc)
	 * @see java.lang.Object#equals(java.lang.Object)
	 */
	@Override
	public boolean equals(Object obj) {
		if (this == obj)
			return true;
		if (obj == null)
			return false;
		if (getClass() != obj.getClass())
			return false;
		ItemPrototype other = (ItemPrototype) obj;
		if (identifier == null) {
			if (other.identifier != null)
				return false;
		} else if (!identifier.equals(other.identifier))
			return false;
		return true;
	}
	
	/* (non-Javadoc)
	 * @see java.lang.Object#toString()
	 */
	@Override
	public String toString() {
		return "ItemPrototype [identifier=" + identifier + ", name=" + name
				+ ", maxDura=" + maxDura + ", worth=" + worth + ", CPWorth="
				+ CPWorth + "]";
	}

	static boolean isEquipment(long identifier) {
		return (  110000 <= identifier && identifier <= 160250 ) || // Armour
				( 410003 <= identifier && identifier <= 580339 ) || // Weapons
				( 900300 <= identifier && identifier <= 900999 );   // Shields
	}

	/**
	 * Equipment prototype is used for the static attributes
	 * of unique equipment instances
	 * @author Jan-willem Gmelig Meyling
	 */
	public final static class EquipmentPrototype extends ItemPrototype {
		
	//	public final ItemQuality itemQuality = ItemQuality.Normal;
		
		public final int classReq;
		
		public final int profReq;
		
		public final int lvlReq;
		
		public final int sexReq;
		
		public final int strReq;
		
		public final int agiReq;
		
		public final int minAtk;
		
		public final int maxAtk;
		
		public final int defence;
		
		public final int MDef;
		
		public final int MAttack;
		
		public final int dodge;
		
		public final int agility;

		EquipmentPrototype(Long identifier, String name, int maxDura, int worth, int CPWorth,
				int classReq, int profReq, int lvlReq,
				int sexReq, int strReq, int agiReq, int minAtk, int maxAtk,
				int defence, int mDef, int mAttack, int dodge, int agility) {
			super(identifier, name, maxDura, worth, CPWorth);
			this.classReq = classReq;
			this.profReq = profReq;
			this.lvlReq = lvlReq;
			this.sexReq = sexReq;
			this.strReq = strReq;
			this.agiReq = agiReq;
			this.minAtk = minAtk;
			this.maxAtk = maxAtk;
			this.defence = defence;
			this.MDef = mDef;
			this.MAttack = mAttack;
			this.dodge = dodge;
			this.agility = agility;
		}
		
		/* (non-Javadoc)
		 * @see java.lang.Object#toString()
		 */
		@Override
		public String toString() {
			return "EquipmentPrototype [classReq=" + classReq + ", profReq="
					+ profReq + ", lvlReq=" + lvlReq + ", sexReq=" + sexReq
					+ ", strReq=" + strReq + ", agiReq=" + agiReq + ", minAtk="
					+ minAtk + ", maxAtk=" + maxAtk + ", defence=" + defence
					+ ", MDef=" + MDef + ", MAttack=" + MAttack + ", dodge="
					+ dodge + ", agility=" + agility + ", identifier="
					+ identifier + ", name=" + name + ", maxDura=" + maxDura
					+ ", worth=" + worth + ", CPWorth=" + CPWorth + "]";
		}
		
		
	}

	private final static HashMap<Long, ItemPrototype> ITEM_PROTOTYPES = new HashMap<Long, ItemPrototype>(9999);

	public static ItemPrototype get(long id) {
		return ITEM_PROTOTYPES.get(id);
	}
	
	/**
	 * 
	 * @param string element
	 * @return a new {@code ItemPrototype} instance
	 * @throws InputMismatchException
     * @throws NoSuchElementException if input is exhausted
	 */
	static ItemPrototype read(String string) {
		String[] split = string.split("=");
		Long id = Long.valueOf(split[0]);
		boolean isEquipment = isEquipment(id);
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
		
		return isEquipment ? new EquipmentPrototype(id, name, maxDura, worth,
				CPWorth, classReq, profReq, lvlReq, sexReq, strReq, agiReq,
				minAtk, maxAtk, defence, mDef, mAttack, dodge, agility)
				: new ItemPrototype(id, name, maxDura, worth, CPWorth);
	}

	public static void read(File file) throws FileNotFoundException {
		Scanner sc = new Scanner(file);		
		if(sc.hasNext()&&sc.next().equalsIgnoreCase("[Items]")) {
			while(sc.hasNext()) {
				try {
					read(sc.next());
				} catch (Exception e) {	}
			}
		}
		
		sc.close();
		System.out.println("Loaded " + ITEM_PROTOTYPES.size() + " item prototypes");
	}
	
	public static void main(String[] args) throws FileNotFoundException {
		read(new File("ini/COItems.txt"));
	}

}
