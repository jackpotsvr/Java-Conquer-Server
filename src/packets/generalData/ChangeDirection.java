package packets.generalData;

import packets.IncommingPacket;
import conquerServer.GameServerThread;

public class ChangeDirection 
{
	public static IncommingPacket in(IncommingGeneralData request, final GameServerThread client) {
		return new IncommingGeneralData(request) {{
			this.getShorts();
		}};
	}
}
