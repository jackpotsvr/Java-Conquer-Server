package conquerServer;

import java.io.IOException;
import java.net.Socket;

import data.Player;
import packets.*;
import packets.generalData.IncommingGeneralData;

public class GameServerThread extends ServerThread
{
	
	private final GameServer gameServer;
	private Player player;
	
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
	
	public GameServer getGameServer() {
		return gameServer;
	}
	
	public void setPlayer(Player player) {
		this.player = player;
	}
	
	public Player getPlayer() {
		return this.player;
	}

	@Override
	protected void route(PacketType packetType, byte[] data) {
		switch(packetType){
		case AUTH_LOGIN_RESPONSE:
			new Auth_Login_Response(packetType, data, this);
			break;
		case GENERAL_DATA_PACKET:
			new IncommingGeneralData(data, this);
			break;
		case CHARACTER_CREATION_PACKET:
			new Character_Creation_Packet(packetType, data, this);
			break;
		case ITEM_USAGE_PACKET:
			ItemUsagePacket.in(data, this);
			break;
		//TO BE DONE - case CHARACTER_CREATION_PACEKT: return new Character_Creation_Packet(packetType, data, thread); 
		default:
			System.out.println("Unimplemented packet " + packetType.toString());
			break;
		}	
	}


}
