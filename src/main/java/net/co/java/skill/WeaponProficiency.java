package net.co.java.skill;

import net.co.java.entity.Player;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;

public class WeaponProficiency {
	
	private static final int[] PROF_LEVEL_EXP = {
		0, 1200, 68000, 250000,640000,
		1600000, 4000000, 10000000, 22000000, 40000000,
		90000000, 95000000, 142500000, 213750000, 320625000,
		480937500, 721406250, 1082109375, 1623164063, 2100000000
	};
	
	public final Player player;
	public final WeaponType weapon;
	public int level;
	public long exp;
	
	public WeaponProficiency(Player player, WeaponType weapon, int level, long exp) {
		this.player = player;
		this.weapon = weapon;
		this.level = level;
		this.exp = exp;
	}
	
	/**
	 * Increase the Experience with a given amount
	 * @param amount
	 */
	public void increaseExperience(int amount) { setExperience(exp + amount); }

	/**
	 * Set the experience to a given value
	 * @param exp
	 */
	public void setExperience(long exp) {
		// The value should be greather than the current value
		if(exp > this.exp ) {
			// The skill should not be fixed
			if(level < PROF_LEVEL_EXP.length) {
				long targetExp = PROF_LEVEL_EXP[level];
				if(exp >= targetExp) {
					level++;
					this.exp = exp - targetExp;
					sendSkill();
				} else {
					this.exp = exp;
					sendUpdateSkill();
				}
			}
		}
	}
	
	public void sendSkill() {
		new PacketWriter(PacketType.PROFICIENCY, 16)
		.putUnsignedInteger(weapon.ProfID)
		.putUnsignedInteger(level)
		.putUnsignedInteger(exp)
		.send(player);
	}
	
	void sendUpdateSkill() {
		new PacketWriter(PacketType.SKILL_UPDATE_PACKET, 12)
		.putUnsignedInteger(exp)
		.putUnsignedShort(weapon.ProfID)
		.putUnsignedShort(0) // 0 prof, 1 magic, [2 skill]
		.send(player);
	}
	
	/**
	 * @return the skill for this Proficiency
	 */
	public WeaponType getWeaponType() { return weapon; }
	
	/**
	 * @return the level for this Proficiency
	 */
	public int getLevel() { return level; }
	
	/**
	 * @return the Experience for this Proficiency
	 */
	public long getExperience() { return exp; }
	
}
