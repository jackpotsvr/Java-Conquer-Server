package packets;

import conquerServer.ByteConversion;

public class Thomas_Auth_Login_Forward{
	private Header packetHeader;
	private int Key2;
	private int Key1;
	private char[] gameServerIP; 
	private int gameServerPort;
	
	private void initialize(){
		packetHeader = new Header(); // IMPORTANT
		//packetHeader.packetSize = 32; // ALWAYS 32 BYTES
		packetHeader.setPacketSize((short) 32);
		//packetHeader.type = PacketType.auth_login_forward;
		packetHeader.setType(PacketType.auth_login_forward);
	}
	
	public Thomas_Auth_Login_Forward(){
		initialize();
		gameServerIP = "127.000.000.001".toCharArray();
		gameServerPort = 5816; 
	}
	
	public Thomas_Auth_Login_Forward(String ip, int port){
		initialize();
		gameServerIP = ip.toCharArray();
		gameServerPort = port;
	}
	
	public void setKey1(int key){
		Key1 = key;
	}
	
	public void setKey2(int key){
		Key2 = key;
	}
	
	public void setGameServerIP(String ip){
		gameServerIP = ip.toCharArray();
	}
	
	public void setGameServerPort(int port){
		gameServerPort = port;
	}

	public byte[] getPacketData(){
		
		byte[] data  = new byte[packetHeader.getPacketSize()]; /* declare and initialize byte array */ 
		
		String ip = new String(gameServerIP);
		
/*		System.arraycopy(ByteConversion.shortToTwoBytes(packetHeader.getPacketSize()), 0, data, 0, 2); // parse packet size
		System.arraycopy(ByteConversion.shortToTwoBytes(packetHeader.getType().value), 0, data, 2, 2); // parse packet type
		System.arraycopy(ByteConversion.intToFourBytes(Key2), 0, data, 4, 4); // parse Key 2
		System.arraycopy(ByteConversion.intToFourBytes(Key1), 0, data, 8, 4); // Parse Key 1
		System.arraycopy(ip.getBytes(), 0, data, 12, 15); // parse gameserver ip
		System.arraycopy(ByteConversion.intToFourBytes(gameServerPort), 0, data, 28, 4); // parse game server port.
*/						
		return data;
	}
	
}
