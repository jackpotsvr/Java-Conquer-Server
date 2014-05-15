package net.co.java.packets;

import net.co.java.entity.Entity;
import net.co.java.entity.Monster;
import net.co.java.entity.NPC;
import net.co.java.entity.Player;
import net.co.java.entity.Player.Inventory;

/**
 * The SpawnPacket is used to spawn {@code Entities} into a {@code Player}s view.
 * The SpawnPacket makes distinction between NPCs, Monsters and Players itself.
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public class SpawnPacket extends PacketWriter {
	
	private final Entity entity;

	public SpawnPacket(final NPC npc) {
		super(PacketType.NPC_SPAWN_PACKET, 21 + npc.getName().length());
		this.entity = npc;
		this.putUnsignedInteger(npc.getIdentity())
			.putUnsignedShort(npc.getLocation().getxCord())
			.putUnsignedShort(npc.getLocation().getyCord())
			.putUnsignedShort(npc.getTypeD())
			.putUnsignedShort(npc.getInteraction().value)
			.putUnsignedShort(npc.getNPC_Flag().value)
			.putUnsignedByte(1)
			.putUnsignedByte(npc.getName().length())
			.putString(npc.getName());
	}
	
	public SpawnPacket(final Monster monster) {
		super(PacketType.ENTITY_SPAWN_PACKET, 82 + monster.getName().length());
		this.entity = monster;
		this.putUnsignedInteger(monster.getIdentity())
			.putUnsignedInteger(monster.getMesh())
			.setOffset(12).putUnsignedInteger(monster.getFlags())
			.setOffset(20).putUnsignedShort(0) // Guild ID
			.setOffset(23)
			.putUnsignedByte(0) // Guild rank
			.putUnsignedInteger(0) // garment 24
			.putUnsignedInteger(0) // helm 28
			.putUnsignedInteger(0) // arm 32
			.putUnsignedInteger(0) // rw 36
			.putUnsignedInteger(0) // lw 40
			.setOffset(48)
			.putUnsignedShort(monster.getHP()) // health 48
			.putUnsignedShort(monster.getLevel()) // mob lvl 50
			.putUnsignedShort(monster.getLocation().getxCord()) // 52
			.putUnsignedShort(monster.getLocation().getyCord()) // 54
			.putUnsignedShort(monster.getHairstyle()) //56
			.putUnsignedByte(0) // direction 58
			.putUnsignedByte(0) // action 59
			.putUnsignedByte(0) // reborn //60
			.setOffset(62)
			.putUnsignedByte(0) // level
			.setOffset(80)
			.putUnsignedByte(1)
			.putUnsignedByte(monster.getName().length())
			.putString(monster.getName());
	}
	
	public SpawnPacket(final Player player) {
		super(PacketType.ENTITY_SPAWN_PACKET, 82 + player.getName().length());
		this.entity = player;
		this.putUnsignedInteger(player.getIdentity())
			.putUnsignedInteger(player.getMesh())
			.setOffset(12).putUnsignedInteger(player.getFlags())
			.setOffset(20).putUnsignedShort(0) // Guild ID
			.setOffset(23)
			.putUnsignedByte((short) player.getGuildRank()) // Guild rank
			.putUnsignedInteger(player.inventory.getEquipmentSID(Inventory.GARMENT)) // garment 24
			.putUnsignedInteger(player.inventory.getEquipmentSID(Inventory.HELM)) // helm 28
			.putUnsignedInteger(player.inventory.getEquipmentSID(Inventory.ARMOR)) // arm 32
			.putUnsignedInteger(player.inventory.getEquipmentSID(Inventory.RIGHT_HAND)) // rw 36
			.putUnsignedInteger(player.inventory.getEquipmentSID(Inventory.LEFT_HAND)) // lw 40
			.setOffset(48)
			.putUnsignedShort(player.getHP()) // health 48
			.putUnsignedShort(0) // mob lvl 50
			.putUnsignedShort(player.getLocation().getxCord()) // 52
			.putUnsignedShort(player.getLocation().getyCord()) // 54
			.putUnsignedShort(player.getHairstyle()) //56
			.putUnsignedByte(player.getLocation().getDirection()) // direction 58
			.putUnsignedByte(player.getAction()) // action 59
			.putUnsignedByte(player.getRebornCount()) // reborn //60
			.setOffset(62)
			.putUnsignedByte(player.getLevel()) // level
			.setOffset(80)
			.putUnsignedByte(1)
			.putUnsignedByte(player.getName().length())
			.putString(player.getName());	
	}
	
	/**
	 * @param entity
	 * @return A {@code SpawnPacket} for the given entity
	 */
	public static SpawnPacket create(Entity entity) {
		if(entity instanceof Player) {
			return new SpawnPacket((Player) entity);
		} else if ( entity instanceof NPC ) {
			return new SpawnPacket((NPC) entity);
		} else if ( entity instanceof Monster) {
			return new SpawnPacket((Monster) entity);
		}
		throw new RuntimeException("Unsupported entity type");
	}
	
	public Entity getEntity() {
		return entity;
	}

}
