package net.co.java.entity.view;

import java.util.ArrayList;
import java.util.HashSet;
import java.util.Iterator;
import java.util.List;
import java.util.Set;

import net.co.java.entity.Entity;
import net.co.java.entity.Location;
import net.co.java.entity.Player;

/**
 * A basic implementation for {@code View}
 * @author Jan-Willem Gmelig Meyling
 */
public class ViewImpl implements View, MovementListener {
	
	private final Entity me;
	private final Set<Entity> entities;
	
	public ViewImpl(Entity entity) {
		this.me = entity;
		this.entities = new HashSet<>();
	}

	@Override
	public void add(Entity entity) {
		boolean added;
		synchronized(entities) {
			added = entities.add(entity);
		}
		if(added) {
			me.notify(entity.SpawnPacket());
			entity.view.add(me);
		}
	}
	
	@Override
	public boolean contains(Entity entity) {
		synchronized(entities) {
			return entities.contains(entity);
		}
	}

	@Override
	public void remove(Entity entity) {
		boolean removed;
		synchronized(entities) {
			removed = entities.remove(entity);
		}
		if(removed) {
			me.notify(entity.removeEntity());
			entity.view.remove(me);
		}
	}

	@Override
	public int size() {
		synchronized(entities) {
			return entities.size();
		}
	}

	@Override
	public List<Entity> getEntities() {
		synchronized(entities) {
			return new ArrayList<>(entities);
		}
	}

	@Override
	public List<Player> getPlayers() {
		List<Player> players = new ArrayList<>();
		for(Entity e : getEntities()) {
			if(e instanceof Player) {
				players.add((Player) e);
			}
		}
		return players;
	}

	@Override
	public Iterator<Entity> entities() {
		return getEntities().iterator();
	}

	@Override
	public Iterator<Player> players() {
		return getPlayers().iterator();
	}

	@Override
	public void movedTo(Entity entitiy, Location location) {
		List<Entity> allEntities = location.getMap().getEntities();
		synchronized(entities) {
			
		}
	}

}
