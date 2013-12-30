package packets;

public class IncommingPacket extends Packet {
		
	public IncommingPacket(PacketType packetType, byte[] data) {
		super(packetType);
		this.data = data;
	}

	/**
	 * Returns unsigned byte at specified offset (1 byte)
	 * @param {int} offset
	 * @return {short}  as the range of ubyte is 0 to 2^8-1
	 */
	protected short readUnsignedByte(int offset) {
		return (short) (data[offset] & 0xFF);
	}
	
	/**
	 * Returns unsigned short at specified offset (2 bytes)
	 * @param {int} offset
	 * @return {int} as the range of ushort is 0 to 2^16-1
	 */
	protected int readUnsignedShort(int offset) {
		return (int) ((data[offset+1] & 0xFF) << 8 | (data[offset] & 0xFF));
	}
	
	/**
	 * Returns unsigned integer at specified offset (4 bytes)
	 * @param {int} offset
	 * @return {long} as the range of uint is 0 to 2^32-1
	 */
	protected long readUnsignedInt(int offset) {
		return ((data[offset + 3] & 0xFF) << 24 |(data[offset + 2] & 0xFF) << 16
                |(data[offset + 1] & 0xFF) << 8 |(data[offset] & 0xFF)); 
	}

	/**
	 * Returns string at specified offset and of specified length
	 * @param {int} offset
	 * @param {int} length
	 * @return {String}
	 */
	protected String readString(int offset, int length) {
        byte[] output = new byte[length];
        System.arraycopy(data, offset, output, 0, length);
        return new String(output);
	}
	
}
