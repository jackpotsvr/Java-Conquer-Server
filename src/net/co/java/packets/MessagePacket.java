package net.co.java.packets;

import net.co.java.entity.Player;
import net.co.java.server.GameServerClient;


/**
 * This packet is used to send messages to other players and to instruct
 * the client on what errors to display on login. The server uses this
 * packet by routing it to it's destination (the other player).
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public class MessagePacket implements PacketHandler {

	public final static String SYSTEM = "SYSTEM";
	public final static String ALL_USERS = "ALLUSERS";
	
	private long aRGB = 0xFFFFFFFFL;
	private MessageType type = MessageType.TALK;
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
		int toLength = ip.readUnsignedByte(26 + fromLength);
		this.to = ip.readString(27 + fromLength, toLength);
		int messageLength = ip.readUnsignedByte(28 + toLength + fromLength);
		this.message = ip.readString(29 + toLength + fromLength, messageLength);
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
	
	public MessagePacket setType(MessageType mt) {
		this.type = mt;
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

	@Override
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
		TALK(2000), WHISPER(2001), ACTION(2002), TEAM(2003), GUILD(2004),
		TOPLEFT(2005), CLAN(2006), SYSTEM(2007), YELL(2008), FRIEND(2009),
		CENTER(2011), GHOST(2013), SERVICE(2014), TIP(2015), WORLD(2021),
		DIALOG(2100), LOGININFO(2101), VENDORHAWK(2104), WEBSITE(2105),
		CLEAR(2108), RIGHTCORNER(2109), GUILDBULLETIN(2111), TRADEBOARD(2201),
		FRIENDBOARD(2202), TEAMBOARD(2203), GUILDBOARD(2204), OTHERSBOARD(2205),
		BROADCAST(2500);		
		
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

	@Override
	public void handle(GameServerClient client) {
		switch(type)
		{
			case TALK:
			{
				for(Player p : client.getPlayer().view.getPlayers())
				{
					if(client.getPlayer() == p)
						continue; 
					this.build().send(p.getClient());
				}
				break;
			}
			default:
			{
				break; // TODO
			}
		}
		
	}
	
}
