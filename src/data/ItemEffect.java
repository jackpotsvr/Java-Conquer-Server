package data;

/**
 * Enum for ItemEffects
 * You can fetch the corresponding value by ItemEffect.None.value
 * @author Jan-Willem
 */
public enum ItemEffect {
	None	((byte) 0x00),
	Poison	((byte) 0xC8),
	HP		((byte) 0xC9),
	MP		((byte) 0xCA),
	Shield	((byte) 0xCB),
	Horse	((byte) 0x64);
	
	public final byte value;
	
	ItemEffect(byte i) {
		this.value = i;
	}
}
