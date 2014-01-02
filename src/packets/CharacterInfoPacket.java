package packets;

import data.Player;
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
	private String name;
	private String spouseName;


	private CharacterInfoPacket(String name, String spouseName)
	{
		super(PacketType.CHAR_INFO_PACKET, new byte[71 + name.length() + spouseName.length()]);
		this.name = name;
		this.spouseName = spouseName;
	}
	
	public CharacterInfoPacket(Player player) {
		super(PacketType.CHAR_INFO_PACKET, new byte[71 + player.getName().length() + player.getSpouseName().length()]);
		putUnsignedInteger(player.getIdentity());
		putUnsignedInteger(player.getMesh());
		putUnsignedShort(player.getHairstyle());
		putUnsignedInteger(player.getGold());
		putUnsignedInteger(player.getCps());
		putUnsignedInteger(player.getExperience());
		setOffset(46);
		putUnsignedShort(player.getStrength());
		putUnsignedShort(player.getDexterity());
		putUnsignedShort(player.getVitality());
		putUnsignedShort(player.getSpirit());
		putUnsignedShort(player.getAttributePoints());
		putUnsignedShort(player.getHP());
		putUnsignedShort(player.getMana());
		putUnsignedShort(player.getPkPoints());
		putUnsignedByte(player.getLevel());
		putUnsignedByte(player.getProfession());
		setOffset(65);
		putUnsignedByte(player.getRebornCount());
		putBoolean(true);
		putUnsignedByte(2);
		putUnsignedByte(player.getName().length());
		putString(player.getName());
		putUnsignedByte(player.getSpouseName().length());
		putString(player.getSpouseName());
	}
	
	private void fill() {
		this.putUnsignedInteger(this.identity);
		this.putUnsignedInteger(this.mesh);
		this.putUnsignedShort(this.hairstyle);
		this.putUnsignedInteger(this.gold);
		this.putUnsignedInteger(this.cps);
		this.putUnsignedInteger(this.experience);
		this.setOffset(46);
		this.putUnsignedShort(this.strength);
		this.putUnsignedShort(this.dexterity);
		this.putUnsignedShort(this.vitality);
		this.putUnsignedShort(this.spirit);
		this.putUnsignedShort(this.attributePoints);
		this.putUnsignedShort(this.currentHP);
		this.putUnsignedShort(this.currentMP);
		this.putUnsignedShort(this.pkPoints);
		this.putUnsignedByte(this.level);
		this.putUnsignedByte(this.profession);
		this.setOffset(65);
		this.putUnsignedByte(this.rebornCount);
		this.putBoolean(this.displayNames);
		this.putUnsignedByte(this.stringCount);
		this.putUnsignedByte(this.name.length());
		this.putString(this.name);
		this.putUnsignedByte(this.spouseName.length());
		this.putString(this.spouseName);
	}

	public static CharacterInfoPacket create()
	{
		/* Uiteindelijk worden deze gegevens uit database geladen... :) */
		String name = "Jackpotsvr";
		String spouseName = "firetao250";
		CharacterInfoPacket packet = new CharacterInfoPacket(name, spouseName);
		
		packet.identity = 1000000L;
		packet.mesh = 381004;
		packet.hairstyle = 315;
		packet.gold = 1000;
		packet.cps = 215;
		packet.experience = 34195965;
		packet.strength = 50;
		packet.dexterity = 50;
		packet.vitality = 50;
		packet.spirit = 50;
		packet.attributePoints = 500; 
		packet.currentHP = 1000;
		packet.currentMP = 1000;
		packet.pkPoints = 0;
		packet.level = 130;
		packet.profession = 15;
		packet.rebornCount = 0;
		packet.displayNames = true;
		packet.stringCount = 2;
		
		/* Uiteindelijk worden deze gegevens uit database geladen... :) */
		
		packet.fill();

		return packet;
	}
}


// type == CHAR_INFO_PACKET