package packets;

public enum PacketType {
	auth_login_packet	(0x41B),
	auth_login_forward	(0x41F),
	auth_login_response (0x41C),
	message_packet		(0x3EC),  /* also used for logging in. See https://spirited-fang.wikispaces.com/Logging+Into+Conquer+1.0 */
	char_info_packet	(0x3EE),
	general_data_packet	(0x3F2);
	
	public int value;
	
	PacketType(int i) {
		this.value = i;
	}
	
	public static PacketType get(int i) {
		for ( PacketType pt : PacketType.values() )
			if ( pt.value == i )
				return pt;
		return null;
	}
}