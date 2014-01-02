package data;

public class Location {

	private final static int VIEW_RANGE = 12;
	
	private final Map map;
	private final int xCord, yCord;

	public Location(Map map, int xCord, int yCord) {
		this.map = map;
		this.xCord = xCord;
		this.yCord = yCord;
	}

	public Map getMap() {
		return map;
	}

	public int getxCord() {
		return xCord;
	}

	public int getyCord() {
		return yCord;
	}
	
	public Location offset(int x, int y) {
		return new Location(map, xCord + x, yCord + y);
	}
	
	public boolean inView(Location location) {
		return location.map.equals(map)
				&& Math.abs(location.xCord - xCord) < VIEW_RANGE
				&& Math.abs(location.yCord - yCord) < VIEW_RANGE;
	}
	
	
	@Override
	public String toString() {
		return map.toString() + ": (" + xCord + "; " + yCord + ")";
	}

	@Override
	public boolean equals(Object obj) {
		if (obj instanceof Location) {
			Location other = (Location) obj;
			return other.map.equals(map) && other.xCord == xCord
					&& other.yCord == yCord;
		}
		return super.equals(obj);
	}

}
