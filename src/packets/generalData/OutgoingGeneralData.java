package packets.generalData;

import java.io.IOException;

import conquerServer.GameServerThread;
import packets.PacketType;
import packets.OutgoingPacket;
import packets.generalData.*;

public class OutgoingGeneralData extends OutgoingPacket
{
	private long timestamp;
	private long identity;
	private int datafields[] = new int[5];
	SubType subType; 
	
	
	OutgoingGeneralData(PacketType packetType, byte[] data, GameServerThread thread) throws IOException
	{
		super(PacketType.GENERAL_DATA_PACKET, new byte[28]);
		
		timestamp = (int) System.currentTimeMillis() & 0xFFFFFFFF; 
		identity = 23L; 
		subType = SubType.get(74l); 
		
		this.putUnsignedInteger(timestamp);
		this.putUnsignedInteger(identity);
		this.setOffset(22);
		this.putUnsignedShort( (int) subType.getType());
		
		
		route();
		thread.send(this.getData());
		
	}
	
	private void route()
	{
		switch(subType)
		{
			case LOCATION:
				new OutgoingLocation(this);
				break;
			default:
				break;
				
		}
	}
	
}
