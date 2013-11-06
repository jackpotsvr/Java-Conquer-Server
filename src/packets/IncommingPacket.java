package packets;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.util.Arrays;

import conquerServer.PasswordCrypter;

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
	
	protected int getInt(int offset, int length) {
		return ByteBuffer.wrap(Arrays.copyOfRange(data, offset, offset + length)).order(java.nio.ByteOrder.LITTLE_ENDIAN).getInt();
	}

	protected short getShort(int offset, int length) {
		return ByteBuffer.wrap(Arrays.copyOfRange(data, offset, offset + length)).order(java.nio.ByteOrder.LITTLE_ENDIAN).getShort();
	}
	
	/**
	 * Function to fetch a String from a package
	 * @param offset
	 * @param length
	 * @return 
	 */
	protected String getString(int offset, int length) {
		char[] output = new char[length];
		for ( int i = 0; i < length; i++ )
			if ( data[i+offset] != 0 )
				output[i] = (char) data[i+offset];
		return new String(output);
	}

	/**
	 * Function to fetch and decrypt the password from the Auth package
	 * @param {int} offset at which the password is at
	 * @return {String} password
	 */
	protected String getPassword(int offset) {
		// String builder for our output
		StringBuilder sb = new StringBuilder();
		// Convert split 16 bytes into 4 longs 
		long[] encrypted = new long[4];
		for ( int i = 0; i < 4; i++ )
			for ( int j = 0; j < 4; j++ )
				encrypted[i] += ((long) data[offset+i*4+j] & 0xff) << (8 * j);
		// Encrypt password
		PasswordCrypter.decrypt(encrypted);
		// Convert 4 decrypted longs back to chars and put them in char[]
		longs: for ( int i = 0; i < 4; i++ ) {
			for ( int j = 0; j < 4; j++ ) {
				long singleByte = encrypted[i] >> (j << 3) & 0xff;
				// Look for end of string
				if(singleByte == 0 )
					break longs;
				// Else, append char to string
				sb.append((char) singleByte);
			}
		}
		// Return decrypted string
		return sb.toString();
	}
}
