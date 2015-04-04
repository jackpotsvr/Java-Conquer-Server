package net.co.java.packets;


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
