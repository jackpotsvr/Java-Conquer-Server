package net.co.java.skill;

import net.co.java.entity.Entity;
import net.co.java.entity.Player;
import net.co.java.packets.InteractPacket;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.server.GameServerClient;
import net.co.java.skill.Skill.MagicSkill;

public class Hercules extends MagicSkill
{

	@Override
	public double damageMutiplier(int level) {
		switch(level)
		{
			case 1:
			case 2:
			case 3:
				return 0.85d;
			default:
				return 0.90d;
		}
	}

	@Override
	public int range(int level) {
		return 3+level; 
	}

	@Override
	public int distance(int level) {
		return 10;
	}

	@Override
	public void handle(GameServerClient client, InteractPacket ip) {
		Player hero = client.getPlayer();
		int stamina = hero.getStamina();
		
		SkillProficiency prof = hero.getSkillProficiency(this);
		
		if ( /* check whether the caster has 2 equivalent weapon types. */
				prof!=null && stamina >= 30 ) {
			int range = range(prof.level);			
			TargetBuilder tb = new TargetBuilder(hero).inCircle(range);

			PacketWriter pw = new PacketWriter(PacketType.SKILL_ANIMATION_PACKET, 20 + tb.size() * 8)
				.putUnsignedInteger(ip.getIdentity())
	 			.putUnsignedShort(ip.getX())
				.putUnsignedShort(ip.getY())
				.putUnsignedShort(getSkillID())
				.putUnsignedShort(prof.getLevel())
				.putUnsignedShort(tb.size())
				.setOffset(20);
			
			for( Entity e : tb )
				pw.putUnsignedInteger(e.getIdentity()).putUnsignedInteger(1);
			
			pw.sendTo(hero.view.getPlayers());
			hero.setStamina(stamina - 30);
			hero.sendStamina();
		}
	}

	@Override
	public int getSkillID() {
		return 1115;
	}
	
	private static final int[] exps = { 167600, 590000, 1216800, 294800 };
	
	@Override
	public int getTargetExp(int level) {
		return exps[level];
	}

	@Override
	public int levelUpLevel(int level) {
		return level * 10 + 40;
	}

	@Override
	public int maxLevel() {
		return exps.length;	
	}

}
