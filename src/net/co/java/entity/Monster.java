package net.co.java.entity;

import net.co.java.packets.PacketWriter;

public class Monster extends Entity {

	private static final long serialVersionUID = -3706911983201300436L;
	
	private Player target = null;

	public Monster(Location location, long identity, String name, int level, int mesh, int HP) {
		super(identity, mesh, 0, name, location, HP);
		this.level = level;
		setLocation(location);
		System.out.println("Constructed monster at " + location);
	}

	@Override
	public int getMaxHP() {
		// TODO Auto-generated method stub
		return getHP();
	}

	@Override
	public int getMaxMana() {
		// TODO Auto-generated method stub
		return getMana();
	}

	@Override
	public void notify(PacketWriter writer) {
		// Maybe hook this to the AI, or just fetch it from memory anyway
	}

	public Player getTarget() {
		return target;
	}

	public void setTarget(Player target) {
		this.target = target;
	}
	
	
	
}
