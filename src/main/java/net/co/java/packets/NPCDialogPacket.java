package net.co.java.packets;

import net.co.java.packets.packethandlers.GeneralDataPacketHandler;
import net.co.java.packets.packethandlers.NPCDialogHandler;
import net.co.java.packets.serialization.*;

/**
 * Created by Thomas on 06/04/2015.
 */
@PacketLength(length = 14)
@Type(type = PacketType.NPC_DIALOG_PACKET)
@Bidirectional(handler = NPCDialogHandler.class)
public class NPCDialogPacket extends Packet {

    private static final byte SUBTYPE_NPC_SAY = 1;
    private static final byte SUBTYPE_NPC_LINK1 = 2;
    private static final byte SUBTYPE_NPC_LINK2 = 3;
    private static final byte SUBTYPE_NPC_SETFACE = 4;
    private static final byte SUBTYPE_NPC_FINISH = 100;

    @PacketValue(type = PacketValueType.UNSIGNED_SHORT)
    @Offset(4)
    private int unknownValue1;

    @PacketValue(type = PacketValueType.UNSIGNED_SHORT)
    @Offset(6)
    private int unknownValue2;

    @PacketValue(type = PacketValueType.UNSIGNED_SHORT)
    @Offset(8)
    private int npcFace;

    @PacketValue(type = PacketValueType.UNSIGNED_BYTE)
    @Offset(10)
    private short dialog;

    @PacketValue(type = PacketValueType.UNSIGNED_BYTE)
    @Offset(11)
    private short subType;

    @PacketValue(type = PacketValueType.UNSIGNED_BYTE)
    @Offset(12)
    private short stringCount;

    @PacketValue(type = PacketValueType.STRING_WITH_LENGTH)
    @Offset(13)
    private String text;

    private NPCDialogPacket() {
        super(null);
        this.setType(PacketType.NPC_DIALOG_PACKET);
        unknownValue1 = 0;
        unknownValue2 = 0;
        npcFace = 0;
        stringCount = 0;
        text = "";
    }

    public static NPCDialogPacket npcSay(final String npcMessage) {
        NPCDialogPacket packet = new NPCDialogPacket();
        packet.dialog = 0xFF;
        packet.subType = SUBTYPE_NPC_SAY;
        packet.stringCount = 1;
        packet.text = npcMessage;
        return packet;
    }

    public static NPCDialogPacket npcLink1(final short dialogNumber, final String text) {
        NPCDialogPacket packet = new NPCDialogPacket();
        packet.dialog = dialogNumber;
        packet.subType = SUBTYPE_NPC_LINK1;
        packet.stringCount = 1;
        packet.text = text;
        return packet;
    }

    public static NPCDialogPacket npcLink2(final short dialogNumber, final String text) {
        NPCDialogPacket packet = new NPCDialogPacket();
        packet.dialog = dialogNumber;
        packet.subType = SUBTYPE_NPC_LINK2;
        packet.stringCount = 1;
        packet.text = text;
        return packet;
    }

    public static NPCDialogPacket npcSetFace(final int face) {
        NPCDialogPacket packet = new NPCDialogPacket();
        packet.unknownValue1 = 10;
        packet.unknownValue2 = 10;
        packet.npcFace = face;
        packet.dialog = 0xFF;
        packet.subType = SUBTYPE_NPC_SETFACE;
        return packet;
    }

    public static NPCDialogPacket npcFinish() {
        NPCDialogPacket packet = new NPCDialogPacket();
        packet.dialog = 0xFF;
        packet.subType = SUBTYPE_NPC_FINISH;
        return packet;
    }

    public NPCDialogPacket(IncomingPacket ip) {
        super(ip);
    }

    public short getDialog() {
        return dialog;
    }


}
