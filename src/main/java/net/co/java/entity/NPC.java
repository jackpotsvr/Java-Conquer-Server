package net.co.java.entity;

import net.co.java.packets.PacketWriter;

public class NPC extends Entity implements Spawnable {
	
	private static final long serialVersionUID = 7294339394890365570L;
	
	private int type;
	private NPC_Flag flag;
	private Interaction interaction; 
	private int face; 
	

	
	public NPC(long uniqueID, String name, Location location, int type, Interaction interaction, NPC_Flag flag)
	{
		super(uniqueID, 0, 0, name, location, 0);
		setType(type, location.getDirection());
		this.flag = flag; 
		this.interaction = interaction;
		this.face = 1; // should be loaded from the db as well.. 
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
		this.type = (type - (type%10)) + direction;
	}
	
	public void setDirection(int direction){
		this.type = getType() + direction; 
	}
	
	public Interaction getInteraction() {
		return interaction;
	}

	public void setInteraction(Interaction interaction) {
		this.interaction = interaction;
	} 

	@Override
	public int getMaxHP() {
		// TODO Auto-generated method stub
		return 0;
	}
	
	public NPC_Flag getNPC_Flag(){
		return flag; 
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
	
	public static enum NPC_Flag {
		NONE 			( 0x0000  ),
		TASK			( 0x0001  ),
		RECYCLE 		( 0x0002  ),
		SCENE 			( 0x0004  ),
		LINKMAP 		( 0x0008  ),
		DIEACTION 		( 0x0010  ),
		ENABLEDELETE	( 0x0020  ),
		EVENT 			( 0x0040  ),
		TABLE 			( 0x0080  );
		
		public final int value;
		
		private NPC_Flag(int value) { this.value = value; }
		
		public static NPC_Flag valueOf(int value)
		{
			for ( NPC_Flag fl : NPC_Flag.values() ) {
				if ( fl.value == value )
					return fl;
			}
			return null;
		}
	}
	
	public static enum Interaction {
		SHOP 			(1),
		DIALOG 			(2),
		WAREHOUSE 		(3),
		SCENE 			(4),
		AVATAR 			(5),
		METEORUPGRADE 	(6),
		GEMSOCKET		(7),
		STATUARY 		(9),
		POLE 			(10),
		MARKETSHOP 		(14),
		MARKETSTALL 	(16),
		GAMBLE 			(19),
		FURNITURE 		(26),
		DIALOG2 		(29);
		
		public final int value;
		
		private Interaction(int value) { this.value = value; }
			
		public static Interaction valueOf(int value)
		{
			for ( Interaction in : Interaction.values() ) {
				if ( in.value == value )
					return in;
			}
			return null;
		}
	}
	
}
