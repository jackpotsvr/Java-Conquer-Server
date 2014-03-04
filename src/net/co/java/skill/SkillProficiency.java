package net.co.java.skill;

import net.co.java.entity.Player;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;

/**
 * A Skill Proficiency binds a skill, its level and its progress to the next level
 * to a Player
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public class SkillProficiency {
	
	public final Player player;
	public final Skill skill;
	public int level;
	public long exp;
	
	/**
	 * Construct a new Proficiency object
	 * @param player
	 * @param skill
	 * @param level
	 * @param exp
	 */
	public SkillProficiency(Player player, Skill skill, int level, long exp) {
		this.player = player;
		this.skill = skill;
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
			// The player should have the required level to train the skill
			if(skill.levelUpLevel(level) <= player.getLevel()) {
				// The skill should not be fixed
				if(skill.maxLevel() > level ) {
					long targetExp = skill.getTargetExp(level);
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
	}
	
	public void sendSkill() {
		new PacketWriter(PacketType.SKILL_PACKET, 12)
		.putUnsignedInteger(exp)
		.putUnsignedShort(skill.getSkillID())
		.putUnsignedShort(level)
		.send(player);
	}
	
	void sendUpdateSkill() {
		new PacketWriter(PacketType.SKILL_UPDATE_PACKET, 12)
		.putUnsignedInteger(exp)
		.putUnsignedShort(skill.getSkillID())
		.putUnsignedShort(1) // 0 prof, 1 magic, [2 skill]
		.send(player);
	}
	
	/**
	 * @return the skill for this Proficiency
	 */
	public Skill getSkill() { return skill; }
	
	/**
	 * @return the level for this Proficiency
	 */
	public int getLevel() { return level; }
	
	/**
	 * @return the Experience for this Proficiency
	 */
	public long getExperience() { return exp; }

}
