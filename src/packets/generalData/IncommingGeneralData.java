package packets.generalData;

import java.io.IOException;

import conquerServer.GameServerThread;
import packets.IncommingPacket;
import packets.PacketType;
import packets.generalData.SubType;

public class IncommingGeneralData extends IncommingPacket  
{
	private long timestamp;
	private long identity;
	private int datafields[] = new int[5];
	SubType subType; 
	
	public IncommingGeneralData(PacketType packetType, byte[] data, GameServerThread thread)
	{
		super(packetType, data);
		timestamp = this.readUnsignedInt(4);
		identity = this.readUnsignedInt(8);
		datafields[0] = this.readUnsignedShort(12);
		datafields[1] = this.readUnsignedShort(14);
		datafields[2] = this.readUnsignedShort(16);
		datafields[3] =  this.readUnsignedShort(18);
		datafields[4] = this.readUnsignedShort(20);
		subType = SubType.get(this.readUnsignedShort(22));
		
		switch(subType)
		{
			case NO_VALUES_ATM:
				System.out.println("Info");
			default:
				System.out.printf("%s, \t %s, \t %s, \t %s, \t %s, \t %s, \t %s", timestamp, identity, datafields[0], datafields[1],
								 datafields[2], datafields[3], subType.getType());
		
		}
		
		// route subtype ;) 
	}
	
}

/**
public Auth_Login_Packet(PacketType packetType, byte[] data, ServerThread thread) throws IOException {
	super(packetType, data);
	accoutName	= this.readString(4,16);
	password	= Cryptographer.decryptPassword(data, 20);
	serverName	= this.readString(36, 16);
	
	Auth_Login_Forward ALF = new Auth_Login_Forward(thread);
	thread.send(ALF.data);
} */