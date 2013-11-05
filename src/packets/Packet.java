package packets;

import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteBuffer;
import java.util.Arrays;

import conquerServer.Cryptography;

public class Packet {
	private short packetSize; 
	private PacketType type;

	public Packet(short packetSize, PacketType type) {
		this.setPacketSize(packetSize);
		this.setType(type);
	}
	
	public short getPacketSize() {
		return packetSize;
	}

	public void setPacketSize(short packetSize) {
		this.packetSize = packetSize;
	}

	public PacketType getType() {
		return type;
	}

	public void setType(PacketType type) {
		this.type = type;
	}

	protected static Cryptography cipher = new Cryptography(); 
	
	private static short getShort(byte[] data, int offset, int length) {
		return ByteBuffer.wrap(Arrays.copyOfRange(data, offset, offset + length)).order(java.nio.ByteOrder.LITTLE_ENDIAN).getShort();
	}
	
	private static PacketType getPacketType(short id) {
		for ( PacketType pt : PacketType.values() )
			if ( pt.value == id )
				return pt;
		return null;
	}
	
	public static Packet delegate(InputStream in) throws IOException {
		byte[] originalPacket = new byte[100];
		in.read(originalPacket);
		cipher.decrypt(originalPacket);
		return delegate(originalPacket);
	}
	
	public static Packet delegate(byte[] originalPacket) {
		short packetSize = getShort(originalPacket, 0, 2);
		PacketType packetType = getPacketType(getShort(originalPacket, 2, 4));
		byte[] packetData = Arrays.copyOf(originalPacket, packetSize);
		
		switch(packetType) {
			case auth_login_packet:	return new Auth_Login_Packet(packetSize, packetType, packetData);
			default: return null;
		}
	}
}
