import packets.OutgoingPacket;
import packets.PacketType;

public class CharacterInfoPacket extends OutgoingPacket
{
	private long identity;
	
	/**
	 * body + (avatar * 10000); 
	 * for instance: Avatar ID 38, and body 1004 makes 381004. 
	 */
	private long mesh;
	private int hairstyle;
	private long gold;
	private long cps;
	private long experience;
	private int strength;
	private int dexterity;
	private int vitality;
	private int spirit;
	private int attributePoints;
	private int currentHP;
	private int currentMP;
	private int pkPoints;
	private short level;
	private short profession;
	private short rebornCount;
	private boolean displayNames;
	private short stringCount;
	private short nameLength;
	private String name;
	private short spouseLength;
	private String spouseName;


	private CharacterInfoPacket(String name, String spouseName)
	{
		super(PacketType.CHAR_INFO_PACKET, new byte[66 + name.length() + spouseName.length()]);
		

	}
	public static CharacterInfoPacket create()
	{
		
		String name = "Jackpotsvr";
		String spouseName = "firetao250";
		CharacterInfoPacket packet = new CharacterInfoPacket(name, spouseName);
		
		packet.identity = 5234902;
		packet.mesh = 381004;
		packet.hairstyle = 315;
		packet.gold = 999999999;
		packet.cps = 999999999;
		packet.experience = 0;
		
		packet.strength = 500;
		packet.dexterity = 500;
		packet.vitality = 500;
		packet.spirit = 500;
		packet.attributePoints = 500; 
		packet.currentHP = 1000;
		packet.currentMP = 1000;
		packet.pkPoints = 0;
		packet.level = 130;
		packet.profession = 15;
		packet.rebornCount = 0;
		packet.displayNames = true;
		packet.stringCount = 2;
		
		
		
		return packet;
	}
}


// type == CHAR_INFO_PACKET