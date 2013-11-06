/**
 * 
 */
package packets;

import conquerServer.Cryptography;

/**
 * @author Jan-Willem
 *
 */
public class Auth_Login_Response extends OutgoingPacket {

	/**
	 * @param packetSize
	 * @param type
	 */
	public Auth_Login_Response(int identity, int token, String ipAddress, int port, Cryptography cipher) {
		super((short) 32, PacketType.auth_login_response, cipher);
		this.put(this.getPacketSize());
		this.put(this.getType().value);
		this.put(identity);
		this.put(token);
		this.put(ipAddress, 16);
		this.put(port);
	}

}
