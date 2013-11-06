package packets;

import java.nio.ByteBuffer;
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
		// Convert split 16 bytes into 4 longs 
		long[] encrypted = new long[4];
		for ( int i = 0; i < 4; i++ )
			for ( int j = 0; j < 4; j++ )
				encrypted[i] += ((long) data[20+i*4+j] & 0xffL) << ( 8 * j);
		// Decrypt password
		PasswordCrypter.decrypt(encrypted);
		// Convert decrypted long[4] array to chars
		char[] decrypted = new char[16];
		for ( int i = 0; i < 4; i++ )
			for ( int j = 0; j < 4; j++ )
				if ((encrypted[i] >> ( 4 - j - 1 << 3) & 0xFF) != 0) 
					decrypted[i*4+3-j] = (char) (encrypted[i] >> ( 4 - j - 1 << 3) & 0xFF);
		// Put char[] to string
		return new String(decrypted);
	}
}
