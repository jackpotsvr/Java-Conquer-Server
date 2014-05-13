package net.co.java.entity.view;

import java.util.Iterator;
import java.util.List;

import net.co.java.entity.Entity;
import net.co.java.entity.Player;

public interface View {
	
	void add(Entity entity);
	
	boolean contains(Entity entity);
	
	void remove(Entity entity);
	
	int size();
	
	List<Entity> getEntities();
	
	List<Player> getPlayers();
	
	Iterator<Entity> entities();
	
	Iterator<Player> players();
	
}
