package net.co.java.entity;

import java.io.Serializable;
import java.util.Arrays;
import java.util.List;

import net.co.java.packets.GeneralData;
import net.co.java.packets.IncomingPacket;
import net.co.java.packets.PacketType;
import net.co.java.packets.PacketWriter;
import net.co.java.packets.GeneralData.SubType;

/**
 * Abstract class for entities (eg. Monsters and Players)
 * 
 * @author Thomas Gmelig Meyling
 * @author Jan-Willem Gmelig Meyling
 *
 */
public abstract class Entity implements Spawnable, Serializable {

	private static final long serialVersionUID = -8544332465232461099L;
	protected transient final long identity;
	protected final String name;
	
	protected volatile int mesh;
	protected volatile int hairstyle;
	protected volatile Location location;
	protected volatile int HP;
	protected volatile int mana;
	protected volatile int level;
	protected volatile long flags = 0;
	public final transient View view = new View();

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
	 * Set the location for this entity
	 * @param location
	 */
	public void setLocation(Location location) {
		if(this.location != null && this.location.getMap() != location.getMap()) {
				this.location.getMap().removeEntity(this);
		}
		location.getMap().addEntity(this);
		this.location = location;
		this.view.update();
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
	 * 
	 * @param direction
	 * @param walkPacket
	 */	
	public void walk(int direction, IncomingPacket walkPacket) {
		PacketWriter pw = walkPacket != null ? new PacketWriter(walkPacket) : 
			new PacketWriter(PacketType.ENTITY_MOVE_PACKET, 12)
				.putUnsignedInteger(identity)
				.putUnsignedByte(direction)
				.putUnsignedByte(1);
		// Send walk packet to me and surroundings
		for(Player player : view.players(true))
			pw.send(player);
		// Change location and send spawn / entity remove packets
		setLocation(this.location.inDirection(direction));
	}
	
	/**
	 * @param x
	 * @param y
	 * @param direction
	 * @param jumpPacket
	 */
	public void jump(int x, int y, int direction, IncomingPacket jumpPacket) {
		PacketWriter pw = jumpPacket != null ? new PacketWriter(jumpPacket) : 
			new GeneralData(GeneralData.SubType.JUMP, this).setDwParam(y << 16 | x).build();
		// Send jump packet to me and surroundings
		for(Player player : view.players(true))
			pw.send(player);
		// Change location and send spawn / entity remove packets
		setLocation(new Location(this.location.map, x, y, direction));
			
	}

	/**
	 * This method is called to setup the initial spawn. It adds the Entity
	 * to a new Map and sends the Spawn packet to the surrounding Players.
	 */
	public void spawn() {
		if(location != null)
			setLocation(location);
	}

	/**
	 * Remove the entity
	 */
	public void remove() {
		if(location != null) {
			location.getMap().removeEntity(this);
			PacketWriter remove = this.removeEntity();
			for(Player player : view.players(false))
				remove.send(player);
		}
	}

	public abstract PacketWriter SpawnPacket();

	public PacketWriter removeEntity() {
		return new GeneralData(SubType.ENTITY_REMOVE, this).build();
	}

	/**
	 * Set a flag for this Entity
	 * @param flag
	 */
	public void setFlag(Flag flag) {
		flags |= flag.value;
	}

	/**
	 * Remove a flag for this Entity
	 * @param flag
	 */
	public void removeFlag(Flag flag) {
		flags ^= flag.value;
	}

	/**
	 * Check if this Entity has a given flag
	 * @param flag
	 * @return true if the Entity has the flag
	 */
	public boolean hasFlag(Flag flag) {
		return (flags & flag.value) == flag.value;
	}

	/**
	 * @return the Flags for this entity
	 */
	public long getFlags() {
		return flags;
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

	/**
	 * The View class is a collection that contains all Entities within the
	 * view range of this Entity
	 * 
	 * @author Jan-Willem Gmelig Meyling
	 * @author Thomas Gmelig Meyling
	 */
	public class View {

		private final static int GROW_AMOUNT = 20; // TODO SET GROW AMOUNT BACK TO 10 AND FIX ERROR IF MORE THAN GROW_AMOUNT ENTITIES GO OFF SCREEN IN ONE CHANGE.
		private Entity[] entities = new Entity[GROW_AMOUNT];
		private int index = 0;
		private int playerCount = 0;
		private int capacity = GROW_AMOUNT;
		
		/**
		 * This method is used to grow the array when necessary
		 */
		private void ensureCapacity() {
			if(index>=capacity) {
				capacity += GROW_AMOUNT;
				entities = Arrays.copyOf(entities, capacity);
			}
		}
		
		/**
		 * This method is used to shrink the array if possible
		 */
		private void freeCapacity() {
			if(index<capacity-GROW_AMOUNT) {
				capacity -= GROW_AMOUNT;
				entities = Arrays.copyOf(entities, capacity);
			}
		}
		
		/**
		 * Check if the View contains a specific Entity
		 * @param e
		 * @return true if the Entity is in View
		 */
		public synchronized boolean contains(Entity e) {
			for(int i = 0; i < index; i++ ) {
				if(entities[i] == e ) {
					return true;
				}
			}
			return false;
		}
		
		/**
		 * @return the amount of Entities in this View
		 */
		public synchronized int size() {
			return index;
		}
	
		/**
		 * Add an Entity to this Entities view. If the Entity is
		 * not in the current view of the other entity, send spawn packets
		 * to each other.
		 * @param e
		 */
		public synchronized void add(Entity e) {
			// The View is a set, so it should not already contain the Entity
			if(!contains(e)) {
				// Grow the array if necessary
				ensureCapacity();
				// Insert the entity at current insert index
				entities[index++] = e;
				// Add this entity to the other entities view as well
				e.view.add(Entity.this);				
				// Send a spawn packet for the entity
				
				//System.out.println("The entity is: " + e);
				//if(Entity.this instanceof Player && e instanceof NPC)
				//	((NPC)e).SpawnPacket().send(((Player) Entity.this).getClient());
				if(Entity.this instanceof Player && e != Entity.this)
					e.SpawnPacket().send((Player) Entity.this);		
				if(e instanceof Player)
					playerCount++;
			}
		}
	
		/**
		 * Remove an Entity from this Entities view. If the Entity
		 * is not in the current view of the other entity, send entity
		 * remove packets to each other.
		 * @param e
		 */
		public synchronized void remove(Entity e) {
			for(int i = 0; i < index; i++ ) {
				if(entities[i] == e ) {
					remove(i);
					break;
				}
			}
		}
		
		/**
		 * Remove Entity from the View at given index
		 * @param i
		 */
		private void remove(int i) {
			Entity e = entities[i];
			// Shrink other elements to the left
			for(int j = i; j < index; j++ ) {
				entities[j] = entities[j+1];
			}
			// Decrease size/ insert index
			index--;
			// Shrink array
			freeCapacity();
			// Remove this entity from the other entities view as well
			e.view.remove(Entity.this);
			// Send Entity remove packet to the player
			if(Entity.this instanceof Player)
				e.removeEntity().send((Player) Entity.this);
			if(e instanceof Player)
				playerCount--;
		}
	
		/**
		 * Update an entities view, remove entities that were in the
		 * previous view, but not in the current, and spawn entities
		 * that only appear in the current view.
		 */
		public synchronized void update() {
			List<Entity> allEntitites = Entity.this.location.getMap().getEntities();
			// Remove entities that only exist in the old view
			for(int i = 0; i < index; i++ ) {
				Entity e = entities[i];
				if(!e.getLocation().inView(location)) {
					remove(i);
					i--;
				}
			}
			// Add new entities
			for(Entity e : allEntitites)
			{
				if(e.getLocation().inView(location))
					add(e);
			}
		}

		/**
		 * @param includingMe
		 * @return An array containing all entities in this view
		 */
		public synchronized Entity[] entities(boolean includingMe) {
			int size = index;
			if(!includingMe) size -= 1; 
			Entity[] result = new Entity[size];
			for(int i = 0, j = 0; i < index; i++ )
				if(includingMe || entities[i] != Entity.this)
					result[j++] = entities[i];
			return result;
		}
		
		/**
		 * @param includingMe
		 * @return an array containing all players in this view
		 */
		public synchronized Player[] players(boolean includingMe) {
			int size = playerCount;
			if(!includingMe) size -= 1; 
			Player[] result = new Player[size];
			for(int i = 0, j = 0; i < index; i++ )
				if(includingMe || entities[i] != Entity.this)
					if(entities[i] instanceof Player)
						result[j++] = (Player) entities[i];
			return result;
		}
		
	}

	/**
	 * Types for Flags
	 * 
	 * @author Jan-Willem Gmelig Meyling
	 * @author Thomas Gmelig Meyling
	 *
	 */
	public static enum Flag {
		CLEAR(0),
		
		/**
		 * Once you attack white/red name players, your name will turn blue and
		 * players or guards/patrols may attack you without punishment
		 */
		BLUENAME(1),
		
		/**
		 * Poison animation
		 */
		POISON(2),
		
		/**
		 * Exp bar
		 */
		EXP(2 << 3),
		
		/**
		 * Unknown for now
		 */
		TRADE_OR_DEAD(2 << 4),
		
		/**
		 * Team leader (star)
		 */
		TEAMLEADER(2 << 5),
		
		/**
		 * Star of accuracy beneath entity
		 */
		ACCURACY(2 << 6),
		
		/**
		 * Shield around entity
		 */
		SHIELD(2 << 7),
				
		
		/**
		 * With this Flag an Stigma appears beneath the Entity. The Stigma always is in the
		 * new fixed form (red). When you want to use another animation render for different
		 * spell levels, you can use a StringPacket instead:
		 * 
		 * <pre>
		 * {@code
		 *  String animation = "attackup35"; // See ini/3DEffect.ini
		 * 	new PacketWriter(PacketType.STRING_PACKET, 11 + animation.length())
		 * 	  .putUnsignedInteger(client.getPlayer().getIdentity())
		 * 	  .putUnsignedByte(10) // Type
		 * 	  .putUnsignedByte(1) // Str count
		 * 	  .putUnsignedByte(animation.length()) // Str length
		 * 	  .putString(animation)
		 * 	  .send(client);
		 * }
		 * </pre>
		 */
		STIGMA(2 << 8),
		
		REVIVE_COUNTER_GHOST(2 << 9),
		FADE_AWAY(2 << 10),
		REDNAME(2 << 13),
		BLACKNAME(2 << 14),
		REFLECT(2 << 16),
		SUPERMAN(2 << 17),
		BALL(2 << 18), BALL2(2 << 19),
		INVISIBLE(2 << 21),
		CYCLONE(2 << 22),
		DODGE(2 << 25),
		FLY(2 << 26),
		INTENSIFY(2 << 27),
		LUCKYTIME(2 << 29),
		PRAY(2 << 30),
		FATAL_STRIKE(0x800000000000l),
		CHAIN_LIGHTING(268435456l);
		
		public final long value;
		private Flag(long value) { this.value = value; }
	}

	
}
