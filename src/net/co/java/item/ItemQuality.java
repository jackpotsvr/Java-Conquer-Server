package net.co.java.item;

/**
 * Enum for Item quality
 * You can fetch the corresponding value by ItemQuality.Fixed.value
 * @author Jan-Willem
 */
public enum ItemQuality {
	Fixed	(0),
	Other	(1),
	Normal	(2),
	NormalV1(3),
	NormalV2(4),
	NormalV3(5),
	Refined (6),
	Unique	(7),
	Elite	(8),
	Super	(9);
	
	public final int value;
	
	private ItemQuality(int i) {
		this.value = i;
	}
	
	public ItemQuality valueOf(int i) {
		for ( ItemQuality iq : ItemQuality.values() ) {
			if ( iq.value == i ) {
				return iq;
			}
		}
		return null;
	}
}