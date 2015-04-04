package net.co.java.packets.packethandlers;

import net.co.java.entity.Entity;
import net.co.java.entity.NPC;
import net.co.java.packets.AbstractPacketHandler;
import net.co.java.packets.MessagePacket;
import net.co.java.packets.NPCInitialPacket;
import net.co.java.packets.Packet;
import net.co.java.packets.serialization.PacketSerializer;
import net.co.java.server.GameServerClient;

/**
 * Created by Thomas on 02/04/2015.
 */
public class NPCInitialPacketHandler extends AbstractPacketHandler {

    public NPCInitialPacketHandler(Packet packet) {
        super(packet);
    }

    @Override
    public void handle(GameServerClient client) {
        if(!(packet instanceof NPCInitialPacket))
            return;

        NPCInitialPacket nip = (NPCInitialPacket) packet;

        NPC npc = null;

        new PacketSerializer(
                new MessagePacket(MessagePacket.SYSTEM, client.getPlayer().getName(), "You tried to talk to the NPC with UID: " + nip.getNpcUID())
                        .setMessageType(MessagePacket.MessageType.TOPLEFT)
        ).serialize().send(client);

        for(Entity e : 	client.getPlayer().getLocation().getMap().getEntities())
        {
            if(e.getIdentity() == nip.getNpcUID())
            {
                npc = (NPC)e;
            }
        }
        if(npc != null) {

            //NPC_Dialog dialog = null;

            switch ((int) nip.getNpcUID()) {
                case 17:
                    //dialog = new TrojanStar(npc);
                    break;
                case 103: // tc conductress
                    //dialog = new TC_Conductress(npc);
                    break;
                default:
                    System.out.println("This npc is yet to be implemented.");
                    break;
            }
        }
    }
}
