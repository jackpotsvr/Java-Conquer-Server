package packets;

import conquerServer.ByteConversion;

/* Packet is always incoming, not outgoing. */  

public class Auth_Login_Packet{
	private Header packetHeader; 
	private char[] username; /* always 16 bytes. */ 
	private char[] password;  /* always 16 bytes. */ 
	private char[] server;  /* always 16 bytes. */ 
	
	public Auth_Login_Packet(){
		packetHeader.setPacketSize( (short) 52);
		packetHeader.setType(PacketType.auth_login_packet);
		username = new char[16]; /* initialize array */ 
		password = new char[16]; /* initialize array */
		server = new char[16]; /* initialize array */	
	}
			
	public void setPacketData(byte[] data){
		byte[] temp = new byte[2];

		System.arraycopy(data, 0, temp, 0, 2);
		packetHeader.setPacketSize(ByteConversion.bytesToShort(temp));
		
		System.arraycopy(data, 0, temp, 2, 2);
		packetHeader.setType(ByteConversion.bytesToShort(temp));
		 
		System.arraycopy(data, 0, username, 4, 16);
		System.arraycopy(data, 0, password, 20, 16);
		System.arraycopy(data, 0, server, 36, 16);			
	}
	
}