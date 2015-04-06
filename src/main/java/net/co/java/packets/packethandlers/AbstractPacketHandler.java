package net.co.java.packets.packethandlers;


import net.co.java.packets.Packet;
import net.co.java.packets.PacketBuilder;
import net.co.java.packets.PacketHandler;
import net.co.java.packets.PacketWriter;

public abstract class AbstractPacketHandler implements PacketHandler {
	protected Packet packet;
	
	public AbstractPacketHandler(Packet packet) { 
		this.packet = packet; 
	}
	
	@Override
	public PacketWriter build() {
		return new PacketBuilder(packet).build();
	}
}
