package data;

import packets.OutgoingPacket;

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
		this.location = location;
		this.HP = HP;
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

	public void setLocation(Location location) {
		this.location = location;
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
		if ( HP >= 0 ) {
			this.HP = HP;
		}
		this.HP = 0;
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
	
	public abstract OutgoingPacket spawn();
	
	public static enum GuildRank {
		None(0),
		Member(50),
		InternManager(60),
		DeputyManager(70),
		BranchManager(80),
		DeputyLeader(90),
		Leader(100);
		
		final int rank;
		
		private GuildRank(int rank) {
			this.rank = rank;
		}
		
		public int getRank() {
			return rank;
		}
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
