package net.co.java.packets;

import java.util.List;

import net.co.java.entity.Entity;
import net.co.java.entity.Player;
import net.co.java.server.ServerThread;

public class PacketWriter {

	private int offset = 0;
	private final byte[] data;
	private final PacketType packetType;

	public PacketWriter(PacketType packetType, int size) {
		this.packetType = packetType;
		this.data = new byte[size];
		this.putUnsignedShort(size);
		this.putUnsignedShort(this.packetType.getType());
	}
	
	/**
	 * Set the offset for this PacketWriter
	 * @param offset
	 * @return this PacketWriter
	 */
	public PacketWriter setOffset(int offset) {
		this.offset = offset;
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
	public PacketWriter putUnsignedByte(short ubyte) {
		data[offset++] = (byte) (ubyte & 0xFF);
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
		data[offset++] = (byte) (ubyte & 0xFF);
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
		data[offset++] = (byte) (ushort & 0xFF);
		data[offset++] = (byte) ((ushort >> 8) & 0xFF);
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
		data[offset++] = (byte) (uint & 0xFF);
		data[offset++] = (byte) ((uint >> 8) & 0xFF);
		data[offset++] = (byte) ((uint >> 16) & 0xFF);
		data[offset++] = (byte) ((uint >> 24) & 0xFF);
		return this;
	}

	/**
	 * Method to put a Java String to a byte[] and put it to the packet
	 * 
	 * @param str value to put
	 * @return this PacketWriter
	 */
	public PacketWriter putString(String str) {
		byte[] bytes = str.getBytes();

		for (byte b : bytes)
			data[offset++] = b;

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
		byte[] bytes = str.getBytes();

		for (byte b : bytes)
			data[offset++] = b;

		offset += length - str.length();
		return this;
	}

	/**
	 * Method used to put a bool into the packet.
	 * 
	 * @param b
	 *            boolean value to put.
	 * @return this PacketWriter
	 */
	public PacketWriter putBoolean(boolean b) {
		putUnsignedByte(b ? 1 : 0);
		return this;
	}

	/**
	 * @return the length for this packet
	 */
	public int getLength() {
		return data.length;
	}
	
	public void send(Player player) {
		player.getClient().offer(data);
	}
	
	/**
	 * Send the response to the Client
	 * @param client
	 */
	public void send(ServerThread client) {
		client.offer(data);
	}
	
	public void sendToSurroundings(Entity me) {
		for ( Player player : me.getSurroundingPlayers() )
			player.getClient().offer(data);
	}
	
	public void sendTo(List<Player> players) {
		for ( Player player : players )
			player.getClient().offer(data);
	}

}
