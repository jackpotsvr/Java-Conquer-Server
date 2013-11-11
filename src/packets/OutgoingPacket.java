package packets;

import java.io.IOException;
import java.io.OutputStream;

public class OutgoingPacket extends Packet {
	private byte[] data;
	private int offset = 0;

	public OutgoingPacket(int packetSize, PacketType type) {
		super(packetSize, type);
		data = new byte[packetSize];
	}
	
	/**
	 * Pushes an unsigned short to the packet
	 * Since unsigned shorts can grow up to 2^16-1, and Java shorts can only grow up to 2^15-1
	 * we supply the short as integer
	 * @param {int} value to be put
	 */
	public void putUnsignedShort(int ushort) {
        data[offset++] = (byte) (ushort & 0xFF);
        data[offset++] = (byte) ((ushort >> 8) & 0xFF);
	}
	
	/**
	 * Pushes an unsigned integer to the packet
	 * Since unsigned integers can grow up to 2^32-1, and Java integers can only grow up to 2^31-1
	 * we supply the integer as long
	 * @param {int} value to be put
	 */
	public void putUnsignedInteger(long uint) {
		data[offset++] = (byte)  (uint & 0xFF);
		data[offset++] = (byte) ((uint >> 8) & 0xFF);
		data[offset++] = (byte) ((uint >> 16) & 0xFF);
		data[offset++] = (byte) ((uint >> 24) & 0xFF);
	}
	
	/**
	 * Method to put a Java String to a byte[] and put it to the packet
	 * @param {String} value to put
	 */
	public void putString(String str) {
        byte[] bytes = str.getBytes();

        for (byte b : bytes)
        	data[offset++] = b;
	}
	
	/**
	 * Method to put a Java String to a byte[] and put it to the packet
	 * @param {String} value to put
	 * @param {int} length to extend
	 */
	public void putString(String str, int length) {
        byte[] bytes = str.getBytes();

        for (byte b : bytes)
        	data[offset++] = b;
        
        offset += length - str.length();
	}
	
	/**
	 * Method to encrypt packet
	 * @param {Cryptography} Cryptographer object of current session
	 */
	public void encrypt(Cryptographer cipher) {
		cipher.Encrypt(data);
	}
	
	/**
	 * Writes the packet to the specified OutputStream
	 * @param out
	 * @throws IOException
	 */
	public void send(OutputStream out) throws IOException {
		out.write(data);
		out.flush();
	}
	

}
