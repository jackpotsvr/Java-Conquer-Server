package net.co.java.entity;

import java.util.List;

import net.co.java.packets.GeneralData;
import net.co.java.packets.IncomingPacket;
import net.co.java.packets.PacketWriter;
import net.co.java.packets.GeneralData.SubType;

/**
 * Abstract class for entities (eg. Monsters and Players)
 * @author Thomas Gmelig Meyling
 * @author Jan-Willem Gmelig Meyling
 *
 */
public abstract class Entity implements Spawnable {

	protected final long identity;
	protected final String name;
	
	protected int mesh;
	protected int hairstyle;
	protected Location location;
	protected int HP;
	protected int mana;
	protected int level;
	
	/**
	 * Construct a new entity
	 * @param identity
	 * @param mesh
	 * @param hairstyle
	 * @param name
	 * @param location
	 * @param HP
	 */
	public Entity(long identity, int mesh, int hairstyle, String name, Location location, int HP) {
		this.identity = identity;
		this.location = location;
		this.mesh = mesh;
		this.hairstyle = hairstyle;
		this.name = name;
		this.HP = HP;
	}
	
	/**
	 * This method is called to setup the initial spawn. It adds the Entity
	 * to a new Map and sends the Spawn packet to the surrounding Players.
	 */
	public void spawn() {
		if(location != null) {
			location.getMap().addEntity(this);
		}
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
			this.SpawnPacket().sendToSurroundings(this);
			return;
		}
		if (oldLocation.getMap() != location.getMap()) {
			// Switched map (portal/scroll)
			oldLocation.getMap().removeEntity(this);
			location.getMap().addEntity(this);
		}
		// Prepare the packets
		PacketWriter spawnPacket = this.SpawnPacket();
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
					entity.SpawnPacket().send((Player) this);
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
		setLocation(new Location(this.location.map, x, y), jump);
	}

	public abstract PacketWriter SpawnPacket();

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
		if ( HP < 0 )
			HP = 0;
		int maxHP = getMaxHP();
		if ( HP > maxHP )
			HP = maxHP;
		this.HP = HP;
	}
	
	public boolean isDead() {
		return HP == 0;
	}
	
	public void dealDamage(int damage) {
		setHP(HP - damage);
	}
	
	public abstract int getMaxHP();
	public abstract int getMaxMana();

	@Override
	public Location getLocation() {
		return location;
	}

	@Override
	public boolean inView(Spawnable spawnable) {
		return location.inView(spawnable.getLocation());
	}

	@Override
	public String toString() {
		return "Entity [identity=" + identity + ", name=" + name + ", mesh="
				+ mesh + ", hairstyle=" + hairstyle + ", location=" + location
				+ ", HP=" + HP + ", mana=" + mana + ", level=" + level + "]";
	}

	
}
