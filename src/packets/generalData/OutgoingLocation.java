package packets.generalData;

public class OutgoingLocation 
{
	int mapID, xCord, yCord;
	
	OutgoingLocation(OutgoingGeneralData packet)
	{
		mapID = 1002;
		xCord = 382;
		yCord = 341;  
		
		packet.setOffset(12);
		packet.putUnsignedShort(mapID);
		packet.setOffset(16);
		packet.putUnsignedShort(xCord);
		packet.putUnsignedShort(yCord);	
		
	}
}
