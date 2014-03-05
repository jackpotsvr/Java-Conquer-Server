package net.co.java.packets;

import net.co.java.cipher.Cryptographer;
import net.co.java.entity.Entity;
import net.co.java.entity.Player;
import net.co.java.packets.PacketType.UnimplementedPacketTypeException;
import net.co.java.server.ServerThread;

/**
 * A wrapper providing methods to read unsigned values from the
 * byte array from the Sockets' {@code InputStream}.
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public class IncomingPacket {
	
	final byte[] data;
	private final int length;
	private final PacketType packetType;
	
	/**
	 * Construct a new {@code IncommingPacket} based on a byte-array
	 * @param data
	 * @throws UnimplementedPacketTypeException 
	 */
	public IncomingPacket(byte[] data) throws UnimplementedPacketTypeException {
		this.data = data;
		this.length = readUnsignedShort(0);
		this.packetType = PacketType.valueOf(readUnsignedShort(2));
	}
	
	/**
	 * Returns unsigned byte at specified offset (1 byte)
	 * @param offset
	 * @return {short}  as the range of ubyte is 0 to 2^8-1
	 */
	public short readUnsignedByte(int offset) {
		return (short) (data[offset] & 0xFF);
	}
	
	/**
	 * @param position
	 * @return byte at position
	 */
	public byte getByte(int position) {
		return data[position];
	}
	
	/**
	 * Returns unsigned short at specified offset (2 bytes)
	 * @param offset
	 * @return {int} as the range of ushort is 0 to 2^16-1
	 */
	public int readUnsignedShort(int offset) {
		return (int) ((data[offset+1] & 0xFF) << 8 | (data[offset] & 0xFF));
	}
	
	/**
	 * Returns unsigned integer at specified offset (4 bytes)
	 * @param offset
	 * @return long as the range of uint is 0 to 2^32-1
	 */
	public long readUnsignedInt(int offset) {
		return (	(long) data[offset] & 0xFF)
				| (((long) data[offset + 1] & 0xFF) << 8)
				| (((long) data[offset + 2] & 0xFF) << 16)
				| (((long) data[offset + 3] & 0xFF) << 24);
	}

	/**
	 * Returns string at specified offset and of specified length
	 * @param offset
	 * @param length
	 * @return {String}
	 */
	public String readString(int offset, int length) {
        byte[] output = new byte[length];
        System.arraycopy(data, offset, output, 0, length);
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
		return Cryptographer.decryptPassword(data, 20);
	}
	
	/**
	 * @return the length for this packet, based on the data
	 */
	public int getLength() {
		return length;
	}
	
	/**
	 * @return the {@code PacketType} for this {@code IncommingPacket}
	 */
	public PacketType getPacketType() {
		return packetType;
	}
	
	@Override
	public String toString() {
		return packetType.toString() + " : " + bytesToHex(data) + " (" + data.length + ")";
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
	
	public void send(ServerThread client) {
		client.offer(data);
	}
	
	public void sendToSurroundings(Player me) {
		for ( Entity e : me.getSurroundingPlayers() )
			this.send(((Player) e).getClient());
	}
	
}
