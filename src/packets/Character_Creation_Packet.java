package packets;

import java.io.IOException;
import conquerServer.ServerThread;

public class Character_Creation_Packet extends IncommingPacket
{
	private String accountName;
	private String characterName;
	private String password;
	
	/*
	 *  2001 (female small), 2002 (female big), 1003 (male thin), 1004 (male big)
	 */
	int body; 
	int proffession; // in-game 'class' e.g. Taoist, Archer etc ...  
	long identity; 
	
	public Character_Creation_Packet(PacketType packetType, byte[] data, ServerThread thread) throws IOException
	{
		super(packetType, data);
		accountName = this.readString(4, 16); 
		characterName = this.readString(20, 16);
		password = this.readString(36, 16);
		body = this.readUnsignedShort(52);
		proffession = this.readUnsignedShort(54);
		identity = this.readUnsignedInt(56); 
		
		System.out.println(characterName);
	}
}
