package net.co.java.entity;

import java.util.List;

import net.co.java.packets.GeneralData;
import net.co.java.packets.IncomingPacket;
import net.co.java.packets.PacketWriter;
import net.co.java.packets.GeneralData.SubType;
import net.co.java.server.Server.Map.Location;

public abstract class Entity implements Spawnable {

	protected final long identity;
	protected final String name;
	
	protected int mesh;
	protected int hairstyle;
	protected Location location;
	protected int HP;
	protected int mana;
	protected int level;
	
	public Entity(long identity, int mesh, int hairstyle, String name, Location location, int HP) {
		this.identity = identity;
		this.mesh = mesh;
		this.hairstyle = hairstyle;
		this.name = name;
		this.HP = HP;
	}
	
	/**
	 * 
	 * @param location
	 */
	public void setLocation(Location location, IncomingPacket ip) {
		Location oldLocation = this.location;
		this.location = location;
		System.out.println(name + " moving to " + this.location.toString());
		if (oldLocation == null ) {
			// Initial spawn, send spawn packet to surroundings 
			this.location.getMap().addEntity(this);
			this.spawn().sendToSurroundings(this);
			return;
		}
		if (oldLocation.getMap() != location.getMap()) {
			// Switched map (portal/scroll)
			oldLocation.getMap().removeEntity(this);
			location.getMap().addEntity(this);
		}
		// Prepare the packets
		PacketWriter spawnPacket = this.spawn();
		PacketWriter removePacket = this.removeEntity();
		// Send the packets to the surroundings
		for (Entity entity : location.getMap().getEntities()) {
			if ( entity == this ) continue;
			boolean isInView = entity.getLocation().inView(location);
			boolean isInOldView = entity.getLocation().inView(oldLocation);
			if ( this instanceof Player ) {
				// Remove entities that are not in view anymore
				if ( !isInView && isInOldView) {
					entity.removeEntity().send((Player) this);
				} else if ( isInView && !isInOldView ) {
					// Spawn new entities
					entity.spawn().send((Player) this);
				}
			}
			// If the entity is not a player, we're done here
			if (!(entity instanceof Player) || entity == this)
				continue;
			Player player = (Player) entity;
			if(isInView) {
				if(isInOldView && ip != null) {
					// When the player is in the new view
					// and was in the previous view as well,
					// forward the incoming Jump or Entity
					// move packet.
					ip.send(player.getClient());
				} else {
					// Send a spawn packet
					spawnPacket.send(player);
				}
			} else if (isInOldView) {
				// Remove the entity from the screen
				removePacket.send(player);
			}
		}
	}
	
	/**
	 * The walk method is called when an Entity moves.
	 * This can be called from AI (monsters/NPCs) or an Entity Move
	 * packet from the client. When existing, the packet can be
	 * forwarded. Otherwise, it needs to be created runtime.
	 * 
	 * When the Entity moves, old surroundings should receive
	 * an entity remove packet. New surroundings should receive
	 * an entity spawn packet. The intersection of surroundings
	 * should receive the delegated entity move packet.
	 */	
	public void walk(int direction, IncomingPacket entityMove) {
		setLocation(this.location.inDirection(direction), entityMove);
	}
	
	/**
	 * 
	 * @param x
	 * @param y
	 * @param jump
	 */
	public void jump(int x, int y, IncomingPacket jump) {
		setLocation(this.location.getMap().new Location(x,y), jump);
	}

	public abstract PacketWriter spawn();

	public PacketWriter removeEntity() {
		return new GeneralData(SubType.ENTITY_REMOVE, identity, new int[3]).build();
	}
	
	public List<Entity> getSurroundings() {
		return location.getMap().getEntitiesInRange(this);
	}
	
	public List<Player> getSurroundingPlayers() {
		return location.getMap().getPlayersInRange(this);
	}
	
	public int getMesh() {
		return mesh;
	}

	public void setMesh(int mesh) {
		this.mesh = mesh;
	}
	
	public int getHairstyle() {
		return hairstyle;
	}
	
	public void setHairstyle(int hairstyle) {
		this.hairstyle = hairstyle;
	}

	public int getMana() {
		return mana;
	}

	public void setMana(int mana) {
		this.mana = mana;
	}

	public int getLevel() {
		return level;
	}

	public void setLevel(int level) {
		this.level = level;
	}

	public long getIdentity() {
		return identity;
	}
	
	public String getName() {
		return name;
	}
	
	public int getHP() {
		return HP;
	}
	
	public void setHP(int HP) {
		// Prevent health points below zero
		this.HP = ( HP > 0 ) ? HP : 0;
	}
	
	public boolean isDead() {
		return HP == 0;
	}
	
	public void dealDamage(int damage) {
		setHP(HP - damage);
	}
	
	public abstract int getMaxHP();

	@Override
	public Location getLocation() {
		return location;
	}

	@Override
	public boolean inView(Spawnable spawnable) {
		return location.inView(spawnable.getLocation());
	}
	
	public static enum Action {
		None( 0x00),
		Cool ( 0xE6),
		Kneel ( 0xD2),
		Sad ( 0xAA),
		Happy ( 0x96),
		Angry (0xA0),
		Lie ( 0x0E),
		Dance ( 0x01),
		Wave ( 0xBE),
		Bow ( 0xC8),
		Sit ( 0xFA),
		Jump ( 0x64);
		
		private final int index;
		
		private Action(int index) {
			this.index = index;
		}
		
		public int getIndex() {
			return index;
		}
	}

}
