package data;

import java.util.ArrayList;
import java.util.List;
import java.util.concurrent.CopyOnWriteArrayList;

import packets.OutgoingPacket;
import conquerServer.GameServerThread;

public class Map {

	private final int id;
	private final List<Entity> entities = new CopyOnWriteArrayList<Entity>();
	
	public Map(int id) {
		this.id = id;
	}
	
	public int getMapID() {
		return id;
	}
	
	public void addEntity(Entity entity) {
		entities.add(entity);
	}
	
	public void removeEntity(Entity entity) {
		entities.remove(entity);
	}
	
	public List<Entity> getEntities() {
		return entities;
	}
	
	public List<Entity> getEntitiesInRange(Entity me) {
		List<Entity> result = new ArrayList<Entity>();
		
		for ( Entity e : entities ) {
			if ( e.equals(me) ) {
				continue;
			}
			
			if (e.getLocation().inView(me.getLocation())) {
				result.add(e);
			}
		}
		
		return result;
	}
	
	public void updateMap(OutgoingPacket esp, GameServerThread thread)
	{
		
		for( Entity e : this.getEntitiesInRange(thread.getPlayer()) ) {	
			if (e instanceof Player)
			{
				Player p = (Player) e;
				p.getThread().offer(esp);
			}
		}
	}

}
