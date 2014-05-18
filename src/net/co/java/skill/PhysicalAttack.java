package net.co.java.skill;

import net.co.java.entity.Entity;
import net.co.java.packets.IncomingPacket;
import net.co.java.packets.InteractPacket;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWrapper;
import net.co.java.packets.PacketWriter;
import net.co.java.server.GameServerClient;

public class PhysicalAttack implements PacketWrapper {
	
	private GameServerClient client;  
	private Entity target; 
	private InteractPacket interactPacket;
	
	
	private final int MELEE_RANGE = 2; // for now
	private final int RANGED_RANGE = 7; // for now
	
	// Retreives the Entity object. 
	public PhysicalAttack(GameServerClient player, InteractPacket interactPacket) {
		this.client = player; 
		this.interactPacket = interactPacket;
		for(Entity e : client.getPlayer().getLocation().getMap().getEntities())
			if(e.getIdentity() == interactPacket.getTarget())
				this.target = e; 
	}

	// Computes if target is in range etc
	public boolean validateHit(){
		TargetBuilder tb = new TargetBuilder(client.getPlayer()).inCircle(getRange());
		
		if(target != null)
		{
			for(Entity e : tb.getEntities())
				if(e == target)
					return true;
		}
		
		return false;
	}
	
	public boolean computeMiss(){
		return (Math.random() < 0.90); // 90 pc hit chance for now
	}
	
	public long computeDamage(){
		return 5;
	}
	
	public int getRange(){
		return MELEE_RANGE;
	}
	
	
	@Override
	public PacketWriter build() {
		if(validateHit())
		{
			if(computeMiss()) {
				
				return new PacketWriter(PacketType.INTERACT_PACKET, 28)
					.putUnsignedInteger(interactPacket.getTimer())
					.putUnsignedInteger(client.getIdentity())
					.putUnsignedInteger(target.getIdentity())
					.putUnsignedShort(client.getPlayer().getLocation().getxCord())
					.putUnsignedShort(client.getPlayer().getLocation().getyCord())
					.putUnsignedByte(interactPacket.getMode().mode)
					.setOffset(24)
					.putUnsignedInteger(computeDamage());
			} else {
				return null;
			}
		} 
		return null;
	}


}
