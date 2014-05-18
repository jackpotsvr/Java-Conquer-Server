package net.co.java.skill;

import net.co.java.server.GameServerClient;
import net.co.java.skill.Skill.AbstractPassiveSkill;

class Celestial extends AbstractPassiveSkill {

	@Override
	public double chance(int level) {
		return .1 + 0.01 * level;
	}

	@Override
	public double damageMutiplier(int level) {
		return 1.1 + .02 * level;
	}

	@Override
	public int range(int level) {
		return 0;
	}

	@Override
	public int getSkillID() {
		return 7030;
	}

	@Override
	public WeaponType getWeaponType() {
		return WeaponType.SCEPTER;
	}

	@Override
	public TargetBuilder getHittedEntities(GameServerClient client, int level) {
		// TODO Auto-generated method stub
		return null;
	}

}
