package net.co.java.packets.packethandlers;

import net.co.java.packets.AbstractPacketHandler;
import net.co.java.packets.Packet;
import net.co.java.packets.PacketHandler;
import net.co.java.packets.PacketWriter;
import net.co.java.server.GameServerClient;

public class MessagePacketHandler extends AbstractPacketHandler {
	public MessagePacketHandler(Packet packet) {
		super(packet);
	}

	@Override
	public void handle(GameServerClient client) {
		
	}
}
