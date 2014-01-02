package data;

import packets.OutgoingPacket;
import packets.PacketType;

public class Monster extends Entity {

	public Monster(Location location, long identity, String name, int level, int mesh, int HP) {
		super(identity, mesh, 0, name, location, HP);
		this.level = level;
	}

	@Override
	public int getMaxHP() {
		// TODO Auto-generated method stub
		return 0;
	}

	public OutgoingPacket spawn() {
		return new OutgoingPacket(PacketType.ENTITY_SPAWN_PACKET, new byte[82+name.length()]) {{
			this.putUnsignedInteger(identity);
			this.putUnsignedInteger(mesh);
			this.setOffset(20); // TODO ulong status flags?
			this.putUnsignedShort(0); // Guild ID
			this.setOffset(23);
			this.putUnsignedByte(0); // Guild rank
			this.putUnsignedInteger(0); // garment 24
			this.putUnsignedInteger(0); // helm 28
			this.putUnsignedInteger(0); // arm 32
			this.putUnsignedInteger(0); // rw 36
			this.putUnsignedInteger(0); // lw 40
			this.setOffset(48);
			this.putUnsignedShort(HP); // health 48
			this.putUnsignedShort(level); // mob lvl 50
			this.putUnsignedShort(location.getxCord()); // 52
			this.putUnsignedShort(location.getyCord()); // 54
			this.putUnsignedShort(hairstyle); //56
			this.putUnsignedByte(4); // direction 58
			this.putUnsignedByte(0x01); // action 59
			this.putUnsignedByte(1); // reborn //60
			this.setOffset(62);
			this.putUnsignedByte(0); // level
			this.setOffset(80);
			this.putUnsignedByte(1);
			this.putUnsignedByte(name.length());
			this.putString(name);
		}};
	}
	
	public class Entity_Spawn_Packet extends OutgoingPacket {

		public Entity_Spawn_Packet() {
			super(PacketType.ENTITY_SPAWN_PACKET, new byte[82+name.length()]);
			this.putUnsignedInteger(identity);
			this.putUnsignedInteger(mesh);
			this.setOffset(20); // TODO ulong status flags?
			this.putUnsignedShort(0); // Guild ID
			this.setOffset(23);
			this.putUnsignedByte(0); // Guild rank
			this.putUnsignedInteger(0); // garment 24
			this.putUnsignedInteger(0); // helm 28
			this.putUnsignedInteger(0); // arm 32
			this.putUnsignedInteger(0); // rw 36
			this.putUnsignedInteger(0); // lw 40
			this.setOffset(48);
			this.putUnsignedShort(HP); // health 48
			this.putUnsignedShort(level); // mob lvl 50
			this.putUnsignedShort(location.getxCord()); // 52
			this.putUnsignedShort(location.getyCord()); // 54
			this.putUnsignedShort(hairstyle); //56
			this.putUnsignedByte(4); // direction 58
			this.putUnsignedByte(0x01); // action 59
			this.putUnsignedByte(1); // reborn //60
			this.setOffset(62);
			this.putUnsignedByte(0); // level
			this.setOffset(80);
			this.putUnsignedByte(1);
			this.putUnsignedByte(name.length());
			this.putString(name);
		}
		
	}
	
	
}
