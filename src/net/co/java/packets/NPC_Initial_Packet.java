package net.co.java.packets;

import net.co.java.entity.Entity;
import net.co.java.entity.NPC;
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
	
		NPC npc = null; 
		
		for(Entity e : 	client.getPlayer().getLocation().getMap().getEntities())
		{
			if(e.getIdentity() == npcUID)
			{
				npc = (NPC)e;
			}
		}
		if(npc != null)
		{
			NPC_Dialog_Packet dialog = new NPC_Dialog_Packet(npc);
			
			dialog.build();
			
			dialog.NPC_Say("Hey, do you want some free Dragonballz?").send(client); 
			
			dialog.NPC_Link1(1, "Yes, sure.").send(client);
	
			dialog.NPC_Link1(255, "Nah. Bye.").send(client);
			
			dialog.NPC_SetFace().send(client);
			
			dialog.NPC_Finish().send(client);
		}
	}

}
