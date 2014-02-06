package packets.generalData;

import packets.IncommingPacket;
import packets.OutgoingPacket;
import conquerServer.GameServerThread;
import data.Location;

public class JumpPacket 
{   /*
	public static IncommingPacket in()
	{
		return new IncommingGeneralData() 
		{{
			this.getShorts();
		}};
	} */
	
	public static OutgoingPacket out(final IncommingGeneralData data, final GameServerThread client)
	{
			return new OutgoingGeneralData(SubType.JUMP, client) 
			{{
				int [] location = data.getShorts();
				client.getPlayer().getLocation().setX(location[0]);
				client.getPlayer().getLocation().setY(location[2]);
				
			
		
				
				
				this.setOffset(12);
				this.putUnsignedShort(location[0]);
				this.setOffset(16);
				this.putUnsignedShort(location[1]);
				this.putUnsignedShort(location[2]);
				
				
				
			}};
	}
}
