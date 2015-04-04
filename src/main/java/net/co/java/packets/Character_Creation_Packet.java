package net.co.java.packets;

public class Character_Creation_Packet
{
	private final String accountName;
	private final String characterName;
	private final String password;
	
	/*
	 *  2001 (female small), 2002 (female big), 1003 (male thin), 1004 (male big)
	 */

	private final int  body; 
	private final int   proffession; // in-game 'class' e.g. Taoist, Archer etc ...  
	private final long  identity; 
	
	public Character_Creation_Packet(IncomingPacket ip)
	{
		accountName = ip.readString(4, 16);
		characterName = ip.readString(20, 16);
		password = ip.readString(36, 16);
		body = ip.readUnsignedShort(52);
		proffession = ip.readUnsignedShort(54);
		identity = ip.readUnsignedInt(56); 
		
	}

	public String getAccountName() {
		return accountName;
	}

	public String getCharacterName() {
		return characterName;
	}

	public String getPassword() {
		return password;
	}

	public int getBody() {
		return body;
	}

	public int getProffession() {
		return proffession;
	}

	public long getIdentity() {
		return identity;
	}
	
	
	
}


