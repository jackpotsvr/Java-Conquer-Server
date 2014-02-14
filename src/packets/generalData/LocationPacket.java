package packets.generalData;

import conquerServer.GameServerThread;
import data.Location;
import packets.IncommingPacket;
import packets.OutgoingPacket;

public class LocationPacket {
	
	public static IncommingPacket in(byte[] data, final GameServerThread client) {
		return new IncommingGeneralData(data, client) {{
			this.getShorts();
		}};
	}
	
	public static OutgoingPacket out(byte[] data, final GameServerThread client) {
		return new OutgoingGeneralData(SubType.LOCATION, client) {{
			Location location = client.getPlayer().getLocation();
			int mapID = location.getMap().getMapID();
			int xCord = location.getxCord();
			int yCord = location.getyCord();

			this.setOffset(12);
			this.putUnsignedShort(mapID);
			this.setOffset(16);
			this.putUnsignedShort(xCord);
			this.putUnsignedShort(yCord);
			
			
		}};
	}
	
}
