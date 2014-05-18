package net.co.java.skill;

import net.co.java.server.GameServerClient;
import net.co.java.skill.Skill.AbstractPassiveSkill;

class Penetration extends AbstractPassiveSkill {

	@Override
	public double chance(int level) {
		return .26 + .01 * level;
	}

	@Override
	public double damageMutiplier(int level) {
		return 1.95 + .05 * level;
	}

	@Override
	public int range(int level) {
		return 0;
	}

	@Override
	public int distance(int level) {
		return 0;
	}

	@Override
	public int getSkillID() {
		return 1290;
	}

	@Override
	public WeaponType getWeaponType() {
		return WeaponType.DAGGER;
	}

	@Override
	public TargetBuilder getHittedEntities(GameServerClient client, int level) {
		// TODO Auto-generated method stub
		return null;
	}

}
