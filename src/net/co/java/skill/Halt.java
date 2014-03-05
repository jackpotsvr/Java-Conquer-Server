package net.co.java.skill;

import net.co.java.skill.Skill.AbstractPassiveSkill;

class Halt extends AbstractPassiveSkill {

	@Override
	public double chance(int level) {
		return .2 + .02 *level;
	}

	@Override
	public double damageMutiplier(int level) {
		return .9 + .1 * level;
	}

	@Override
	public int range(int level) {
		return 0; // TODO Sector?
	}

	@Override
	public int getSkillID() {
		return 1300;
	}

	@Override
	public WeaponType getWeaponType() {
		return WeaponType.LONGHAMMER;
	}

}
