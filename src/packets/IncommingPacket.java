package packets;

import conquerServer.PasswordCrypter;

public class IncommingPacket extends Packet {
	protected byte[] data;
	
	public IncommingPacket(byte[] data) {
		this.data = data;
		this.setPacketSize(this.readUnsignedShort(0));
		this.setPacketType(this.readUnsignedShort(2));
	}
	
	public IncommingPacket(IncommingPacket ip) {
		this.data = ip.data;
		this.setPacketSize(ip.getPacketSize());
		this.setPacketType(ip.getPacketType());
	}

	/**
	 * Returns unsigned byte at specified offset (1 byte)
	 * @param {int} offset
	 * @return {short}  as the range of ubyte is 0 to 2^8-1
	 */
	public short readUnsignedByte(int offset) {
		return (short) (data[offset] & 0xFF);
	}
	
	/**
	 * Returns unsigned short at specified offset (2 bytes)
	 * @param {int} offset
	 * @return {int} as the range of ushort is 0 to 2^16-1
	 */
	public int readUnsignedShort(int offset) {
		return (int) ((data[offset+1] & 0xFF) << 8 | (data[offset] & 0xFF));
	}
	
	/**
	 * Returns unsigned integer at specified offset (4 bytes)
	 * @param {int} offset
	 * @return {long} as the range of uint is 0 to 2^32-1
	 */
	public long readUnsignedInt(int offset) {
		return ((data[offset + 3] & 0xFF) << 24 |(data[offset + 2] & 0xFF) << 16
                |(data[offset + 1] & 0xFF) << 8 |(data[offset] & 0xFF)); 
	}

	/**
	 * Returns string at specified offset and of specified length
	 * @param {int} offset
	 * @param {int} length
	 * @return {String}
	 */
	public String readString(int offset, int length) {
        byte[] output = new byte[length];
        System.arraycopy(data, offset, output, 0, length);
        return new String(output);
	}
	
	/**
	 * Function to fetch and decrypt the password from the Auth package
	 * @param {int} offset at which the password is at
	 * @return {String} password
	 */
	public String readPassword(int offset) {
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
