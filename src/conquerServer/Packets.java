package conquerServer;


public class Packets {
	
	public enum PacketType {
		auth_login_packet	((short) 0x41B),
		auth_login_forward ((short) 0x41F),
		auth_login_response ((short) 0x41C),
		message_packet ((short) 0x3EC),  /* also used for logging in. See https://spirited-fang.wikispaces.com/Logging+Into+Conquer+1.0 */
		char_info_packet ((short) 0x3EE),
		general_data_packet((short) 0x3F2);
		
		public final short value;
		
		PacketType(short i) {
			this.value = i;
		}
	}
	
	class Header{ // header of packet (first 4 bytes). 
		public short packetSize; 
		public PacketType type;
	}
	
	class Auth_Login_Packet{
		private Header packetHeader; 
		private char[] username; /* always 16 bytes. */ 
		private char[] password;  /* always 16 bytes. */ 
		private char[] server;  /* always 16 bytes. */ 
		
		public Auth_Login_Packet(){
			packetHeader.packetSize = 52; /* ALWAYS 52 bytes */
			packetHeader.type = PacketType.auth_login_packet; 
			username = new char[16]; /* initialize array */ 
			password = new char[16]; /* initialize array */
			server = new char[16]; /* initialize array */	
		}
		
		public byte[] getPacketData(){
			byte[] data = new byte[packetHeader.packetSize]; /* declare and initialize byte array */ 
			
			System.arraycopy(ToByteArray.shortToTwoBytes(packetHeader.packetSize), 0, data, 0, 2); // parse packet size
			System.arraycopy(ToByteArray.shortToTwoBytes(packetHeader.type.value), 0, data, 2, 4); // parse packet type
			System.arraycopy(username, 0, data, 4, 20); // parse username.
			System.arraycopy(password, 0, data, 20, 36); // parse password.
			System.arraycopy(server, 0, data, 36, 52); // parse server.
			
			return data;
		} 
		
		class Auth_Login_Forward{
			private Header packetHeader;
			private int Key2;
			private int Key1;
			private char[] gameServerIP; 
			private int gameServerPort;
			
			private void initialize(){
				packetHeader.packetSize = 32; // ALWAYS 32 BYTES
				packetHeader.type = PacketType.auth_login_forward;
			}
			
			public Auth_Login_Forward(){
				initialize();
				gameServerIP = "127.000.000.001".toCharArray();
				gameServerPort = 5816; 
			}
			
			public Auth_Login_Forward(String IP, int port){
				gameServerIP = IP.toCharArray();
				gameServerPort = port;
			}
			
			public void setKey1(int key){
				Key1 = key;
			}
			
			public void setKey2(int key){
				Key2 = key;
			}
			
			public byte[] getPacketData(){
				
				byte[] data = new byte[packetHeader.packetSize]; /* declare and initialize byte array */ 
				
				System.arraycopy(ToByteArray.shortToTwoBytes(packetHeader.packetSize), 0, data, 0, 2); // parse packet size
				System.arraycopy(ToByteArray.shortToTwoBytes(packetHeader.type.value), 0, data, 2, 4); // parse packet type
				System.arraycopy(ToByteArray.intToFourBytes(Key2), 0, data, 4, 4); // parse Key 2
				System.arraycopy(ToByteArray.intToFourBytes(Key1), 0, data, 8, 4); // Parse Key 1
				System.arraycopy(gameServerIP, 0, data, 12, 16); // parse gameserver ip
				System.arraycopy(ToByteArray.intToFourBytes(gameServerPort), 0, data, 28, 4); // parse game server port.
								
				return data;
			}
			
		}
		
	}
	
	//public static PacketType packetType = PacketType.Fixed;
	//packetType.value;

}