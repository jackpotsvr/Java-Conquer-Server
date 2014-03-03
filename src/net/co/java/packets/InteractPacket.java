package net.co.java.packets;

import java.math.BigInteger;

import net.co.java.entity.Skill;
import net.co.java.server.Server.GameServer.Client;

/**
 * The Interact packet is most commonly used for direct melee/archer attacks,
 * but also used for certain player to player actions, such as marriage.
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 * 
 */
public class InteractPacket implements PacketHandler {
	
	private final long timestamp;
	private final long identity;
	private final long target;
	private final int x;
	private final int y;
	private final Mode mode;
	private final Skill skill;
	private final IncomingPacket ip;
	
	/**
	 * Construct a new {@code InteractPacket} based on a {@code IncomingPacket}
	 * @param ip
	 */
	public InteractPacket(IncomingPacket ip) {
		this.timestamp = ip.readUnsignedInt(4);
		this.identity = ip.readUnsignedInt(8);
		this.mode = Mode.valueOf(ip.readUnsignedByte(20));
		this.ip = ip;
		
		int skillid = ip.readUnsignedShort(24);
		skillid ^= 0x915d;
		skillid ^= identity & 0xFFFF;
		skillid = (skillid << 0x3 | skillid >> 0xd ) & 0xFFFF;
		skillid -= 0xeb42;
		this.skill = Skill.valueOf(skillid);
		
		long x = ip.readUnsignedShort(16);
		x = x ^ ( identity & 0xFFFF ) ^ 0x2ed6;
		x = ((x << 1) | ((x & 0x8000) >> 15)) & 0xffff;
        x |= 0xffff0000;
        x -= 0xffff22ee;
        this.x = (int) x;
        
        long y = ip.readUnsignedShort(18);
        y = y ^ (identity & 0xffff) ^ 0xb99b;
        y = ((y << 5) | ((y & 0xF800) >> 11)) & 0xffff;
        y |= 0xffff0000;
        y -= 0xffff8922;
        this.y = (int) y;

        BigInteger target = ip.readUnsingedLong(12);
        this.target = target.and(BigInteger.valueOf(0xffffe000)).shiftRight(13)
			.or(target.and(BigInteger.valueOf(0x1fff)).shiftLeft(19))
			.xor(BigInteger.valueOf(0x5F2D2463))
			.xor(BigInteger.valueOf(identity))
			.subtract(BigInteger.valueOf(0x746F4AE6))
			.longValue();
        
	}

	/**
	 * @return the timer value
	 */
	public long getTimer() {
		return timestamp;
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
	public Skill getSkill() {
		return skill;
	}

	@Override
	public void handle(Client player) {
		switch(mode){
		case AcceptMarriage:
			break;
		case ArcherAttack:
			break;
		case DashEffect:
			break;
		case Death:
			break;
		case MagicAttack:
			skill.handle(player, this);
			break;
		case MagicReflect:
			break;
		case None:
			break;
		case PhysicalAttack:
			break;
		case RequestMarriage:
			break;
		case RushAttack:
			break;
		case SendFlowers:
			break;
		case WeaponReflect:
			break;
		default:
			break;
		
		}
	}

	@Override
	public String toString() {
		return "InteractPacket [timestamp=" + timestamp + ", identity=" + identity
				+ ", target=" + target + ", x=" + x + ", y=" + y + ", mode="
				+ mode + ", skill=" + skill + "]";
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

	@Override
	public PacketWriter build() {
		// TODO Auto-generated method stub
		return null;
	}

	public IncomingPacket getIp() {
		return ip;
	}	

}
