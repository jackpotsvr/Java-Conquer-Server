package net.co.java.skill;

import net.co.java.skill.Skill.AbstractPassiveSkill;

class WideStrike extends AbstractPassiveSkill {

	@Override
	public int getSkillID() {
		return 1250;
	}

	@Override
	public double chance(int level) {
		return .20 + .02 * level;
	}

	@Override
	public double damageMutiplier(int level) {
		return .9 + .1 * level;
	}

	@Override
	public int range(int level) {
		if (level >= 6)
			return 6;
		else if (level >= 4)
			return 5;
		else if (level >= 2)
			return 4;
		else
			return 3;
	}
	
	static final class Boreas extends WideStrike {
		
		@Override
		public int getSkillID() {
			return 5050;
		}
		
	}

	@Override
	public WeaponType getWeaponType() {
		return WeaponType.GLAIVE;
	}

}
