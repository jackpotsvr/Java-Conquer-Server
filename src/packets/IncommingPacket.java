package packets;

import java.nio.ByteBuffer;
import java.util.Arrays;

public abstract class IncommingPacket extends Packet {
	protected byte[] data;

	public IncommingPacket(short packetSize, PacketType type) {
		super(packetSize, type);
	}
	
	public IncommingPacket(short packetSize, PacketType type, byte[] data) {
		super(packetSize, type);
		this.receive(data);
	}
	
	public void receive(byte[] data) {
		this.data = data;
	}
	
	protected short getShort(int offset, int length) {
		return ByteBuffer.wrap(Arrays.copyOfRange(data, offset, offset + length)).order(java.nio.ByteOrder.LITTLE_ENDIAN).getShort();
	}
	
	protected long[] getLongArray(int offset, int length) {
		long[] temp = new long[length];
		for ( int i = 0; i < length; i++ )
			temp[i] = (long) data[offset+i];
//		System.arraycopy(data, 0, temp, offset, length);
		return temp;
	}
	
	protected int getInt(int offset, int length) {
		return ByteBuffer.wrap(Arrays.copyOfRange(data, offset, offset + length)).order(java.nio.ByteOrder.LITTLE_ENDIAN).getInt();
	}
	
	protected String getString(int offset, int length) {
		String output = "";
		for ( int i = offset; i < (length + offset); i++)
			if ( data[i] != 0 )
				output += (char) data[i];
		return output;
	}
}
