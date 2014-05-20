package net.co.java.skill;

import java.util.Arrays;
import java.util.Iterator;
import java.util.List;

import net.co.java.entity.Entity;
import net.co.java.entity.Location;
import net.co.java.entity.Monster;
import net.co.java.entity.Player;

/**
 * A {@code TargetBuilder} is an iterable collection of entities that satisfy
 * given filter rules
 * 
 * @author Jan-Willem Gmelig Meyling
 * 
 */
public class TargetBuilder implements Iterator<Entity>, Iterable<Entity> {
	
	private final Location A;
//	private final Entity target;
	private Entity[] temp;
	private final int capacity;
	private int size;
	private int position;
	
	public TargetBuilder(Player hero) {
		A = hero.getLocation();
//		target = null; // TODO Get target
		List<Entity> entities = hero.view.getEntities();
		entities.remove(hero);
		temp = new Entity[entities.size()];
		temp = entities.toArray(temp); 
		capacity = temp.length;
		size = temp.length;
	}
	
	public TargetBuilder(Monster m) {
		A = m.getLocation();
		temp = new Entity[1];
		List<Entity> entities = m.view.getEntities();
		entities.remove(m);
		
		// Monster only target players.
		for(Entity e : entities)
			if(!(e instanceof Player))
				entities.remove(e);
		
		temp = new Entity[entities.size()];
		temp = entities.toArray(temp);
		
		capacity = temp.length;
		size = temp.length;
	}
	
	/** for single target skills */ 
	public TargetBuilder(Entity entity)
	{
		A = null; 
		temp = new Entity[1];
		List<Entity> entities = entity.view.getEntities();
		//entities.remove(entity);
		
		temp[0] = entity;
		capacity = temp.length;
		size = temp.length;
	}
	
	/**
	 * Filter the entities. Entities on a line between the current location
	 * and a given point remain in the result.
	 * @param BxCord
	 * @param ByCord
	 * @return This TargetBuilder
	 */
	public TargetBuilder inLinePart(int BxCord, int ByCord) {
		double a = ((double) (ByCord - A.yCord)) / ((double) (BxCord - A.xCord));
		for(int i = 0; i < capacity && size > 0; i++ ) {
			Entity e = temp[i];
			if(e != null) {
				Location C = e.getLocation();
				/* Yc should be between Ya and Yb (line part)
				 * c should be on the line through a and b
				 * Exception for when Xa == Xb == Xc (a = Infinity)
				 * Negation because it's easier to read in a positive way :)
				 */
				if(!( (    (A.yCord <= C.yCord && C.yCord <= ByCord)
					   	|| (A.yCord >= C.yCord && C.yCord >= ByCord)
					  ) && (
						   (A.xCord == BxCord && A.xCord == C.xCord)  
						||  Math.round(a * (C.xCord - A.xCord) + A.yCord) == C.yCord ))) {
					temp[i] = null;
					size--;
				}
			}
		}
		return this;
	}
	
	/**
	 * Used for inView skills and skills like Rage / Fire ring
	 * @param radius
	 * @return This TargetBuilder
	 */
	public TargetBuilder inCircle(int radius) {
		for(int i = 0; i < capacity && size > 0; i++ ) {
			Entity e = temp[i];
			if(e != null) {
				Location B = e.getLocation();
				if(Math.sqrt(Math.pow(A.xCord - B.xCord, 2) + Math.pow(A.yCord - B.yCord, 2)) > radius) {
					temp[i] = null;
					size--;
				}
			}
		}
		return this;
	}
	
	public int size() {
		return size;
	}
	
	// public void inTriangle(int width, int radius, int direction) {}
	// public void atPosition(int x, int y) {}
	// public void inTeam() {};
	// public void PKMode() {};
	
	public Entity[] getEntities() {
		Entity[] result = new Entity[size];
		for ( int i = 0, j = 0; i < capacity; i++ )
			if ( temp[i] != null )
				result[j++] = temp[i];
		return result;
	}
	
	@Override
	public Iterator<Entity> iterator() {
		position = 0;
		return this;
	}

	@Override
	public boolean hasNext() {
		for(; position < capacity; position++ )
			if(temp[position] != null )
				return true;
		return false;
	}

	@Override
	public Entity next() {
		return temp[position++];
	}

	@Override
	public void remove() {
		if(position > 0 && position < capacity)
			temp[position-1] = null;
	}

	@Override
	public String toString() {
		return "TargetBuilder [getEntities()=" + Arrays.toString(getEntities()) + "]";
	}
}
