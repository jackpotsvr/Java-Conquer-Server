/**
 * 
 */
package packets;

/**
 * @author Jan-Willem
 *
 */
public class Auth_Login_Forward extends OutgoingPacket {

	/**
	 * 
	 * @param identity
	 * @param token
	 * @param ipAddress
	 * @param port
	 */
	public Auth_Login_Forward(long identity, long token, String ipAddress, int port) {
		super(32, PacketType.auth_login_response);
		this.putUnsignedShort(this.getPacketSize());
		this.putUnsignedShort(this.getPacketType().value);
		this.putUnsignedInteger(identity);
		this.putUnsignedInteger(token);
		this.putString(ipAddress, 16);
		this.putUnsignedInteger(port);
	}

}
