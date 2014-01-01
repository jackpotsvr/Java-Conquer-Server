package packets;

import java.io.IOException;

import conquerServer.AuthServerThread;
import conquerServer.GameServerThread;

public class Auth_Login_Response extends IncommingPacket
{
	
	public Auth_Login_Response(PacketType packetType, byte[] data, GameServerThread thread)
	{
		super(packetType, data);
		
		long inKey2 = this.readUnsignedInt(4);
		long inKey1 = this.readUnsignedInt(8);
		thread.setKeys(inKey1, inKey2);
		
		 /*
		  * long aRGB, long type, long chatID, String from, String to, String  message) 2101 = Login Info, no enum yet 
		  */
        //Message_Packet reply = new Message_Packet(0xFFFFFFFFL, 2101L, 0L, "SYSTEM", "ALLUSERS", "NEW_ROLE");
		Message_Packet reply1 = new Message_Packet(0xFFFFFFFFL, 2101L, 0L, "SYSTEM", "ALLUSERS", "ANSWER_OK");
		CharacterInfoPacket reply2 = CharacterInfoPacket.create();
		
		
		thread.offer(reply1.data);
		thread.offer(reply2.data);
		
	}
	
	public Auth_Login_Response(PacketType packetType, byte[] data, AuthServerThread thread) {
		super(packetType, data);
		
		long identity = this.readUnsignedInt(4);
		long resNumber = this.readUnsignedInt(8);
		String resLocation = this.readString(12,16);
		System.out.println("ALR: " + resLocation + " " + identity + ", "  + resNumber);
	}

}