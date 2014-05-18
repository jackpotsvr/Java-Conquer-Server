package net.co.java.skill;

import net.co.java.server.GameServerClient;
import net.co.java.skill.Skill.AbstractPassiveSkill;

class Seizer extends AbstractPassiveSkill {

	@Override
	public double chance(int level) {
		return .15 + .02 * level; // Not exactly...
	}

	@Override
	public double damageMutiplier(int level) {
		return 1.05 + .01 * level;
	}

	@Override
	public int range(int level) {
		return 0; // Not defined for Seizer
	}

	@Override
	public int distance(int level) {
		return 0; // Not defined for Seizer
	}

	@Override
	public int getSkillID() {
		return 7000;
	}

	@Override
	public WeaponType getWeaponType() {
		return WeaponType.HOOK;
	}

	@Override
	public TargetBuilder getHittedEntities(GameServerClient client, int level) {
		// TODO Auto-generated method stub
		return null;
	}

}
