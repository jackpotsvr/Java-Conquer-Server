package packets;

import conquerServer.ByteConversion;

// incoming packet only! (to be set with a packet, get the specific values */ 

public class Auth_Login_Response {
	private Header packetHeader; 
	private int key2;
	private int key1;
	private char[] szInfo; /* unknown information, to be ignored for now */
	
	/**
	 *  @param Constructor without parameters. Use it with 'setPacket(packet);' 
	 */
	Auth_Login_Response(){
		// do nothing, required to have a constructor with parameters. 
	}
	
	/** 
	 * @param Constructor that sets the values to the packet. 
	 */
	Auth_Login_Response(byte[] data){ 
		setPacket(data);
	}
	
	
	/** 
	 * To set values for the packet, if you didn't do so with the constructor.  
	 */
	public void setPacket(byte[] data){
		byte[] temp = new byte[2];
		
		System.arraycopy(data, 0, temp, 0, 2);
		packetHeader.setPacketSize(ByteConversion.bytesToShort(temp));
		
		System.arraycopy(data, 0, temp, 2, 2); 
		packetHeader.setType(ByteConversion.bytesToShort(temp));
		
		temp = new byte[4];
		
		System.arraycopy(data, 0, temp, 4, 4);
		key2 = ByteConversion.bytesToInt(temp);
		
		System.arraycopy(data, 0, temp, 8, 4);
		key1 = ByteConversion.bytesToInt(temp);
		
		System.arraycopy(data, 0, szInfo, 12, 16);
	}
	
	public int getKey1(){
		return key1;
	}
	
	public int getKey2(){
		return key2;
	}
	
	/**
	 * 
	 * @return Returns our unknown. We might want to figure out what this is?! 
	 */
	public char[] getUnkown(){
		return szInfo;
	}
	
	
	
}


