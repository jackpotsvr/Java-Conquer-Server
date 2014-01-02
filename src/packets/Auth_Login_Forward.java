/**
 * 
 */
package packets;

import conquerServer.ServerThread;

public class Auth_Login_Forward extends OutgoingPacket
{

	public Auth_Login_Forward(ServerThread thread)
	{
		super(PacketType.AUTH_LOGIN_FORWARD);
		this.putUnsignedInteger(1000000L); 
		this.putUnsignedInteger(5);
		this.putString("127.000.000.001", 16);
		this.putUnsignedInteger(5816);
	}

}
