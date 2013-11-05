package testProjects;

public class first255bytes {

	public static void main(String[] args) {
		// TODO Auto-generated method stub
		byte b = 0x00;
		for ( int i = 0; i <= 255; i++) {
			String bin = Integer.toBinaryString(b & 0xFF);
			String hex = Integer.toHexString(b & 0xFF);
			bin = "00000000".substring(bin.length()) + bin;
			hex = "00".substring(hex.length()) + hex;
			byte c = (byte) (b & 0xFF);
			System.out.println(bin + " : " + hex + " : " + b++ + " " + c);
		}
	}

}
