package packets;

import java.io.IOException;
import java.io.OutputStream;

import conquerServer.Cryptography;

/* Packet is always incoming, not outgoing. */  

public class Auth_Login_Packet extends IncommingPacket {
	private String accoutName;
	private String password;
	private String serverName;
	private OutputStream out;
	
	public Auth_Login_Packet(short packetSize, PacketType packetType, byte[] packetData, OutputStream out, Cryptography cipher) throws IOException {
		super(packetSize, packetType, packetData, cipher);
		this.out = out;
		accoutName	= this.getString(4,16);
		password	= this.getPassword(20);
		serverName	= this.getString(36, 16);
		respond();
	}
	
	public void respond() throws IOException {
		Auth_Login_Response ALR = new Auth_Login_Response(1000000, 000,  "localhost", 5816, cipher);
		ALR.send(out);
	}
	
}