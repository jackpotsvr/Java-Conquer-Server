package net.co.java.skill;

import net.co.java.skill.Skill.AbstractPassiveSkill;

class Phoenix extends AbstractPassiveSkill {

	@Override
	public int getSkillID() {
		return 5030;
	}

	@Override
	public double chance(int level) {
		return .33 + .05 * level;
	}

	@Override
	public double damageMutiplier(int level) {
		return 2.0 + (level + level%8) * 0.1;
	}

	@Override
	public int distance(int level) {
		return 10;
	}

	@Override
	public int range(int level) {
		return 9;
	}

}
