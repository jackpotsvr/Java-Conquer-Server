package Items;

import java.util.HashMap;
import java.util.Map;

enum ItemQuality {
	Fixed(0), Other(1), Normal(2), NormalV1(3), NormalV2(4), NormalV3(5), Refined(6), Unique(7), Elite(8), Super(9);
	int quality;
	ItemQuality(int p) { quality = p; }
	int getValue() { return quality; }
}

enum ItemColor {
	Black(2), Orange(3), LightBlue(4), Red(5), Blue(6), Yellow(7), Purple(8), White(9);
	int color;
	ItemColor(int p) { color = p; }
	int getValue() { return color; }
}

enum ItemEffects {
	None(0), Poison(0xC8), HP(0xC9), MP(0xCA), Shield(0xCB), Horse(0x64);
	int effect;
	ItemEffects(int p) { effect = p; }
	int getValue() { return effect; }
}

public class Item {	
	
	public int itemID;
	public String itemName;
	public int worth; 
	public int cpWorth;
	private Object quality;

	public static Map<Integer, Item> items = new HashMap<Integer, Item>();
	
	public Item(int itemID, String itemName, int worth, int cpWorth) {
		this.itemID = itemID;
		this.itemName = itemName;
		this.worth = worth;
		this.cpWorth = cpWorth;
		items.put(itemID, this);
	}
	
	public String toString() {
		return itemName;
	}

}
