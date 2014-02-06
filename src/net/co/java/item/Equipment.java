package net.co.java.item;

@SuppressWarnings("unused")
public class Equipment extends Item {
	
	private int ClassReq;
	private int ProfReq;
	private int LvlReq;
	private int SexReq;
	private int StrReq;
	private int AgiReq;
	private int MinAtk;
	private int MaxAtk;
	private int Defense;
	private int MDef;
	private int MAttack;
	private int Dodge;
	private int AgiGive;
	private EquipmentSlot equipmentSlot;
	
	public Equipment(int itemID, String itemName, ItemColor itemColor, ItemQuality itemQuality, ItemEffect itemEffect, int worth, int cpWorth) {
		super(itemID, itemName, itemColor, itemQuality, itemEffect, worth, cpWorth);
		// TODO Auto-generated constructor stub
	}

}
