package net.co.java.entity;

import net.co.java.packets.NPC_Spawn_Packet;
import net.co.java.packets.PacketWriter;

public class NPC extends Entity implements Spawnable {
	
	//private Location location;
	//private String name; 
	//private long uniqueID;
	private int model;
	private int interaction; // TODO MAKE ENUMS?
	private int flags; // TODO MAKE ENUMS?
	private int direction;
	
	public NPC(long uniqueID, String name, Location location, int model, int interaction, int flags, int direction)
	{
		super(uniqueID, 0, 0, name, location, 0);
		this.model = model;
		this.interaction = interaction;
		this.flags = flags;
		this.direction = direction;
	}
	
	@Override
	public boolean inView(Spawnable spawnable) {
		return location.inView(spawnable.getLocation());
	}
	
	public int getDirection() {
		return direction;
	}

	public void setDirection(int direction) {
		this.direction = direction;
	}

	public int getModel() {
		return model;
	}

	public void setModel(int model) {
		this.model = model;
	}

	public int getNpc_interactions() {
		return interaction;
	}

	public void setNpc_interactions(int npc_interactions) {
		this.interaction = npc_interactions;
	}

	public int getNpc_flags() {
		return flags;
	}

	public void setNpc_flags(int npc_flags) {
		this.flags = npc_flags;
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
}
