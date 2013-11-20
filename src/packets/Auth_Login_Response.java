package packets;

import java.io.IOException;

import conquerServer.ServerThread;

public class Auth_Login_Response extends IncommingPacket
{
	
	private long key2;
	private long key1;
	
	public Auth_Login_Response(PacketType packetType, byte[] data, ServerThread thread) throws IOException
	{
		super(packetType, data);
		
		key2 = this.readUnsignedInt(4);
		key1 = this.readUnsignedInt(8);
		
		 /*
		  * long aRGB, long type, long chatID, String from, String to, String  message) 2101 = Login Info, no enum yet 
		  */
        Message_Packet reply = new Message_Packet(0xFFFFFFFFL, 2101L, 0L, "SYSTEM", "ALLUSERS", "NEW_ROLE");
        thread.send(reply.data);
        
	}
	
	public long getKey2()
	{
		return key2;
	}
	
	public long getKey1()
	{
		return key1;
	}
}