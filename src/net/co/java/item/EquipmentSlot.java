/**
 * 
 */
package net.co.java.item;

import net.co.java.item.ItemInstance.EquipmentInstance.Socket;

/**
 * Enum for EquipmentSlot
 * You can fetch the corresponding value by EquipmentSlot.None.value
 * @author Jan-Willem
 *
 */
public enum EquipmentSlot {
	Inventory	(0),
	Helm		(1),
	Necklace	(2),
	Armor		(3),
	RightHand	(4),
	LeftHand	(5),
	Ring		(6),
	Talisman	(7),
	Boots		(8),
	Garment		(9),
	AttackTalisman 	(10),
	DefenseTalisman	(11),
	Steed		(12);
	
	public final int value;
	
	EquipmentSlot(int i) {
		this.value = i;
	}
	
	public static EquipmentSlot valueOf(int value) {
		for ( EquipmentSlot e : EquipmentSlot.values() )
			if ( e.value == value )
				return e;
		return null;
	}
}
