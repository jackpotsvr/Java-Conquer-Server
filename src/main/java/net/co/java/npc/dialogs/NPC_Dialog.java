//package net.co.java.npc.dialogs;
//
//import net.co.java.entity.NPC;
//import net.co.java.packets.Packet;
//import net.co.java.packets.PacketHandler;
//import net.co.java.packets.PacketType;
//import net.co.java.packets.PacketWriter;
//import net.co.java.server.GameServerClient;
//
//public abstract class NPC_Dialog implements PacketHandler
//{
//	private int packetLength = 14; 
//	private NPC npc; 
//	protected int input; 
//	public final static int INITIAL_DIALOG = -1; // used this number because it's not sent, and not in the range 0-255.
//	public final static int DIALOG_QUIT = 255;
//
//	public enum NPC_Dialog_Type
//	{
//		NPC_SAY,
//		NPC_LINK1,
//		NPC_LINK2,
//		NPC_SETFACE,
//		NPC_FINISH;
//	}
//	
//	public NPC_Dialog(NPC npc)
//	{
//		this.npc = npc;
//		this.input = INITIAL_DIALOG; 
//	}
//	/*
//	public NPC_Dialog_Packet(short dialognr,  NPC_Dialog_Type type)
//	{
//		this.type = type; 
//	} */ 
//
//	@Override
//	public PacketWriter build() {		
//		return null;
//	}
//	
//	/** All the packets for NPC Dialogs... */ 
//	
//	public PacketWriter NPC_Say(String text)
//	{
//		int length = packetLength + text.length();
//		return new PacketWriter(PacketType.NPC_DIALOG_PACKET, length)
//			.setOffset(10)
//			.putUnsignedByte(0xFF)
//			.putUnsignedByte(1)
//			.putUnsignedByte(1)
//			.putUnsignedByte(text.length()) // length of all the strings together. 
//			.putString(text);
//	}
//	
//	/**
//	 * Used for predefined texts.
//	 * @return
//	 */
//	public PacketWriter NPC_Link1(int dialog, String text)
//	{
//		return new PacketWriter(PacketType.NPC_DIALOG_PACKET, packetLength + text.length())
//			.setOffset(10)
//			.putUnsignedByte(dialog)
//			.putUnsignedByte(2)
//			.putUnsignedByte(1)
//			.putUnsignedByte(text.length())
//			.putString(text);
//	}
//	
//	/**
//	 * Used for text input fields.
//	 * @return
//	 */
//	public PacketWriter NPC_Link2(int dialog, String text)
//	{
//		return new PacketWriter(PacketType.NPC_DIALOG_PACKET, packetLength+5)
//			.setOffset(10)
//			.putUnsignedByte(dialog)
//			.putUnsignedByte(3)
//			.putUnsignedByte(1)
//			.putUnsignedByte(text.length())
//			.putString(text);
//	}
//	
//	public PacketWriter NPC_SetFace()
//	{
//		return new PacketWriter(PacketType.NPC_DIALOG_PACKET, packetLength)
//			.putUnsignedShort(10)
//			.putUnsignedShort(10)
//			.putUnsignedShort(npc.getFace())
//			.putUnsignedByte(0xFF)
//			.putUnsignedByte(4);
//	}
//	
//	public PacketWriter NPC_Finish()
//	{
//		return new PacketWriter(PacketType.NPC_DIALOG_PACKET, packetLength)
//			.setOffset(10)
//			.putUnsignedByte(0xFF)
//			.putUnsignedByte(100);
//	}
//
//	@Override
//	public void handle(GameServerClient client, Packet packet) {
//		switch(input)
//		{
//			case DIALOG_QUIT:
//				client.getPlayer().setActiveDialog(null); // remove the reference to this dialog. 
//				break; // not neccessary to send another packet;
//			default: 
//				npc_handle(client); 
//				break;
//		}
//	}
//	
//	protected abstract void npc_handle(GameServerClient client);
//
//	public void setInput(int input) {
//		this.input = input; 
//	}	
//}
//
//
//
