package net.co.java.skill;

import net.co.java.skill.Skill.AbstractPassiveSkill;

class Earthquake extends AbstractPassiveSkill {

	@Override
	public double chance(int level) {
		return .1 + .02 * level;
	}

	@Override
	public double damageMutiplier(int level) {
		return 0; // Stun the target
	}

	@Override
	public int range(int level) {
		return 0; // Not defined?
	}

	@Override
	public int getSkillID() {
		return 7010;
	}

}
