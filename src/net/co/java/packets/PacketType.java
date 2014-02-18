package net.co.java.packets;

/**
 * Enumeration for the Packet types
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 *
 */
public  enum PacketType {
	AUTH_LOGIN_PACKET	(0x041B),
	AUTH_LOGIN_FORWARD	(0x041F),
	AUTH_LOGIN_RESPONSE (0x041C),
	MESSAGE_PACKET		(0x03EC),
	CHAR_INFO_PACKET	(0x03EE),
	ENTITY_MOVE_PACKET   (0x3ED),
	CHARACTER_CREATION_PACKET(0x03E9),
	GENERAL_DATA_PACKET	(0x03F2),
	ENTITY_SPAWN_PACKET (0x03F6),
	ITEM_INFORMATION_PACKET(0x3F0),
	ITEM_USAGE_PACKET (0x3F1);
	
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