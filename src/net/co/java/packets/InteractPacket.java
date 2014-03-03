package net.co.java.packets;

/**
 * The Interact packet is most commonly used for direct melee/archer attacks,
 * but also used for certain player to player actions, such as marriage.
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 * 
 */
public class InteractPacket {
	
	private final long timer;
	private final long identity;
	private final long target;
	private final int x;
	private final int y;
	private final Mode mode;
	private final long damage;
	
	/**
	 * Construct a new {@code InteractPacket} based on a {@code IncomingPacket}
	 * @param ip
	 */
	public InteractPacket(IncomingPacket ip) {
		timer = ip.readUnsignedInt(4);
		identity = ip.readUnsignedInt(8);
		target = ip.readUnsignedInt(12);
		x = ip.readUnsignedShort(16);
		y = ip.readUnsignedShort(18);
		mode = Mode.valueOf(ip.readUnsignedByte(20));
		damage = ip.readUnsignedInt(24);
	}

	/**
	 * @return the timer value
	 */
	public long getTimer() {
		return timer;
	}

	/**
	 * @return the identity value
	 */
	public long getIdentity() {
		return identity;
	}

	/**
	 * @return the target value
	 */
	public long getTarget() {
		return target;
	}

	/**
	 * @return the X value
	 */
	public int getX() {
		return x;
	}

	/**
	 * @return the Y value
	 */
	public int getY() {
		return y;
	}

	/**
	 * @return the mode
	 */
	public Mode getMode() {
		return mode;
	}

	/**
	 * @return the damage
	 */
	public long getDamage() {
		return damage;
	}

	@Override
	public String toString() {
		return "InteractPacket [timer=" + timer + ", identity=" + identity
				+ ", target=" + target + ", x=" + x + ", y=" + y + ", mode="
				+ mode + ", damage=" + damage + "]";
	}
	
	/**
	 * An enumeration of Interaction modes
	 * @author Jan-Willem Gmelig Meyling
	 */
	public static enum Mode {
		None(0),
		PhysicalAttack(2),
		RequestMarriage(8),
		AcceptMarriage(9),
		SendFlowers(13),
		Death(14),
		RushAttack(20),
		MagicAttack(21),
		WeaponReflect(23),
		DashEffect(24),
		ArcherAttack(25),
		MagicReflect(26);
		
		public final int mode;
		
		private Mode(int mode) {
			this.mode = mode;
		}
		
		/**
		 * @param mode
		 * @return the Mode for a given value
		 */
		public static Mode valueOf(int mode) {
			for(Mode m : Mode.values())
				if(m.mode == mode)
					return m;
			return null;
		}
	}	

}
