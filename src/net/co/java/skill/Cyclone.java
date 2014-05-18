package net.co.java.skill;

import net.co.java.entity.Entity.Flag;
import net.co.java.packets.InteractPacket;
import net.co.java.packets.UpdatePacket;
import net.co.java.packets.UpdatePacket.Mode;
import net.co.java.server.GameServerClient;
import net.co.java.skill.Skill.MagicSkill;

public class Cyclone extends MagicSkill {

	public Cyclone(){
		
	}
	
	@Override
	public double damageMutiplier(int level) {
		return 0;
	}

	@Override
	public int range(int level) {
		return 0;
	}

	@Override
	public int distance(int level) {
		return 0;
	}

	@Override
	public void handle(GameServerClient player, InteractPacket ip) {
		player.getPlayer().setFlag(Flag.CYCLONE);
		new UpdatePacket(player.getPlayer())
			.setAttribute(Mode.RaiseFlag, player.getPlayer().getFlags())
			.build().send(player.getPlayer().getClient());
	}

	@Override
	public int getSkillID() {
		return 1110;
	}

	@Override
	public int getTargetExp(int level) {
		return 0;
	}

	@Override
	public int levelUpLevel(int level) {
		return 0;
	}

	@Override
	public int maxLevel() {
		return 0;
	}

}
