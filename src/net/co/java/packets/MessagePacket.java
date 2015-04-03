package net.co.java.packets;

import net.co.java.entity.Player;
import net.co.java.guild.GuildMember;
import net.co.java.packets.packethandlers.GeneralDataPacketHandler;
import net.co.java.packets.packethandlers.MessagePacketHandler;
import net.co.java.packets.serialization.*;
import net.co.java.server.GameServerClient;


/**
 * This packet is used to send messages to other players and to instruct
 * the client on what errors to display on login. The server uses this
 * packet by routing it to it's destination (the other player).
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */

@Type(type = PacketType.MESSAGE_PACKET)
@Bidirectional(handler = MessagePacketHandler.class)
@PacketLength(length = 32)
public class MessagePacket extends Packet /*implements PacketHandler*/ {

	public final static String SYSTEM = "SYSTEM";
	public final static String ALL_USERS = "ALLUSERS";

    @PacketValue(type = PacketValueType.UNSIGNED_INT)
    @Offset(4)
	private long aRGB = 0xFFFFFFFFL;

    @PacketValue(type = PacketValueType.ENUM_VALUE)
    @Offset(8)
   	private MessageType type = MessageType.TALK;

    @PacketValue(type = PacketValueType.UNSIGNED_INT)
    @Offset(12)
	private long chatID = 0L;

    @PacketValue(type = PacketValueType.UNSIGNED_INT)
    @Offset(16)
	private long send_avatar = 0;

    @PacketValue(type = PacketValueType.UNSIGNED_INT)
    @Offset(20)
	private long recv_avatar = 0;

    @PacketValue(type = PacketValueType.UNSIGNED_BYTE)
    @Offset(24)
    private short stringCount = 4; /** Always 4 with suffix  */

    @PacketValue(type = PacketValueType.STRING_WITH_LENGTH)
    @Offset(25)
	private String from;

    @PacketValue(type = PacketValueType.STRING_WITH_LENGTH)
    @Offset(26)
	private String to;

    @PacketValue(type = PacketValueType.BYTE)
    @Offset(27)
    private short suffixLength = 0; // always an empty string.

    @PacketValue(type = PacketValueType.STRING_WITH_LENGTH)
    @Offset(28)
	private String message;
	
	/**
	 * Construct a new Message packet with the from, to and message
	 * values already set, and the aRGB, type and chatID set to default
	 * values
	 * @param from
	 * @param to
	 * @param message
	 */
	public MessagePacket(String from, String to, String message) {
        super(null);
        this.setType(PacketType.MESSAGE_PACKET);
		this.from = from;
		this.to = to;
		this.message = message;
	}
	
	/**
	 * Construct a new Message packet based on a IncomingPacket
	 * @param ip
	 */
	public MessagePacket(IncomingPacket ip) {
        super(ip);
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
     * Set the MessageType for this message
     * @param mt
     * @return this MessagePacket (builder pattern)
     */
	public MessagePacket setMessageType(MessageType mt) {
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
		
		final int type;
		
		/**
		 * Constructor for MessageType enum values
		 * @param type
		 */
		private MessageType(int type) {
			this.type = type;
		}
		
		/**
		 * @param value
		 * @return {@code MessageType} for the given value
		 * @throws RuntimeException when there is no such MessageType
		 */
		public static MessageType valueOf(int value) {
			for ( MessageType mt : MessageType.values() )
				if ( mt.type == value )
					return mt;
			throw new RuntimeException("Unimplemented MessageType for " + value);
		}
	}
}
