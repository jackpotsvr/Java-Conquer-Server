package net.co.java.entity;

import net.co.java.server.Map;

/**
 * The Location class contains the information for a location
 * in a Map
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 */
public class Location {

	public final static int VIEW_RANGE = 18;
	public final int xCord, yCord;
	public final Map map;
	public final int direction;
	
	/**
	 * Construct a new Location in this Map
	 * @param map
	 * @param xCord
	 * @param yCord
	 */
	public Location(Map map, int xCord, int yCord) {
		this(map, xCord, yCord, 0);
	}
	
	public int getDistanceFrom(Location l) {
		return 1;
	}
	/**
	public int getDirection(Location l) {
		if(this.yCord > l.yCord)
		{
			if(this.xCord > l.xCord)
				return 5;
			else if (this.xCord == l.xCord)
				return 4;
			else
				return 3;
		} else if(this.yCord == l.yCord)
		{
			if(this.xCord > l.xCord)
				return 2;
			else
				return 6;
		} else {
			if(this.xCord > l.xCord)
				return 1;
			else if (this.xCord == l.xCord)
				return 0;
			else
				return 7;
		}
	} */ 
	
	public int getDirection(Location l) {
		int dY = l.yCord - this.yCord;
		int dX = l.xCord - this.xCord;
		if(dY == dX && dY == 0) return 0;
	    return (int) ((Math.atan2(dY, dX) / Math.PI * 180) + 270) % 360 / 40;
	}
	

	/**
	 * Construct a new Location in this Map
	 * @param map
	 * @param xCord
	 * @param yCord
	 * @param direction
	 */
	public Location(Map map, int xCord, int yCord, int direction) {
		this.xCord = xCord;
		this.yCord = yCord;
		this.map = map;
		this.direction = direction % 8;
	}

	/**
	 * @return the Map for this Location
	 */
	public Map getMap() {
		return map;
	}
	
	/**
	 * @return the x-coordinate for this Location
	 */
	public int getxCord() {
		return xCord;
	}
	
	/**
	 * @return the direction
	 */
	public int getDirection() {
		return direction;
	}
	
	/**
	 * @return the y-coordinate for this Location
	 */
	public int getyCord() {
		return yCord;
	}
	
	/**
	 * @param x offset
	 * @param y offset
	 * @return a new Location in this name with a given offset
	 * from this Location
	 */
	public Location atOffset(int x, int y) {
		return new Location(map, xCord + x, yCord + y, direction);
	}
	
	/**
	 * @param direction
	 * @return the next point in the given direction
	 */
	public Location inDirection(int direction) {
		switch (direction%8) {
        case 0:
            return new Location(map, xCord, yCord+1, direction);
        case 1:
            return new Location(map, xCord-1, yCord+1, direction);
        case 2:
            return new Location(map, xCord-1, yCord, direction);
        case 3:
            return new Location(map, xCord-1, yCord-1, direction);
        case 4:
            return new Location(map, xCord, yCord-1, direction);
        case 5:
            return new Location(map, xCord+1, yCord-1, direction);
        case 6:
            return new Location(map, xCord+1, yCord, direction);
        default:
            return new Location(map, xCord+1, yCord+1, direction);
		}
	}
	
	/**
	 * @param direction
	 * @return Create a new Location with only a changed direction
	 */
	public Location switchDirection(int direction) {
		return new Location(map, xCord, yCord, direction);
	}
	
	/**
	 * @param location
	 * @return true if an entity is within the default sight of
	 * the entity at this location
	 */
	public boolean inView(Location location) {
		return location.getMap().equals(getMap())
				&& Math.sqrt(Math.pow(location.xCord - xCord, 2) + Math.pow(location.yCord - yCord, 2)) < VIEW_RANGE;
		//		&& Math.abs(location.xCord - xCord) < VIEW_RANGE
		//		&& Math.abs(location.yCord - yCord) < VIEW_RANGE;
	}
	
	
	@Override
	public String toString() {
		return map.toString() + "(" + xCord + "; " + yCord + ")";
	}

	@Override
	public boolean equals(Object obj) {
		if (obj instanceof Location) {
			Location other = (Location) obj;
			return other.map.equals(map)
					&& other.xCord == xCord
					&& other.yCord == yCord;
		}
		return false;
	}

}
