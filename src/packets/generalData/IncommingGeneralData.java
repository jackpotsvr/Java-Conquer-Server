package packets.generalData;

import java.io.IOException;

import conquerServer.GameServerThread;
import data.Entity;
import data.Map;
import data.Player;
import packets.IncommingPacket;
import packets.OutgoingPacket;
import packets.PacketType;
import packets.generalData.SubType;

public class IncommingGeneralData extends IncommingPacket  
{
	private long timestamp;
	private long identity;
	private int datafields[] = new int[3];
	SubType subType; 
	
	public IncommingGeneralData(PacketType packetType, byte[] data, GameServerThread thread)
	{
		super(packetType, data);
		timestamp = this.readUnsignedInt(4);
		identity = this.readUnsignedInt(8);
		datafields[0] = this.readUnsignedShort(12);
		datafields[1] = this.readUnsignedShort(16);
		datafields[1] = this.readUnsignedShort(18);
		subType = SubType.get(this.readUnsignedShort(22));
		
		route(packetType, data, thread);
	}
	
	
	/**
	 * Used to route packet to handle different subtypes ;) 
	 * @param packetType
	 * @param data
	 * @param thread 
	 * @throws IOException 
	 */
	private void route(PacketType packetType, byte[] data, GameServerThread client)
	{
		switch(subType)
		{
			case LOCATION:
				new OutgoingLocation(client).send(client);
				break;
			case GET_SURROUNDINGS:
				System.out.println("Get surroundings!");
				Player player = client.getPlayer();
				Map map = player.getLocation().getMap();
				for ( Entity e : map.getEntitiesInRange(player) ) {
					e.spawn().send(client);
				}
				break;
			default:
				System.out.printf("Fix subtype %s", this.readUnsignedShort(22));
		}
	}
	
}
