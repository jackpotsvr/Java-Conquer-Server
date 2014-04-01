package net.co.java.packets;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import net.co.java.cipher.Cryptographer;
import net.co.java.packets.PacketType.UnimplementedPacketTypeException;

/**
 * A wrapper providing methods to read unsigned values from the
 * byte array from the Sockets' {@code InputStream}.
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public class IncomingPacket {
	
	final ByteBuffer buffer;
	private final PacketType packetType;
	
	/**
	 * Construct a new {@code IncommingPacket} based on a {@code ByteBuffer}
	 * @param buffer
	 * @throws UnimplementedPacketTypeException
	 */
	public IncomingPacket(ByteBuffer buffer) throws UnimplementedPacketTypeException {
		this.buffer = buffer.order(ByteOrder.LITTLE_ENDIAN);
		this.packetType = PacketType.valueOf(this.readUnsignedShort(2));
	}

	/**
	 * Returns unsigned byte at specified offset (1 byte)
	 * @param offset
	 * @return {short}  as the range of ubyte is 0 to 2^8-1
	 */
	public short readUnsignedByte(int offset) {
		return ((short) (buffer.get(offset) & (short) 0xff));
	}
	
	/**
	 * @param position
	 * @return byte at position
	 */
	public byte getByte(int position) {
		return buffer.get(position);
	}
	
	/**
	 * Returns unsigned short at specified offset (2 bytes)
	 * @param offset
	 * @return {int} as the range of ushort is 0 to 2^16-1
	 */
	public int readUnsignedShort(int offset) {
		return (buffer.getShort(offset) & 0xffff);
	}
	
	/**
	 * Returns unsigned integer at specified offset (4 bytes)
	 * @param offset
	 * @return long as the range of uint is 0 to 2^32-1
	 */
	public long readUnsignedInt(int offset) {
		return ((long) buffer.getInt(offset) & 0xffffffffL);
	}

	/**
	 * Returns string at specified offset and of specified length
	 * @param offset
	 * @param length
	 * @return {String}
	 */
	public String readString(int offset, int length) {
        byte[] output = new byte[length];
        System.arraycopy(buffer.array(), offset, output, 0, length);
		/*
		 * We had to replace the null termination for Strings because they did
		 * not work well with database models: ERROR: invalid byte sequence for
		 * encoding "UTF8": 0x00 FIX
		 */
        return new String(output).replaceAll("[\u0000]", "");
	}
	
	/**
	 * @return decrypted password
	 */
	public String readPassword() {
		/*
		 * We had to replace the null termination for Strings because they did
		 * not work well with database models: ERROR: invalid byte sequence for
		 * encoding "UTF8": 0x00 FIX
		 */
		return Cryptographer.decryptPassword(buffer.array(), 20).replaceAll("[\u0000]", "");
	}
	
	/**
	 * @return the length for this packet, based on the data
	 */
	public int getLength() {
		return buffer.limit();
	}
	
	/**
	 * @return the {@code PacketType} for this {@code IncommingPacket}
	 */
	public PacketType getPacketType() {
		return packetType;
	}
	
	@Override
	public String toString() {
		return packetType.toString() + " : " + bytesToHex(buffer.array()) + " (" + getLength() + ")";
	}
	
	private final static char[] hexArray = "0123456789ABCDEF".toCharArray();
	
	private static String bytesToHex(byte[] bytes) {
	    char[] hexChars = new char[bytes.length * 2];
	    for ( int j = 0; j < bytes.length; j++ ) {
	        int v = bytes[j] & 0xFF;
	        hexChars[j * 2] = hexArray[v >>> 4];
	        hexChars[j * 2 + 1] = hexArray[v & 0x0F];
	    }
	    return new String(hexChars);
	}
	
}
