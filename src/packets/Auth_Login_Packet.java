package packets;

import conquerServer.ServerThread;

public class Auth_Login_Packet extends IncommingPacket {
	private String accoutName;
	private String password;
	private String serverName;
	
	public Auth_Login_Packet(PacketType packetType, byte[] data, ServerThread thread) {
		super(packetType, data);
		accoutName	= this.readString(4,16);
		password	= Cryptographer.decryptPassword(data, 20);
		serverName	= this.readString(36, 16);
		
		Auth_Login_Forward ALF = new Auth_Login_Forward(thread);
		thread.offer(ALF.data);
	}
	
}