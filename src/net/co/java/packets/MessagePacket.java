package net.co.java.packets;


/**
 * This packet is used to send messages to other players and to instruct
 * the client on what errors to display on login. The server uses this
 * packet by routing it to it's destination (the other player).
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public class MessagePacket {

	public final static String SYSTEM = "SYSTEM";
	public final static String ALL_USERS = "ALLUSERS";
	
	private long aRGB = 0xFFFFFFFFL;
	private MessageType type = MessageType.Talk;
	private long chatID = 0L;
	
	private final String from;
	private final String to;
	private final String message;
	
	/**
	 * Construct a new Message packet with the from, to and message
	 * values already set, and the aRGB, type and chatID set to default
	 * values
	 * @param from
	 * @param to
	 * @param message
	 */
	public MessagePacket(String from, String to, String message) {
		this.from = from;
		this.to = to;
		this.message = message;
	}
	
	/**
	 * Construct a new Message packet based on a IncomingPacket
	 * @param ip
	 */
	public MessagePacket(IncomingPacket ip) {
		this.aRGB = ip.readUnsignedInt(4);
		this.type = MessageType.valueOf(ip.readUnsignedInt(8));
		this.chatID = ip.readUnsignedInt(12);
		int fromLength = ip.readUnsignedByte(25);
		this.from = ip.readString(26, fromLength);
		int toLength = ip.readUnsignedByte(27 + fromLength);
		this.to = ip.readString(28 + fromLength, toLength);
		int messageLength = ip.readUnsignedByte(31 + toLength + fromLength);
		this.message = ip.readString(32 + toLength + fromLength, messageLength);
	}
	
	/**
	 * Set the colour for this message
	 * @param ARGB
	 * @return this MessagePacket (builder pattern)
	 */
	public MessagePacket setARGB(long ARGB) {
		this.aRGB = ARGB;
		return this;
	}
	
	/**
	 * @return ARGB.
	 */
	public long getARGB() {
		return this.aRGB;
	}
	
	/**
	 * Set the MessageType for this message
	 * @param type
	 * @return this MessagePacket (builder pattern)
	 */
	public MessagePacket setMessageType(MessageType type) {
		this.type = type;
		return this;
	}
	
	/**
	 * @return MessageType for this message
	 */
	public MessageType getMessageType() {
		return type;
	}
	
	/**
	 * Set the ChatID for this message
	 * @param chatID
	 * @return this MessagePacket (builder pattern)
	 */
	public MessagePacket setChatID(long chatID) {
		this.chatID = chatID;
		return this;
	}
	
	/**
	 * @return CHATID
	 */
	public long getChatID() {
		return chatID;
	}
	
	/**
	 * @return the sender
	 */
	public String getFrom() {
		return from;
	}
	
	/**
	 * @return the reciepient
	 */
	public String getTo() {
		return to;
	}
	
	/**
	 * @return the message
	 */
	public String getMessage() {
		return message;
	}
	
	/**
	 * @return the total packet size based on all values
	 * and total String length
	 */
	private int getPacketSize() {
		return 32 + from.length() + to.length() + message.length();
	}

	/**
	 * @return A PacketWriter instance based on this MessagePacket
	 */
	public PacketWriter build() {
		return new PacketWriter(PacketType.MESSAGE_PACKET, getPacketSize())
		.putUnsignedInteger(aRGB)
		.putUnsignedInteger(type.type)
		.putUnsignedInteger(chatID)
		.putUnsignedInteger(0) // Receiver avatar.
		.putUnsignedInteger(0) // Sender avatar.
		.putUnsignedByte(4) // always 4, with suffix.
		.putUnsignedByte(from.length())
		.putString(from)
		.putUnsignedByte(to.length())
		.putString(to)
		//  Suffix is now ignored, people were adding "[PM]" & "[GM]"
		// to the end of their names in order to access admin commands
		// which were built into the client, these commands have
		// since been removed, but are still existing in the packets. 
		.putUnsignedByte(0x00)
		.putUnsignedByte(message.length())
		.putString(message);
	}

	/**
	 * An enumeration of various Message Types for the MessagePackets
	 * @author Jan-Willem Gmelig Meyling
	 * @author Thomas Gmelig Meyling
	 */
	public static enum MessageType {
		Talk(2000), Whisper(2001), Action(2002), Team(2003), Guild(2004),
		TopLeft(2005), Clan(2006), System(2007), Yell(2008), Friend(2009),
		Center(2011), Ghost(2013), Service(2014), Tip(2015), World(2021),
		Dialog(2100), LoginInfo(2101), VendorHawk(2104), Website(2105),
		Clear(2108), RightCorner(2109), GuildBulletin(2111), TradeBoard(2201),
		FriendBoard(2202), TeamBoard(2203), GuildBoard(2204), OthersBoard(2205),
		Broadcast(2500);		
		
		final long type;
		
		/**
		 * Constructor for MessageType enum values
		 * @param type
		 */
		private MessageType(long type) {
			this.type = type;
		}
		
		/**
		 * @param value
		 * @return {@code MessageType} for the given value
		 * @throws RuntimeException when there is no such MessageType
		 */
		public static MessageType valueOf(long value) {
			for ( MessageType mt : MessageType.values() )
				if ( mt.type == value )
					return mt;
			throw new RuntimeException("Unimplemented MessageType for " + value);
		}
	}
	
}
