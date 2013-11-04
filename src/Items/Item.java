package Items;

import java.util.HashMap;
import java.util.Map;

public class Item {	
	
	public int itemID;
	public String itemName;
	
	public ItemColor itemColor;
	public ItemQuality itemQuality;
	public ItemEffect itemEffect;
	
	public int worth; 
	public int cpWorth;

	public static Map<Integer, Item> items = new HashMap<Integer, Item>();
	
	public Item(int itemID, String itemName, ItemColor itemColor, ItemQuality itemQuality, ItemEffect itemEffect, int worth, int cpWorth) {
		this.itemID = itemID;
		this.itemName = itemName;
		
		this.itemColor = itemColor;
		this.itemQuality = itemQuality;
		this.itemEffect = itemEffect;
		
		this.worth = worth;
		this.cpWorth = cpWorth;
		items.put(itemID, this);
	}
	
	public String toString() {
		return itemName;
	}

}
