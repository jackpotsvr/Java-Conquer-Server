package net.co.java.packets;

import java.math.BigInteger;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.List;

import net.co.java.entity.Player;
import net.co.java.server.AbstractClient;

public class PacketWriter {

	public final ByteBuffer buffer;
	private final PacketType packetType;
	
	/**
	 * Create a new PacketWriter
	 * @param packet
	 */
	public PacketWriter(IncomingPacket packet) {
		this.packetType = packet.getPacketType();
		this.buffer = packet.buffer;
		this.buffer.position(4);
	}

	/**
	 * Create a new PacketWriter
	 * @param packetType
	 * @param size
	 */
	public PacketWriter(PacketType packetType, int size) {
		this.packetType = packetType;
		this.buffer = ByteBuffer.allocate(size).order(ByteOrder.LITTLE_ENDIAN);
		this.putUnsignedShort(size);
		this.putUnsignedShort(this.packetType.getType());
	}
	
	/**
	 * Set the offset for this PacketWriter
	 * @param offset
	 * @return this PacketWriter
	 */
	public PacketWriter setOffset(int offset) {
		this.buffer.position(offset);
		return this;
	}
	
	/**
	 * Increment the offset for this PacketWriter
	 * @param amount
	 * @return this PacketWriter
	 */
	public PacketWriter incrementOffset(int amount) {
		return this.setOffset(this.buffer.position() + amount);
	}

	/**
	 * Method used to put a bool into the packet.
	 * 
	 * @param b
	 *            boolean value to put.
	 * @return this PacketWriter
	 */
	public PacketWriter putBoolean(boolean b) {
		return this.putUnsignedByte(b ? 1 : 0);
	}

	/**
	 * Pushes an unsigned byte to the packet Since unsigned bytes can grow up to
	 * 2^8-1, and Java bytes can only grow up to 2^7-1 We supply the byte as
	 * short.
	 * 
	 * @param ubyte
	 * @return this PacketWriter
	 */
	public PacketWriter putUnsignedByte(short ubyte) {
		buffer.put((byte) (ubyte & 0xff));
		return this;
	}

	/**
	 * Pushes an unsigned byte to the packet Since unsigned bytes can grow up to
	 * 2^8-1, and Java bytes can only grow up to 2^7-1 We supply the byte as
	 * short.
	 * 
	 * @param ubyte
	 * @return this PacketWriter
	 */
	public PacketWriter putUnsignedByte(int ubyte) {
		buffer.put((byte) (ubyte & 0xff));
		return this;
	}

	/**
	 * Pushes an unsigned short to the packet Since unsigned shorts can grow up
	 * to 2^16-1, and Java shorts can only grow up to 2^15-1 we supply the short
	 * as integer
	 * 
	 * @param ushort value to be put
	 * @return this PacketWriter
	 */
	public PacketWriter putUnsignedShort(int ushort) {
		buffer.putShort((short) (ushort & 0xffff));
		return this;
	}

	/**
	 * Pushes an unsigned integer to the packet Since unsigned integers can grow
	 * up to 2^32-1, and Java integers can only grow up to 2^31-1 we supply the
	 * integer as long
	 * 
	 * @param uint value to be put
	 * @return this PacketWriter
	 */
	public PacketWriter putUnsignedInteger(long uint) {
		buffer.putInt((int) (uint & 0xffffffffL));
		return this;
	}
	
	public PacketWriter putSignedInteger(int sint) {
		buffer.putInt(sint);
		return this;
	}
	
	/**
	 * Pushes an unsigned long to the packet. Since unsigned longs can grow
	 * up to 2^64-1, and Java integers can only grow up to 2^64-1 we supply the
	 * long as BigInteger
	 * 
	 * @param ulong value to be put
	 * @return this PacketWriter
	 */
	public PacketWriter putUnsignedLong(BigInteger ulong) {
		buffer.putLong(ulong.longValue());
		return this;
	}

	/**
	 * Method to put a Java String to a byte[] and put it to the packet
	 * 
	 * @param str value to put
	 * @return this PacketWriter
	 */
	public PacketWriter putString(String str) {
		buffer.put(str.getBytes());
		return this;
	}

	/**
	 * Method to put a Java String to a byte[] and put it to the packet
	 * 
	 * @param str value to put
	 * @param length to extend
	 * @return this PacketWriter
	 */
	public PacketWriter putString(String str, int length) {
		if(str.length() <= length) {
			this.putString(str);
			if(str.length() < length) {
				this.incrementOffset(length - str.length());
			}
		}
		return this;
	}

	/**
	 * @return the length for this packet
	 */
	public int getLength() {
		return buffer.capacity();
	}
	
	/**
	 * Send the packet in this PacketWriter to a Player
	 * @param player
	 */
	public void send(Player player) {
		send(player.getClient());
	}
	
	/**
	 * Send the packet in this PacketWriter to a List of Players
	 * @param players
	 */
	public void sendTo(List<Player> players) {
		for ( Player player : players )
			send(player);
	}
	
	/**
	 * Send the packet in this PacketWriter to an array of Players
	 * @param players
	 */
	public void sendTo(Player[] players) {
		for ( Player player : players )
			send(player);
	}

	/**
	 * Send the packet in this PacketWriter to a Client
	 * @param client
	 */
	public void send(AbstractClient client) {
		client.write(buffer);
	}

}
