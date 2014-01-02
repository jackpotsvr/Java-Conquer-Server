package data;

public abstract class Entity implements Spawnable {

	private final long identity;
	private final String name;
	private Location location;
	private int HP;
	
	
	public Entity(long identity, String name, Location location, int HP) {
		this.identity = identity;
		this.name = name;
		this.location = location;
		this.HP = HP;
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

}
