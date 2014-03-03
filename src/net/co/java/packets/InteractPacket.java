package net.co.java.packets;

public class InteractPacket {
	
	private final long timer;
	private final long identity;
	private final long target;
	private final int x;
	private final int y;
	private final short mode;
	private final long damage;
	
	public InteractPacket(IncomingPacket ip) {
		timer = ip.readUnsignedInt(4);
		identity = ip.readUnsignedInt(8);
		target = ip.readUnsignedInt(12);
		x = ip.readUnsignedShort(16);
		y = ip.readUnsignedShort(18);
		mode = ip.readUnsignedByte(20);
		damage = ip.readUnsignedInt(24);
	}

	public long getTimer() {
		return timer;
	}

	public long getIdentity() {
		return identity;
	}

	public long getTarget() {
		return target;
	}

	public int getX() {
		return x;
	}

	public int getY() {
		return y;
	}

	public short getMode() {
		return mode;
	}

	public long getDamage() {
		return damage;
	}

	@Override
	public String toString() {
		return "InteractPacket [timer=" + timer + ", identity=" + identity
				+ ", target=" + target + ", x=" + x + ", y=" + y + ", mode="
				+ mode + ", damage=" + damage + "]";
	}
	
	
	

}
