package packets;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.nio.ByteBuffer;
import java.util.Arrays;

import conquerServer.Cryptography;

public class Packet {
	private short packetSize; 
	private PacketType type;
	protected Cryptography cipher;

	public Packet(short packetSize, PacketType type, Cryptography cipher) {
		this.setPacketSize(packetSize);
		this.setType(type);
		this.cipher = cipher;
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
	
	private static short getShort(byte[] data, int offset, int length) {
		return ByteBuffer.wrap(Arrays.copyOfRange(data, offset, offset + length)).order(java.nio.ByteOrder.LITTLE_ENDIAN).getShort();
	}
	
	private static PacketType getPacketType(short id) {
		for ( PacketType pt : PacketType.values() )
			if ( pt.value == id )
				return pt;
		return null;
	}
	
	public static Packet delegate(InputStream in, OutputStream out, Cryptography cipher) throws IOException {
		byte[] originalPacket = new byte[100];
		in.read(originalPacket);
		cipher.decrypt(originalPacket);
		return delegate(originalPacket, out, cipher);
	}
	
	public static Packet delegate(byte[] originalPacket, OutputStream out, Cryptography cipher) throws IOException {
		short packetSize = getShort(originalPacket, 0, 2);
		PacketType packetType = getPacketType(getShort(originalPacket, 2, 4));
		byte[] packetData = Arrays.copyOf(originalPacket, packetSize);
		
		switch(packetType) {
			case auth_login_packet:	return new Auth_Login_Packet(packetSize, packetType, packetData, out, cipher);
			default: return null;
		}
	}
}
