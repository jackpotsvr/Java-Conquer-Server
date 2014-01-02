package data;

import java.util.HashMap;

public class Player extends Entity {
	
	private int mesh = 381004;
	private int hairstyle = 315;
	private int gold = 1000;
	private int cps = 215;
	private int experience = 34195965;
	private int strength = 51;
	private int dexterity = 52;
	private int vitality = 53;
	private int spirit = 54;
	private int attributePoints = 500; 
	private int currentHP = 1000;
	private int currentMP = 1000;
	private int pkPoints = 0;
	private int level = 130;
	private int profession = 15;
	private int rebornCount = 0;
	private Player spouse;
	
	private HashMap<Integer, Item> inventory = new HashMap<Integer, Item>();
	private HashMap<EquipmentSlot, Equipment> equipment = new HashMap<EquipmentSlot, Equipment>();
	
	public Player(long identity, String name, Location location, int HP) {
		super(identity, name, location, HP);
		// TODO Auto-generated constructor stub
	}

	public int getMesh() {
		return mesh;
	}

	public void setMesh(int mesh) {
		this.mesh = mesh;
	}
	
	public int getHairStyle() {
		return hairstyle;
	}
	
	public void setHairStyle(int hairstyle) {
		this.hairstyle = hairstyle;
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

	public int getCurrentHP() {
		return currentHP;
	}

	public void setCurrentHP(int currentHP) {
		this.currentHP = currentHP;
	}

	public int getCurrentMP() {
		return currentMP;
	}

	public void setCurrentMP(int currentMP) {
		this.currentMP = currentMP;
	}

	public int getPkPoints() {
		return pkPoints;
	}

	public void setPkPoints(int pkPoints) {
		this.pkPoints = pkPoints;
	}

	public int getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = level;
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

}
