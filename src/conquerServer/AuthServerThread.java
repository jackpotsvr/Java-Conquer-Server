/**
 * 
 */
package conquerServer;

import java.io.IOException;
import java.net.Socket;

import packets.*;

/**
 * 
 * @author Jan-Willem
 *
 */
public class AuthServerThread extends ServerThread
{
	
	private AuthServer authServer = null;
	
	/**
	 * 
	 * @param client
	 * @param authServer
	 * @throws IOException 
	 */
	public AuthServerThread(Socket client, AuthServer authServer) throws IOException
	{
		super(client);
		this.authServer = authServer;
	}

	@Override
	protected void route(PacketType packetType, byte[] data) {		
		switch(packetType){
		case AUTH_LOGIN_PACKET:
			new Auth_Login_Packet(packetType, data, this);
			break;
		case AUTH_LOGIN_RESPONSE:
			new Auth_Login_Response(packetType, data, this);
			break; 
		default:
			break;
		}
	}
	
}
