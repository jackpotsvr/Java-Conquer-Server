package packets;

public class Message_Packet extends OutgoingPacket {
	
	public Message_Packet(long aRGB, long type, long chatID, String from, String to, String  message){
		this.setPacketSize(30 + from.length() + to.length() + message.length());
		this.setPacketType(PacketType.message_packet);
		setDataLength(this.getPacketSize());
		this.putUnsignedShort(this.getPacketSize());
		this.putUnsignedShort(this.getPacketType().value);
		this.putUnsignedInteger(aRGB);
		this.putUnsignedInteger(type);
		this.putUnsignedInteger(chatID);
		this.putUnsignedInteger(0); // Receiver avatar.
		this.putUnsignedInteger(0); // Sender avatar.
		this.putUnsignedByte( (short) 4); // always 4, with suffix.
		
		this.putUnsignedByte(from.length());
		this.putString(from);
		
		this.putUnsignedByte(to.length());
		this.putString(to);
		
		
		this.putUnsignedByte(0x00); //SUFFIX
	//	this.putUnsignedByte(0x00); //SUFFIX
		
		this.putUnsignedByte(message.length());
		this.putString(message);
		
		System.out.println("Hi");
	}
	

}


