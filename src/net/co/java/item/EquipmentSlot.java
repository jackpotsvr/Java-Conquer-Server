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
	Helm		((byte) 0x1),
	Necklace	((byte) 0x2),
	Armor		((byte) 0x3),
	RightHand	((byte) 0x4),
	LeftHand	((byte) 0x5),
	Ring		((byte) 0x6),
	Talisman	((byte) 0x7),
	Boots		((byte) 0x8),
	Garment		((byte) 0x9),
	AttackTalisman 	((byte) 0xA),
	DefenseTalisman	((byte) 0xB),
	Steed		((byte) 0xC);
	
	public final byte value;
	
	EquipmentSlot(byte i) {
		this.value = i;
	}
}
