package packets;

import java.io.IOException;
import java.io.OutputStream;

import conquerServer.Cryptography;

/* Packet is always incoming, not outgoing. */  

public class Auth_Login_Packet extends IncommingPacket {
	private String accoutName;
	private String password;
	private String serverName;
	
	public Auth_Login_Packet(IncommingPacket ip) {
		super(ip);
		accoutName	= this.readString(4,16);
		password	= this.readPassword(20);
		serverName	= this.readString(36, 16);
	}
	
}