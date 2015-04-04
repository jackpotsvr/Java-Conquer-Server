package net.co.java.skill;

import net.co.java.server.GameServerClient;
import net.co.java.skill.Skill.AbstractPassiveSkill;

public class Snow extends AbstractPassiveSkill {

	@Override
	public double chance(int level) {
		return .20 + .03 * level;
	}

	@Override
	public double damageMutiplier(int level) {
		if(level>=9) return 1.45;
		else if(level>=4) return 1.4;
		else return 1.1;
	}

	@Override
	public int range(int level) {
		if(level>=4) return 3;
		return 2;
	}

	@Override
	public int distance(int level) {
		return 9;
	}

	@Override
	public int getSkillID() {
		return 5010;
	}

	@Override
	public WeaponType getWeaponType() {
		return WeaponType.WAND;
	}

	@Override
	public TargetBuilder getHittedEntities(GameServerClient client, int level) {
		// TODO Auto-generated method stub
		return null;
	}

}
