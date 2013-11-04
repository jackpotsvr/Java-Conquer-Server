package packets;

public enum PacketType {
	auth_login_packet	((short) 0x41B),
	auth_login_forward ((short) 0x41F),
	auth_login_response ((short) 0x41C),
	message_packet ((short) 0x3EC),  /* also used for logging in. See https://spirited-fang.wikispaces.com/Logging+Into+Conquer+1.0 */
	char_info_packet ((short) 0x3EE),
	general_data_packet((short) 0x3F2);
	
	public short value;
	
	PacketType(short i) {
		this.value = i;
	}
}