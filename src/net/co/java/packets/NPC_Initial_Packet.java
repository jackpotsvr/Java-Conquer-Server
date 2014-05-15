package net.co.java.packets;

import net.co.java.entity.Entity;
import net.co.java.entity.NPC;
import net.co.java.npc.dialogs.TC_Conductress;
import net.co.java.npc.dialogs.NPC_Dialog;
import net.co.java.npc.dialogs.TrojanStar;
import net.co.java.packets.MessagePacket.MessageType;
import net.co.java.server.GameServerClient;

public class NPC_Initial_Packet implements PacketHandler
{
	private int npcUID; 
	
	public NPC_Initial_Packet(IncomingPacket ip)
	{
		npcUID = (int) ip.readUnsignedInt(4);
	}
	
	@Override
	public PacketWriter build() { 
		return null;
	}

	@Override
	public void handle(GameServerClient client) {
	
		NPC npc = null; 
		
		new MessagePacket(MessagePacket.SYSTEM, client.getPlayer().getName(), "You tried to talk to the NPC with UID: " + npcUID)
						.setMessageType(MessageType.TopLeft)
						.build().send(client);
		
		for(Entity e : 	client.getPlayer().getLocation().getMap().getEntities())
		{
			if(e.getIdentity() == npcUID)
			{
				npc = (NPC)e;
			}
		}
		if(npc != null)
		{
	
			NPC_Dialog dialog = null;
			
			switch(npcUID)
			{
				case 17:
					dialog = new TrojanStar(npc);
					break;
				case 103: // tc conductress
					dialog = new TC_Conductress(npc);
					break;
				default: 
					System.out.println("This npc is yet to be implemented.");
					break;
			}
			
			if(dialog != null)
			{
				client.getPlayer().setActiveDialog(dialog);
				dialog.handle(client);
			}
		}
	}

}
