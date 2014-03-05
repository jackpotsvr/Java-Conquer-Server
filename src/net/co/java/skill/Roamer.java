package net.co.java.skill;

import net.co.java.skill.Skill.AbstractPassiveSkill;

class Roamer extends AbstractPassiveSkill {

	@Override
	public double chance(int level) {
		return .2 + .03 * level;
	}

	@Override
	public double damageMutiplier(int level) {
		return .9 + .02 * level;
	}

	@Override
	public int range(int level) {
		if ( level >= 6 ) return 5;
		return 4;
	}

	@Override
	public int getSkillID() {
		return 7040;
	}

	@Override
	public WeaponType getWeaponType() {
		return WeaponType.WHIP;
	}

}
