package net.co.java.packets;

import net.co.java.packets.NPC_Dialog_Packet.NPC_Dialog_Type;
import net.co.java.server.GameServerClient;

public class NPC_Initial_Packet implements PacketHandler
{
	long npcUID; 
	
	public NPC_Initial_Packet(IncomingPacket ip)
	{
		npcUID = ip.readUnsignedInt(4);
	}
	
	@Override
	public PacketWriter build() { 
		return null;
	}

	@Override
	public void handle(GameServerClient client) {
		NPC_Dialog_Packet dialog = new NPC_Dialog_Packet((short)255, NPC_Dialog_Type.NPC_SAY);
		//dialog.build().send(client);
		
		dialog.NPC_Say("Hey, do you want some free Dragonballz?").send(client); 
		
		dialog.NPC_Link1(1, "Yes, sure.").send(client);

		dialog.NPC_Link1(255, "Nah. Bye.").send(client);
		
		dialog.NPC_SetFace().send(client);
		
		dialog.NPC_Finish().send(client);
	}

}
