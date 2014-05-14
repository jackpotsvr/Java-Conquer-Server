package net.co.java.packets;

import net.co.java.entity.NPC;

public class NPC_Dialog_Packet implements PacketWrapper
{
	private int packetLength = 14; 
	private NPC npc; 

	public enum NPC_Dialog_Type
	{
		NPC_SAY,
		NPC_LINK1,
		NPC_LINK2,
		NPC_SETFACE,
		NPC_FINISH;
	}
	
	public NPC_Dialog_Packet(NPC npc)
	{
		this.npc = npc;
	}
	/*
	public NPC_Dialog_Packet(short dialognr,  NPC_Dialog_Type type)
	{
		this.type = type; 
	} */ 

	@Override
	public PacketWriter build() {		
		return null;
	}
	
	
	public PacketWriter NPC_Say(String text)
	{
		int length = packetLength + text.length();
		return new PacketWriter(PacketType.NPC_DIALOG_PACKET, length)
			.setOffset(10)
			.putUnsignedByte(0xFF)
			.putUnsignedByte(1)
			.putUnsignedByte(1)
			.putUnsignedByte(text.length()) // length of all the strings together. 
			.putString(text);
	}
	
	/**
	 * Used for predefined texts.
	 * @return
	 */
	public PacketWriter NPC_Link1(int dialog, String text)
	{
		return new PacketWriter(PacketType.NPC_DIALOG_PACKET, packetLength + text.length())
			.setOffset(10)
			.putUnsignedByte(1)
			.putUnsignedByte(2)
			.putUnsignedByte(1)
			.putUnsignedByte(text.length())
			.putString(text);
	}
	
	/**
	 * Used for text input fields.
	 * @return
	 */
	public PacketWriter NPC_Link2()
	{
		return new PacketWriter(PacketType.NPC_DIALOG_PACKET, packetLength+5)
			.setOffset(10)
			.putUnsignedByte(255)
			.putUnsignedByte(3)
			.putUnsignedByte(1)
			.putUnsignedByte(4)
			.putString("Nope");
	}
	
	public PacketWriter NPC_SetFace()
	{
		return new PacketWriter(PacketType.NPC_DIALOG_PACKET, packetLength)
			.putUnsignedShort(10)
			.putUnsignedShort(10)
			.putUnsignedShort(npc.getFace())
			.putUnsignedByte(0xFF)
			.putUnsignedByte(4);
	}
	
	public PacketWriter NPC_Finish()
	{
		return new PacketWriter(PacketType.NPC_DIALOG_PACKET, packetLength)
			.setOffset(10)
			.putUnsignedByte(0xFF)
			.putUnsignedByte(100);
	}	
}



