package packets;

public enum PacketType
{
	AUTH_LOGIN_PACKET	(0x041B, 0x34),
	AUTH_LOGIN_FORWARD	(0x041F, 0x20),
	AUTH_LOGIN_RESPONSE (0x041C, 0x1C),
	MESSAGE_PACKET		(0x03EC, 0x00),
	CHAR_INFO_PACKET	(0x03EE, 0x00),
	CHARACTER_CREATION_PACKET(0x03E9, 0x3C),
	GENERAL_DATA_PACKET	(0x03F2, 0x18),
	ENTITY_SPAWN_PACKET (0x03F6, 0),
	ITEM_USAGE_PACKET (0x3F1, 28);
	
	private final int type, size;
	
	private PacketType(int type, int size)
	{
		this.type = type;
		this.size = size;
	}
	
	/**
	 * @return Packet type
	 */
	public int getType() {
		return type;
	}
	
	/**
	 * @return packet size
	 */
	public int getSize() {
		return size;
	}
	
	public static PacketType get(int type)
	{
		for ( PacketType pt : PacketType.values() ) 
		{
			if ( pt.type == type )
			{
				return pt;
			}
		}
		throw new IllegalArgumentException("No packettype implemented for " + type);
	}
	
}
