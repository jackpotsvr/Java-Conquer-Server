package packets;

public class Packet {
	private int packetSize; 
	private PacketType packetType;

	public Packet() {
		
	}
	
	public Packet(int packetSize, PacketType packetType) {
		this.setPacketSize(packetSize);
		this.setPacketType(packetType);
	}
	
	public int getPacketSize() {
		return packetSize;
	}

	public void setPacketSize(int packetSize) {
		this.packetSize = packetSize;
	}

	public PacketType getPacketType() {
		return packetType;
	}

	public void setPacketType(PacketType type) {
		packetType = type;
	}
	
	public void setPacketType(int type) {
		packetType = PacketType.get(type);
	}

}
