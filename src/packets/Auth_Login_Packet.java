package packets;

import conquerServer.PasswordCrypter;

/* Packet is always incoming, not outgoing. */  

public class Auth_Login_Packet extends IncommingPacket {
	private String accoutName; /* always 16 bytes. */ 
	private String password;  /* always 16 bytes. */ 
	private String serverName;  /* always 16 bytes. */ 
	
	public Auth_Login_Packet(short packetSize, PacketType packetType, byte[] packetData) {
		super(packetSize, packetType, packetData);
		accoutName	= this.getString(4,16);
		
		long[] password = new long[16];
		for ( int i = 0; i < 16; i++ )
			password[i] = (long) (data[i+20] & 0xff);
		PasswordCrypter.decrypt(password);
		String output = "";
		for ( int i = 0; i < 16; i++)
			output += (char) password[i];
		
		//password	= this.getString(20, 16);
		serverName	= this.getString(36, 16);
	}
	
}