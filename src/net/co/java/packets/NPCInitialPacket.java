package net.co.java.packets;

import net.co.java.packets.packethandlers.NPCInitialPacketHandler;
import net.co.java.packets.serialization.*;

/**
 * Created by Thomas on 02/04/2015.
 */
@Type(type = PacketType.NPC_INITIAL_PACKET)
@Bidirectional(handler = NPCInitialPacketHandler.class)
@PacketLength(length = 24)
public class NPCInitialPacket extends Packet {


    @PacketValue(type = PacketValueType.UNSIGNED_INT)
    @Offset(4)
    private long npcUID;

    public NPCInitialPacket(IncomingPacket ip) {
        super(ip);
    }

    public long getNpcUID() {
        return npcUID;
    }

    public void setNpcUID(long npcUID) {
        this.npcUID = npcUID;
    }
}
