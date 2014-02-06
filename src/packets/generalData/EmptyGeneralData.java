package packets.generalData;

import packets.IncommingPacket;
import conquerServer.GameServerThread;

public class EmptyGeneralData extends OutgoingGeneralData
{
	EmptyGeneralData(IncommingGeneralData ip, GameServerThread client) {
		super(ip.getSubType(), client);
		
		System.out.println(ip.getSubType() + " PACKET SUBTYPE " + ip.getIntSubType());
		
		this.setOffset(12);
		this.putUnsignedShort(0);
		this.setOffset(16);
		this.putUnsignedShort(0);
		this.putUnsignedShort(0);
		
		
	}
}
