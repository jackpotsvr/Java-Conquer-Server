package net.co.java.packets;

import net.co.java.packets.serialization.PacketSerializerFactory;

public class PacketBuilder implements PacketWrapper {
	private Packet packet; 
	
	public PacketBuilder(Packet packet) { 
		this.packet = packet;
	}

	@Override
	public PacketWriter build() {
		return PacketSerializerFactory.valueOf(packet.getType())
					.getInstance(packet)
					.serialize(); 
	}
}
