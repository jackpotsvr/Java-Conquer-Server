package net.co.java.skill;

import net.co.java.skill.Skill.AbstractPassiveSkill;

class Rage extends AbstractPassiveSkill {

	@Override
	public double chance(int level) {
		return .2 + .3 * level;
	}

	@Override
	public double damageMutiplier(int level) {
		if(level>=9) return 1.45;
		else if(level>=4) return 1.4;
		return 1.1;
	}

	@Override
	public int range(int level) {
		return 2;
	}

	@Override
	public int getSkillID() {
		return 7020;
	}

}
