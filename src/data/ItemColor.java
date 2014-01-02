package data;

/**
 * Enum for Item color
 * You can fetch the corresponding value by ItemColor.Black.value
 * @author Jan-Willem
 */
public enum ItemColor {
	Black(2), Orange(3), LightBlue(4), Red(5), Blue(6), Yellow(7), Purple(8), White(9);
	
	public final int value;
	
	ItemColor(int i) {
		this.value = i;
	}
}
