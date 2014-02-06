package packets;

public class EntityMovePacket extends IncommingPacket
{
	
	private long entityID;
	private short direction; 
	private boolean running;
	private long timestamp;
	
	
	
	public EntityMovePacket(PacketType packetType, byte[] data)
	{
		super(packetType, data);
		
		entityID = this.readUnsignedInt(4);
		direction = this.readUnsignedByte(8);
		running = this.readUnsignedByte(9) == 1;
		
		System.out.println(entityID + " " + direction%8 + " "  + ((running) ? "true" : "false"));
		 
	}
}
