package net.co.java.packets;

import net.co.java.server.Server.GameServer.Client;

/**
 * A {@code PacketHandler} is a {@code PacketWrapper} that implements the way
 * the {@code GameServer} should handle the {@code IncomingPacket}
 * 
 * @author Jan-Willem Gmelig Meyling
 * 
 */
public interface PacketHandler extends PacketWrapper {

	/**
	 * Delegate the Packet
	 * @param client
	 */
	void handle(Client client);
	
}
