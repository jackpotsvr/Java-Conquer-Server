package net.co.java.skill;

import net.co.java.entity.Entity;
import net.co.java.entity.Player.Inventory;
import net.co.java.item.ItemInstance.EquipmentInstance;
import net.co.java.packets.IncomingPacket;
import net.co.java.packets.InteractPacket;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWrapper;
import net.co.java.packets.PacketWriter;
import net.co.java.server.GameServerClient;
import net.co.java.skill.Skill.PassiveSkill;

public class PhysicalAttack implements PacketWrapper {
	
	private GameServerClient client;  
	private Entity target; 
	private InteractPacket interactPacket;
	
	
	private final static int MELEE_RANGE = 2; // for now
	private final static int RANGED_RANGE = 13; // for now
	
	// Retreives the Entity object. 
	public PhysicalAttack(GameServerClient player, InteractPacket interactPacket) {
		this.client = player; 
		this.interactPacket = interactPacket;
		for(Entity e : client.getPlayer().getLocation().getMap().getEntities())
			if(e.getIdentity() == interactPacket.getTarget())
				this.target = e; 
	}

	// Computes if target is in range etc
	// TODO CHECK FOR TIME 
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
	
	public boolean doPassive(){
		//client.getPlayer().inventory.getEquipments()
		
		WeaponType wt1 = WeaponType.valueOf((int) (client.getPlayer().inventory.getEquipmentSID(Inventory.RIGHT_HAND) / 1000));
		WeaponType wt2 = WeaponType.valueOf((int) (client.getPlayer().inventory.getEquipmentSID(Inventory.LEFT_HAND) / 1000)); 
		
		PassiveSkill skill1 = Skill.passiveSkillOf(wt1);
		PassiveSkill skill2 = Skill.passiveSkillOf(wt2);
				
		// we shouldn't give the player twice as much chance.
		if(skill1 == skill2 && skill1 != null)
		{
			if(client.getPlayer().getSkills().containsKey(skill1))
			{
				if(Math.random() < skill1.chance(9))
				{
					passiveSkillHandle(skill1); 
					return true;
				}
			}
		} else if (skill1 != null && skill2 != null){
			if(client.getPlayer().getSkills().containsKey(skill1))
			{
				if(Math.random() < skill1.chance(9))
				{
					passiveSkillHandle(skill1); 
					return true;
				}
			}
			if(client.getPlayer().getSkills().containsKey(skill2))
			{
				if(Math.random() < skill2.chance(9))
				{
					passiveSkillHandle(skill2); 
					return true;
				}
			}
		} else if (skill1 != null) {
			if(client.getPlayer().getSkills().containsKey(skill1))
			{
				if(Math.random() < skill1.chance(9))
				{
					passiveSkillHandle(skill1); 
					return true;
				}
			}
		} else if (skill2 != null) {
			if(client.getPlayer().getSkills().containsKey(skill2))
			{
				if(Math.random() < skill2.chance(9))
				{
					passiveSkillHandle(skill2); 
					return true;
				}
			}
		}
		
		
		return false;
	}
	
	public long computeDamage(){
		return 4000;
	}
	
	public int getRange(){
		return MELEE_RANGE;
	}
	
	
	@Override
	public PacketWriter build() {
		if(validateHit())
		{
			if(doPassive()) {
				return null;				
			} else { 
				if(computeMiss()) {
					long damage = computeDamage();
					target.setHP((int) (target.getHP()-damage));
					target.spawn();
					
					return new PacketWriter(PacketType.INTERACT_PACKET, 28)
						.putUnsignedInteger(interactPacket.getTimer())
						.putUnsignedInteger(client.getIdentity())
						.putUnsignedInteger(target.getIdentity())
						.putUnsignedShort(client.getPlayer().getLocation().getxCord())
						.putUnsignedShort(client.getPlayer().getLocation().getyCord())
						.putUnsignedByte(interactPacket.getMode().mode)
						.setOffset(24)
						.putUnsignedInteger(computeDamage());
				} 
				else {
					return null;
				}
			}
		} 
		return null;
	}
	
	public void passiveSkillHandle(PassiveSkill skill){
		
		skill.setTarget(target);
		
		TargetBuilder tb = skill.getHittedEntities(client, client.getPlayer().getSkillLevel(skill));
		long damage = (long) (computeDamage()*skill.damageMutiplier(client.getPlayer().getSkillLevel(skill)));
		
		target.setHP((int) (target.getHP()-damage));
	
		PacketWriter pw = new PacketWriter(PacketType.SKILL_ANIMATION_PACKET, 20 + tb.size() * 8)
			.putUnsignedInteger(client.getIdentity())
				.putUnsignedShort(client.getPlayer().getLocation().xCord)
			.putUnsignedShort(client.getPlayer().getLocation().yCord)
			.putUnsignedShort(skill.getSkillID())
			.putUnsignedShort(client.getPlayer().getSkillLevel(skill))
			.putUnsignedShort(tb.size())
			.setOffset(20);
		
		for( Entity e : tb )
			pw.putUnsignedInteger(e.getIdentity()).putUnsignedInteger(damage);
		
		pw.sendTo(client.getPlayer().view.getPlayers());
	}
	
	public final static class ArcherAttack extends PhysicalAttack {

		public ArcherAttack(GameServerClient player, InteractPacket interactPacket) {
			super(player, interactPacket);
		
		}
		
		@Override
		public long computeDamage(){
			return 1000;
		}
		
		@Override
		public boolean computeMiss(){
			return (Math.random() < 0.95); 
		}
		
		public int getRange(){
			return RANGED_RANGE;
		}
		
	}



}
