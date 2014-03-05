package net.co.java.skill;

import net.co.java.skill.Skill.AbstractPassiveSkill;

class SpeedGun extends AbstractPassiveSkill {

	@Override
	public double chance(int level) {
		return .2 + .03 * level;
	}

	@Override
	public double damageMutiplier(int level) {
		return 1 + .125 * level;
	}

	@Override
	public int range(int level) {
		return 0; // TODO line?
	}

	@Override
	public int getSkillID() {
		return 1260;
	}

	@Override
	public WeaponType getWeaponType() {
		return WeaponType.SPEAR;
	}

}
