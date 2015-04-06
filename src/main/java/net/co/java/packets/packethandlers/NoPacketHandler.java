package net.co.java.packets.packethandlers;

import net.co.java.packets.Packet;
import net.co.java.packets.PacketWriter;
import net.co.java.server.GameServerClient;

public class NoPacketHandler extends AbstractPacketHandler {

	public NoPacketHandler(Packet packet) {
		super(packet);
	}

	@Override
	public PacketWriter build() {
		return null;
	}

	@Override
	public void handle(GameServerClient client) { 
		
	}

}
