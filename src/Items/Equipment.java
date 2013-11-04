package Items;

public class Equipment extends Item {
	public int ClassReq;
	public int ProfReq;
	public int LvlReq;
	public int SexReq;
	public int StrReq;
	public int AgiReq;
	public int MinAtk;
	public int MaxAtk;
	public int Defense;
	public int MDef;
	public int MAttack;
	public int Dodge;
	public int AgiGive;
	
	public Equipment(int itemID, String itemName, ItemColor itemColor, ItemQuality itemQuality, ItemEffect itemEffect, int worth, int cpWorth) {
		super(itemID, itemName, itemColor, itemQuality, itemEffect, worth, cpWorth);
		// TODO Auto-generated constructor stub
	}

}
