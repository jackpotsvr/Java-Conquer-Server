package packets.generalData;

import packets.PacketType;
import packets.OutgoingPacket;

public class OutgoingGeneralData extends OutgoingPacket
{
	private long timestamp;
	private long identity;
	private int datafields[] = new int[5];
	private final SubType subType; 
	
	
	OutgoingGeneralData(SubType subType)
	{
		super(PacketType.GENERAL_DATA_PACKET, new byte[28]);
		this.subType = subType;
		
		timestamp = System.currentTimeMillis(); 
		identity = 1000000L;
		
		this.putUnsignedInteger(timestamp);
		this.putUnsignedInteger(identity);
		this.setOffset(22);
		this.putUnsignedShort(subType.getType());		
	}	
}
