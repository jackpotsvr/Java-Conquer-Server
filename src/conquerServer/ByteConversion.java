package conquerServer;

import java.nio.ByteBuffer;

public class ByteConversion {
	/* TO BE REPLACED WITH BytesToInt */
	public static int fourBytesToInt(byte[] bytes){  
		return ((bytes[0] & 0xFF) << 24) | ((bytes[1] & 0xFF) << 16) | ((bytes[2] & 0xFF) << 8) | (bytes[3] & 0xFF);
	}
	//todo
	public static int bytesToInt(byte[] bytes){
		return ByteBuffer.wrap(bytes).order(java.nio.ByteOrder.LITTLE_ENDIAN).getInt();
	}

	public static byte[] intToFourBytes(int value){ /* int to byte array with length 4 */ 
		return ByteBuffer.allocate(4).putInt(value).array();
	}
	
	public static byte[] shortToTwoBytes(short value){ /* short to byte array with length 2 */ 
		return ByteBuffer.allocate(2).putShort(value).array();  
	}
	
	public static short bytesToShort(byte[] bytes){
		return ByteBuffer.wrap(bytes).order(java.nio.ByteOrder.LITTLE_ENDIAN).getShort();
	}
	
	public static String bytesToString(byte[] bytes) {
		String output = "";
		for ( byte b : bytes )
			output += (char)b ;
		return output;
	}
}
