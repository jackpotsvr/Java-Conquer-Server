package packets;

public class OutgoingPacket extends Packet {

	/**
	 * Creates a new Outgoing packet with given packetType
	 * Remember to set data length with setSize or setData!
	 * @param packetType
	 */
	public OutgoingPacket(PacketType packetType) {
		super(packetType);
		setData(new byte[packetType.getSize()]);
	}
	
	/**
	 * Creates a new Outgoing packet with given PacketType
	 * @param packetType
	 * @param data
	 */
	public OutgoingPacket(PacketType packetType, byte[] data) {
		super(packetType);
		setData(data);
	}
	
	/**
	 * Creates a new data array with size length and updates the packet type and size
	 * @param length
	 */
	public void setSize(int length) {
		setData(new byte[length]);
	}
	
	/**
	 * Updates the data array for the packet
	 * @param data
	 */
	public void setData(byte[] data) {
		this.data = data;
		offset = 0;
		this.putUnsignedShort(data.length);
		this.putUnsignedShort(packetType.getType());
	}
	
	/**
	 * Set the offset for packets with blank spaces. 
	 * @param offset vale
	 */
	public void setOffset(int offset)
	{
		this.offset = offset;
	}
	
	/**
	 *  Pushes an unsigned byte to the packet
	 *  Since unsigned bytes can grow up to 2^8-1, and Java bytes can only grow up to 2^7-1
	 *  We supply the byte as short.
	 * @param ubyte
	 */
	public void putUnsignedByte(short ubyte) {
		data[offset++] = (byte) ( ubyte & 0xFF );
	}
	
	/**
	 *  Pushes an unsigned byte to the packet
	 *  Since unsigned bytes can grow up to 2^8-1, and Java bytes can only grow up to 2^7-1
	 *  We supply the byte as short.
	 * @param ubyte
	 */
	public void putUnsignedByte(int ubyte) {
		data[offset++] = (byte) ( ubyte & 0xFF );
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
	 * Method used to put a bool into the packet.
	 * @param b boolean value to put.
	 */
	public void putBoolean(boolean b) {
		  putUnsignedByte(b?1:0);
	}

}
