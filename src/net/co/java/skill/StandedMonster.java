package net.co.java.skill;

import net.co.java.skill.Skill.AbstractPassiveSkill;

class StandedMonster extends AbstractPassiveSkill {

	@Override
	public double chance(int level) {
		return .20 + .3 * level;
	}

	@Override
	public double damageMutiplier(int level) {
		return 1.3 + 0.01 * level;
	}

	@Override
	public int range(int level) {
		if(level>=8) return 6;
		if(level>=6) return 5;
		if(level>=3) return 4;
		return 3;
	}

	@Override
	public int getSkillID() {
		return 5020;
	}

}
