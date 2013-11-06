package packets;

import java.io.IOException;
import java.io.OutputStream;
import java.nio.ByteBuffer;

import conquerServer.Cryptography;

public class OutgoingPacket extends Packet {
	private byte[] data;
	private int index = 0;

	public OutgoingPacket(short packetSize, PacketType type, Cryptography cipher) {
		super(packetSize, type, cipher);
		data = new byte[packetSize];
	}
	
	/**
	 * Converts a short to a byte[2] array and appends it to the packet
	 * @param value
	 */
	public void put(short value) {
		put(ByteBuffer.allocate(2).putShort(value).array());
	}
	
	/**
	 * Converts an integer to a byte[4] array and appends it to the packet
	 * @param value
	 */
	public void put(int value) {
		put(ByteBuffer.allocate(4).putInt(value).array());
	}
	
	/**
	 * Converts a string to a char[] array of specified length and appends it to the packet
	 * @param s
	 * @param length
	 */
	public void put(String s, int length) {
		char[] ca = new char[length];
		s.getChars(0, (s.length() < length)? s.length() : length, ca, 0);
		put(ca);
	}
	
	/**
	 * Converts a char[] to a byte[] and appends it to the packet
	 * @param ca
	 */
	public void put(char[] ca) {
		for(char b : ca)
			put((byte) b);
	}
	
	/**
	 * Appends a byte[] to the packet
	 * @param ba
	 */
	public void put(byte[] ba) {
		for(byte b : ba)
			put(b);
	}
	
	/**
	 * Puts a single byte to the packet
	 * @param b
	 */
	public void put(byte b) {
		data[index] = b;
		index++;
	}
	
	/**
	 * Writes the packet to the specified OutputStream
	 * @param out
	 * @throws IOException
	 */
	public void send(OutputStream out) throws IOException {
		cipher.encrypt(data);
		out.write(data);
		out.flush();
	}
	

}
