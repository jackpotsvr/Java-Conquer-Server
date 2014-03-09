package net.co.java.packets;

/**
 * Enumeration for the Packet types
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 *
 */
public  enum PacketType {
	
	/**
	 * This packet is sent to the Account Server during login. It contains the
	 * login information such as the Account Name, Password, and Server which
	 * the client wishes to connect to. The Account Server must check this
	 * information with the database.
	 */
	AUTH_LOGIN_PACKET	(0x041B),
	
	/**
	 * The Auth Message Packet (0x41C) is sent by the client during initial
	 * Authentication, it is sent to the game server, the game server uses the
	 * keys to determine which account to link to the client.
	 * 
	 * It is also sent by the auth server when you get your username/password
	 * wrong or the server is down for example, the packet contains the string
	 * of bytes which identify the message to display. (For example: B7 FE CE F1
	 * C6 F7 CE B4 C6 F4 B6 AF)
	 */
	AUTH_LOGIN_RESPONSE (0x041C),
	
	/**
	 * The Authorization Response (0x41F) is made by the Auth Server if the
	 * account is permitted to enter the Game Server.
	 */
	AUTH_LOGIN_FORWARD	(0x041F),
	
	/**
	 * This packet is used to send messages to other players and to instruct the
	 * client on what errors to display on login. The server uses this packet by
	 * routing it to it's destination (the other player).
	 */
	MESSAGE_PACKET		(0x03EC),

	/**
	 * The EntityMove packet, also known as the Walk packet, this packet is used
	 * by all entitys that can move, such as Players, Monsters and if you wish
	 * NPCs. The EntityMove packet, also known as the Walk packet, this packet
	 * is used by all entitys that can move, such as Players, Monsters and if
	 * you wish NPCs. The EntityMove packet, also known as the Walk packet, this
	 * packet is used by all entitys that can move, such as Players, Monsters
	 * and if you wish NPCs.
	 */
	ENTITY_MOVE_PACKET   (0x3ED),
	
	/**
	 * The Character Information packet, this packet is sent primarily during
	 * the login process to set the majority of your characters values.
	 */
	CHAR_INFO_PACKET	(0x03EE),
	
	/**
	 * Sent by the client in response to the NEW_ROLE string when the account
	 * being used to login doesn't have a character associated with it on the
	 * target server. On successfully handling the Character Creation Packet the
	 * server responds with a Chat Packet with the message "ANSWER_OK" sent to
	 * "ALLUSERS" with the type being ChatType.Dialog. The client will then
	 * disconnect and return to the login screen, or in later versions of the
	 * client proceed with the typical login procedure.
	 */
	CHARACTER_CREATION_PACKET(0x03E9),
	
	/**
	 * Send by the server to add and remove items to and from equipment slots,
	 * inventory, inspect- and trade windows
	 */
	ITEM_INFORMATION_PACKET(0x3F0),
	
	/**
	 * The General Data packet performs a variety of tasks for the client, these
	 * vary from moving the client around the game, to ending an XP skill.
	 */
	GENERAL_DATA_PACKET	(0x03F2),
	
	/**
	 * The Entity Spawn packet is used to spawn both Monsters and Characters, it
	 * does not spawn NPCs. The packet's layout changes slightly depending on
	 * which you are spawning.
	 */
	ENTITY_SPAWN_PACKET (0x03F6),
	
	STRING_PACKET(0x3F7),
	
	WEATHER_PACKET(0x3F8),
	
	/**
	 * The Entity Status packet, also known as the "Update packet" is used to
	 * change the appearance(in some cases), certain values unique to a
	 * character (such as level, stat points, exp) and show active abilities (in
	 * other cases). This packet can be used to send 1 status update, or many.
	 */
	UPDATE_PACKET(0x3F9),
	
	/**
	 * The Item Usage packet used for interacting with the warehouse and
	 * equiping/un-equiping/updating items.
	 */
	ITEM_USAGE_PACKET (0x3F1),
	
	/**
	 * The Interact packet is most commonly used for direct melee/archer
	 * attacks, but also used for certain player to player actions, such as
	 * marriage.
	 */
	INTERACT_PACKET(0x3FE),	
	
	/**
	 * The Skill Packet is sent during login and when you learn a new skill.
	 */
	SKILL_PACKET(0x44F),
	
	/**
	 * The Skill Experience packet can be used to update both Skills and
	 * Proficiency.
	 */
	SKILL_UPDATE_PACKET(0x450),
	
	/**
	 * The Magic Attack Packet, used for offensive and defensive spells of all
	 * kind.
	 */
	SKILL_ANIMATION_PACKET(0x451),
	
	GUILD_INFORMATION(0x452),
	
	/**
	 * The Weapon Proficiency Packet is sent during login and when you learn a
	 * new Proficiency.
	 */
	PROFICIENCY(0x401),
	
	
	MAP_STATUS(0x456);//,
	//PORTAL_PACKET (0x24);
	
	private final int type;
	
	PacketType(int type) {
		this.type = type;
	}
	
	public int getType() {
		return type;
	}
	
	/**
	 * @param id
	 * @return the {@code PacketType} for a specific ID
	 * @throws UnimplementedPacketTypeException when the
	 * {@code PacketType} could not be found.
	 */
	public static PacketType valueOf(int id) throws UnimplementedPacketTypeException {
		for ( PacketType pt : PacketType.values() ) {
			if ( pt.type == id )
				return pt;
		}
		throw new UnimplementedPacketTypeException(id);
	}
	
	public static class UnimplementedPacketTypeException extends Exception {
		
		private static final long serialVersionUID = 1L;

		UnimplementedPacketTypeException(int type) {
			super("Unimplemented PacketType: " + type);
		}
		
	}
}