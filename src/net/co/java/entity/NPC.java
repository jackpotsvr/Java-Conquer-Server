package net.co.java.entity;

import net.co.java.packets.NPC_Spawn_Packet;
import net.co.java.packets.PacketWriter;

public class NPC extends Entity implements Spawnable {
	
	//private Location location;
	//private String name; 
	//private long uniqueID;
	private int type;
	private int interaction; // TODO MAKE ENUMS?
	private long flags; // TODO MAKE ENUMS?
	private int face; 

	
	public NPC(long uniqueID, String name, Location location, int model, int interaction, int flags, int direction)
	{
		super(uniqueID, 0, 0, name, location, 0);
		setType(type, direction);
		this.interaction = interaction;
		this.flags = flags;
		this.face = 30;
	}
	
	@Override
	public boolean inView(Spawnable spawnable) {
		return location.inView(spawnable.getLocation());
	}
	
	/**
	 * @return  Returns the type (without direction) 
	 * If you want the type with direction use getTypeD() 
	 */
	public int getType() {
		return (type - (type %10));
	}
	
	/**
	 * @return returns the type with direction included.
	 */
	public int getTypeD() {
		return type; 
	}
	
	public void setType(int type, int direction){
		type = (type - (type%10)) + direction;
	}
	
	public void setDirection(int direction){
		type = getType() + direction; 
	}
	
	
	
	public int getInteraction() {
		return interaction;
	}

	public void setInteraction(int interaction) {
		this.interaction = interaction;
	}

	@Override 
	public long getFlags() {
		return flags;
	}
	
	public void setFlags(long flags) {
		this.flags = flags;
	}

	@Override
	public PacketWriter SpawnPacket() {
		return new NPC_Spawn_Packet(this).build();
	}


	@Override
	public int getMaxHP() {
		// TODO Auto-generated method stub
		return 0;
	}


	@Override
	public int getMaxMana() {
		// TODO Auto-generated method stub
		return 0;
	}

	@Override
	public void notify(PacketWriter writer) {
		// Do nothing
	}

	public int getFace() {
		return face;
	}
	
	public static enum Flag {
		NONE 			( 0x0000  ),
		TASK			( 0x0001  ),
		RECYCLE 		( 0x0002  ),
		SCENE 			( 0x0004  ),
		LINKMAP 		( 0x0008  ),
		DIEACTION 		( 0x0010  ),
		ENABLEDELETE	( 0x0020  ),
		EVENT 			( 0x0040  ),
		TABLE 			( 0x0080  );
		
		public final long value;
		
		private Flag(long value) { this.value = value; }
	}

}
