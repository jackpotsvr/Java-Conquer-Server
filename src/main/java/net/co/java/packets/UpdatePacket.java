package net.co.java.packets;

import java.util.HashMap;
import java.util.Map;
import java.util.Map.Entry;

import net.co.java.entity.Entity;

/**
 * The Entity Status packet, also known as the "Update packet" is used to change
 * the appearance(in some cases), certain values unique to a character (such as
 * level, stat points, exp) and show active abilities (in other cases). This
 * packet can be used to send 1 status update, or many.
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 * 
 */
public class UpdatePacket implements PacketWrapper {

	private final Entity entity;
	private final Map<Mode, Long> attributes = new HashMap<Mode, Long>();
	
	/**
	 * Create a new UpdatePacket
	 * @param entity
	 */
	public UpdatePacket(Entity entity) {
		this.entity = entity;
	}
	
	/**
	 * Set an attribute for this UpdatePacket
	 * @param mode
	 * @param value
	 * @return this UpdatePacket instance (builder pattern)
	 */
	public UpdatePacket setAttribute(Mode mode, Long value) {
		attributes.put(mode, value);
		return this;
	}

	@Override
	public PacketWriter build() {
		int StatusCount = attributes.size();
		
		PacketWriter pw = new PacketWriter(PacketType.UPDATE_PACKET, 12 + 12 * StatusCount)
		.putUnsignedInteger(entity.getIdentity())
		.putUnsignedInteger(StatusCount);
		
		for( Entry<Mode, Long> kv : attributes.entrySet() ) {
			pw.putUnsignedInteger(kv.getKey().mode)
			.putUnsignedInteger(kv.getValue().longValue())
			.putUnsignedInteger(2);
		}
		
		return pw;
	}
	
	public static enum Mode {
		HP(0), MaxHP(1), Mana(2), MaxMana(3), Money(4), Experience(5), PKPoints(6), Job(7), Stamina(
				9), StatPoints(11), Model(12), Level(13), Spirit(14), Vitality(
				15), Strength(16), Agility(17), GuildDonation(20), KOSeconds(22), RaiseFlag(
				26), Hairstyle(27), XPCircle(28), LuckyTime(29),  LocationPoint(255);
		
		public final int mode;
		private Mode(int i) { mode = i; }
	}

}
