package net.co.java.skill;

import net.co.java.entity.Player;
import net.co.java.packets.InteractPacket;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.server.Server.GameServer.Client;
import net.co.java.skill.Skill.MagicSkill;

class FastBlade extends MagicSkill {
	
	@Override
	public int getSkillID() {
		return 1045;
	}

	@Override
	public double damageMutiplier(int level) {
		return 1.05d + 0.05d * level; 
	}

	@Override
	public int range(int level) {
		return level + 4;
	}

	@Override
	public int distance(int level) {
		return 10;
	}

	private static final int[] exps = { 100000, 300000, 741000, 1440000 };

	@Override
	public int getTargetExp(int level) {
		if(level<exps.length){
			return exps[level];
		}
		return 0;
	}

	@Override
	public int maxLevel() {
		return exps.length;
	}

	@Override
	public int levelUpLevel(int level) {
		return level * 10 + 40;
	}
	
	@Override
	public void handle(Client client, InteractPacket ip) {
		Player hero = client.getPlayer();
		int stamina = hero.getStamina();
		SkillProficiency prof = hero.getSkillProficiency(this);
		
		// The player should have the Skill and enough stamina
		if (prof!=null && stamina >= 20 ) {
			new PacketWriter(PacketType.SKILL_ANIMATION_PACKET, 20)
			.putUnsignedInteger(ip.getIdentity())
			.putUnsignedShort(ip.getX())
			.putUnsignedShort(ip.getY())
			.putUnsignedShort(getSkillID())
			.putUnsignedShort(prof.getLevel())
			.putUnsignedShort(0) // No damage
			.send(client);
			
			hero.setStamina(stamina - 20);
			hero.sendStamina();
		}
	}

	final static class ScentSword extends FastBlade {

		@Override
		public int getSkillID() {
			return 1046;
		}

	}
	
	/*
	 * 	public PacketWriter SkillAnimationPacket(Player hero, Map<Entity, Long> targets, int AimX, int AimY, long target) {
		PacketWriter pw = new PacketWriter(PacketType.SKILL_ANIMATION_PACKET, 20 + targets.size());
		pw.putUnsignedInteger(hero.getIdentity());
		
		if ( target != 0 ) {
			pw.putUnsignedInteger(target);
		} else {
			pw.putUnsignedShort(AimX);
			pw.putUnsignedShort(AimY);
		}
		
		pw.putUnsignedShort(skillID);
		
		pw.putUnsignedShort(targets.size());
		for ( Entry<Entity, Long> kv : targets.entrySet() ) {
			pw.putUnsignedInteger(kv.getKey().getIdentity());
			pw.putUnsignedInteger(kv.getValue());
		}
		
		return pw;
	}
	 */

}
