package net.co.java.entity;

import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.server.Server.Map.Location;

public class Monster extends Entity {

	public Monster(Location location, long identity, String name, int level, int mesh, int HP) {
		super(identity, mesh, 0, name, location, HP);
		this.level = level;
		setLocation(location, null);
	}

	@Override
	public int getMaxHP() {
		// TODO Auto-generated method stub
		return getHP();
	}

	@Override
	public PacketWriter spawn() {
		return new PacketWriter(PacketType.ENTITY_SPAWN_PACKET, 82 + name.length())
		.putUnsignedInteger(identity)
		.putUnsignedInteger(mesh)
		.setOffset(20) // TODO ulong status flags?
		.putUnsignedShort(0) // Guild ID
		.setOffset(23)
		.putUnsignedByte(0) // Guild rank
		.putUnsignedInteger(0) // garment 24
		.putUnsignedInteger(0) // helm 28
		.putUnsignedInteger(0) // arm 32
		.putUnsignedInteger(0) // rw 36
		.putUnsignedInteger(0) // lw 40
		.setOffset(48)
		.putUnsignedShort(HP) // health 48
		.putUnsignedShort(level) // mob lvl 50
		.putUnsignedShort(location.getxCord()) // 52
		.putUnsignedShort(location.getyCord()) // 54
		.putUnsignedShort(hairstyle) //56
		.putUnsignedByte(4) // direction 58
		.putUnsignedByte(0x01) // action 59
		.putUnsignedByte(1) // reborn //60
		.setOffset(62)
		.putUnsignedByte(0) // level
		.setOffset(80)
		.putUnsignedByte(1)
		.putUnsignedByte(name.length())
		.putString(name);
	}

	@Override
	public int getMaxMana() {
		// TODO Auto-generated method stub
		return getMana();
	}
	
}
