package packets.generalData;

import packets.IncommingPacket;
import conquerServer.GameServerThread;

public class ChangeDirection 
{
	public static IncommingPacket in(byte[] data, final GameServerThread client) {
		return new IncommingGeneralData(data, client) {{
			this.getShorts();
		}};
	}
}
