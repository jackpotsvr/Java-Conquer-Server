package net.co.java.packets.packethandlers;

import net.co.java.packets.Packet;
import net.co.java.packets.PacketHandler;
import net.co.java.packets.PacketWriter;
import net.co.java.server.GameServerClient;

public class NoPacketHandler implements PacketHandler {

	@Override
	public PacketWriter build() {
		return null;
	}

	@Override
	public void handle(GameServerClient client, Packet packet) { 
		
	}

}
