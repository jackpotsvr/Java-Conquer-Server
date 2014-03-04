package net.co.java.entity;

import java.util.Map;
import java.util.Map.Entry;

import net.co.java.packets.InteractPacket;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.server.Server.GameServer.Client;

public enum Skill {
	
	FAST_BLADE(1045) {

		@Override
		public void handle(Client client, InteractPacket ip) {
			Player hero = client.getPlayer();
			int stamina = hero.getStamina();
			
			if ( stamina >= 20 ) {
				new PacketWriter(PacketType.SKILL_ANIMATION_PACKET, 20)
				.putUnsignedInteger(ip.getIdentity())
				.putUnsignedShort(ip.getX())
				.putUnsignedShort(ip.getY())
				.putUnsignedShort((int) ip.getSkill().skillID)
				.putUnsignedShort(2)
				.putUnsignedShort(0) // No damage
				.send(client);
				
				hero.setStamina(stamina - 20);
				hero.sendStamina();
			}
		}
		
	};
	
	public final int skillID;
	
	private Skill(int skillID) {
		this.skillID = skillID;
	}
	
	public int getSkillID() { return skillID; }
	
	public abstract void handle(Client client, InteractPacket ip);
	
	public PacketWriter SkillAnimationPacket(Player hero, Map<Entity, Long> targets, int AimX, int AimY, long target) {
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
	
	public static Skill valueOf(int skillID) {
		for ( Skill skill : Skill.values())
			if (skill.skillID == skillID )
				return skill;
		return null;
	}
}
