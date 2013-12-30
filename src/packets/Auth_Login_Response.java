package packets;

import java.io.IOException;

import conquerServer.GameServerThread;

public class Auth_Login_Response extends IncommingPacket
{

	private long inKey1;
	private long inKey2;
	
	public Auth_Login_Response(PacketType packetType, byte[] data, GameServerThread thread) throws IOException
	{
		super(packetType, data);
		
		inKey2 = this.readUnsignedInt(4);
		inKey1 = this.readUnsignedInt(8);
		thread.setKeys(inKey1, inKey2);
		
		 /*
		  * long aRGB, long type, long chatID, String from, String to, String  message) 2101 = Login Info, no enum yet 
		  */
        //Message_Packet reply = new Message_Packet(0xFFFFFFFFL, 2101L, 0L, "SYSTEM", "ALLUSERS", "NEW_ROLE");
		Message_Packet reply1 = new Message_Packet(0xFFFFFFFFL, 2101L, 0L, "SYSTEM", "ALLUSERS", "ANSWER_OK");
		CharacterInfoPacket reply2 = CharacterInfoPacket.create();
		
		
		thread.send(reply1.data);
		thread.send(reply2.data);
		
		
        
	}

}