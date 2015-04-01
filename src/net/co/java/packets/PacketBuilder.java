package net.co.java.packets;


import net.co.java.packets.serialization.PacketSerializer;

public class PacketBuilder implements PacketWrapper {
	private Packet packet; 
	
	public PacketBuilder(Packet packet) { 
		this.packet = packet;
	}

	@Override
	public PacketWriter build() {
		return new PacketSerializer(packet).serialize();
	}
}
