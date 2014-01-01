package packets.generalData;

public class OutgoingLocation extends OutgoingGeneralData
{
	int mapID, xCord, yCord;

	OutgoingLocation() {
		super(SubType.LOCATION);

		mapID = 1002;
		xCord = 382;
		yCord = 341;

		this.setOffset(12);
		this.putUnsignedShort(mapID);
		this.setOffset(16);
		this.putUnsignedShort(xCord);
		this.putUnsignedShort(yCord);
	}
	
}
