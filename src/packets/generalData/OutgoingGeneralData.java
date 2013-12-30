package packets.generalData;

import java.io.IOException;

import conquerServer.GameServerThread;
import packets.PacketType;
import packets.OutgoingPacket;

public class OutgoingGeneralData extends OutgoingPacket
{
	private long timestamp;
	private long identity;
	private int datafields[] = new int[5];
	SubType subType; 
	
	
	OutgoingGeneralData(PacketType packetType, byte[] data, GameServerThread thread) throws IOException
	{
		super(PacketType.GENERAL_DATA_PACKET, new byte[28]);
		
		
		timestamp = 0; 
		
		identity = 23L; 
		
		datafields[0] = 1002;
		datafields[1] = 382;
		datafields[2] = 341;  //HAHAHA
		
		subType = SubType.get(74l); 
		
		this.putUnsignedInteger(timestamp);
		this.putUnsignedInteger(identity);
		
		this.putUnsignedShort(datafields[0]);
		this.setOffset(16);
		this.putUnsignedShort(datafields[1]);
		this.putUnsignedShort(datafields[2]);
		
		
		this.setOffset(22);
		this.putUnsignedShort( (int) subType.getType());
		
		thread.send(this.getData());

	}
}
