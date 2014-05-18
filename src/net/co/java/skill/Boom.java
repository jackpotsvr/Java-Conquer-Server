package net.co.java.skill;

import net.co.java.server.GameServerClient;
import net.co.java.skill.Skill.AbstractPassiveSkill;

class Boom extends AbstractPassiveSkill {

	@Override
	public double chance(int level) {
		return .1 + .02 * level;
	}

	@Override
	public double damageMutiplier(int level) {
		return 0; // TODO Stuns the target instead!
	}

	@Override
	public int range(int level) {
		return 0; // Not defined for Boom
	}

	@Override
	public int getSkillID() {
		return 5040;
	}

	@Override
	public WeaponType getWeaponType() {
		return WeaponType.HAMMER;
	}

	@Override
	public TargetBuilder getHittedEntities(GameServerClient client, int level) {
		// TODO Auto-generated method stub
		return null;
	}

}
