package packets;

/* Packet is always incoming, not outgoing. */  

public class Auth_Login_Packet extends IncommingPacket {
	private String accoutName;
	private String password;
	private String serverName;
	
	public Auth_Login_Packet(short packetSize, PacketType packetType, byte[] packetData) {
		super(packetSize, packetType, packetData);
		accoutName	= this.getString(4,16);
		password	= this.getPassword(20);
		serverName	= this.getString(36, 16);
	}
	
}