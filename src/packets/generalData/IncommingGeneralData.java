package packets.generalData;

import conquerServer.GameServerThread;
import data.Entity;
import data.Map;
import data.Player;
import packets.IncommingPacket;
import packets.PacketType;
import packets.generalData.SubType;

public class IncommingGeneralData extends IncommingPacket  
{
	
	public IncommingGeneralData(byte[] data, GameServerThread client)
	{
		super(PacketType.GENERAL_DATA_PACKET, data);
		
		switch(getSubType())
		{
			case LOCATION:
				new OutgoingLocation(client).send(client);  // Doesn't need parameters ;) 
				break;
			case CHANGE_DIRECTION:
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
	
	public long getTimestamp() {
		return  this.readUnsignedInt(4);
	}
	
	public long getIdentity() {
		return this.readUnsignedInt(8);
	}
	
	public SubType getSubType() {
		return SubType.get(this.readUnsignedShort(22));
	}
	
	public int getShort() {
		return this.readUnsignedShort(18);
	}
	
	public long getInteger() {
		return this.readUnsignedInt(12);
	}
	
	public int[] getShorts() {
		return new int[] {
			this.readUnsignedShort(12),
			this.readUnsignedShort(16),
			this.readUnsignedShort(18)
		};
	}
	
}
