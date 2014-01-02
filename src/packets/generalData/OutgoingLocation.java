package packets.generalData;

import conquerServer.GameServerThread;
import data.Location;

public class OutgoingLocation extends OutgoingGeneralData
{

	OutgoingLocation(GameServerThread client) {
		super(SubType.LOCATION);
		
		Location location = client.getPlayer().getLocation();
		int mapID = location.getMap().getMapID();
		int xCord = location.getxCord();
		int yCord = location.getyCord();

		this.setOffset(12);
		this.putUnsignedShort(mapID);
		this.setOffset(16);
		this.putUnsignedShort(xCord);
		this.putUnsignedShort(yCord);
	}
	
}
