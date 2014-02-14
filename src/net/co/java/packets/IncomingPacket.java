package net.co.java.packets;

import java.util.Arrays;

import net.co.cipher.Cryptographer;
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
		return ((data[offset + 3] & 0xFF) << 24 |(data[offset + 2] & 0xFF) << 16
                |(data[offset + 1] & 0xFF) << 8 |(data[offset] & 0xFF)); 
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
        return new String(output);
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
		return packetType.toString() + " : " + Arrays.toString(data);
	}
	
	public void send(ServerThread client) {
		client.offer(data);
	}
	
	public void sendToSurroundings(Player me) {
		for ( Entity e : me.getSurroundingPlayers() )
			this.send(((Player) e).getClient());
	}
	
}
