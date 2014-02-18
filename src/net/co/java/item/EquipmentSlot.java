/**
 * 
 */
package net.co.java.item;

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
}
