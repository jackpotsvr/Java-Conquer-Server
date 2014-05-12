package net.co.java.packets;

import net.co.java.entity.NPC;

public class NPC_Spawn_Packet implements PacketWrapper
{
	private int packetLength = 21; // packet length without string length
	private NPC npc; 
	
	public NPC_Spawn_Packet(NPC npc)
	{
		this.npc = npc; 
		packetLength += npc.getName().length();
	}
	
	@Override
	public PacketWriter build() {
		System.out.println(packetLength);
		PacketWriter pw = new PacketWriter(PacketType.NPC_SPAWN_PACKET, packetLength)
		.putUnsignedInteger(npc.getIdentity())
		.putUnsignedShort(npc.getLocation().getxCord())
		.putUnsignedShort(npc.getLocation().getyCord())
		.putUnsignedShort(npc.getModel())
		.putUnsignedShort(npc.getNpc_interactions())
		.putUnsignedByte(npc.getDirection())
		.putUnsignedShort(npc.getNpc_flags())
		.putUnsignedByte(1) // str count
		.putUnsignedByte(npc.getName().length())
		.putString(npc.getName());
	
		return pw;
	}
	
}
