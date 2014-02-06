package packets.generalData;

import conquerServer.GameServerThread;
import packets.PacketType;
import packets.OutgoingPacket;

public class OutgoingGeneralData extends OutgoingPacket
{
	private long timestamp;
	private long identity;
	private int datafields[] = new int[5];
	private final SubType subType; 
	
	
	OutgoingGeneralData(SubType subType,  GameServerThread client)
	{
		super(PacketType.GENERAL_DATA_PACKET, new byte[28]);
		this.subType = subType;
		
		timestamp = System.currentTimeMillis(); 
		identity = client.getIdentity();
	
		
		this.putUnsignedInteger(timestamp);
		this.putUnsignedInteger(identity);
		this.setOffset(22);
		this.putUnsignedShort(subType.getType());		
	}	
}
