package conquerServer;

import java.io.IOException;
import java.net.Socket;

import packets.*;
import packets.generalData.IncommingGeneralData;

public class GameServerThread extends ServerThread
{
	
	private final GameServer gameServer;
	
	/**
	 * 
	 * @param client
	 * @param authServer
	 * @throws IOException 
	 */
	public GameServerThread(Socket client, GameServer gameServer) throws IOException
	{
		super(client);
		this.gameServer = gameServer;
	}

	@Override
	protected void route(PacketType packetType, byte[] data) {
		switch(packetType){
		case AUTH_LOGIN_RESPONSE:
			new Auth_Login_Response(packetType, data, this);
			break;
		case GENERAL_DATA_PACKET:
			new IncommingGeneralData(packetType, data, this);
			break;
		case CHARACTER_CREATION_PACKET:
			new Character_Creation_Packet(packetType, data, this);
			break;
		//TO BE DONE - case CHARACTER_CREATION_PACEKT: return new Character_Creation_Packet(packetType, data, thread); 
		default:
			System.out.println("Unimplemented packet " + packetType.toString());
			break;
		}	
	}


}
