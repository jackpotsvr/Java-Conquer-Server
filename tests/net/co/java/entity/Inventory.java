package net.co.java.entity;
import static org.junit.Assert.*;

import java.io.FileNotFoundException;
import java.util.ArrayList;

import net.co.java.entity.Player;
import net.co.java.item.ItemInstance;
import net.co.java.item.ItemPrototype;
import net.co.java.model.AccessException;
import net.co.java.model.Mock;
import net.co.java.model.Model;

import org.junit.Before;
import org.junit.Test;


public class Inventory {
	
	private Player player;
	private static Model mock;
	private static ArrayList<ItemInstance> items = new ArrayList<ItemInstance>();
	private static long itemIdentity = 2342345l;

	static {
		try {
			mock = new Mock();
			createItems();
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (AccessException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	}
	
	@Before
	public void setUp() throws Exception {
		player = new Player(100000l, "TestPlayer", null, 1000);
	}

	private static void createItems() throws AccessException {
		final ItemPrototype METEOR = mock.getItemPrototype(1088001l);
		final ItemPrototype DRAGONBALL = mock.getItemPrototype(1088000l);
		
		for(int i = 0; i < 40; i++ )
			items.add(new ItemInstance(itemIdentity++, METEOR));
		for(int i = 0; i < 10; i++ )
			items.add(new ItemInstance(itemIdentity++, DRAGONBALL));
	}

	@Test
	public void testEmtpyToFull() {
		assertTrue(player.inventory.isEmpty());
		assertFalse(player.inventory.isFull());
		for(int i = 0; i < 40; i++ )
			player.inventory.addItem(items.get(i));
		for(int i = 0; i < 40; i++ )
			assertTrue(player.inventory.contains(items.get(i)));
		for(int i = 40, s = items.size(); i < s; i++ )
			assertFalse(player.inventory.contains(items.get(i)));
		assertTrue(player.inventory.isFull());
		assertFalse(player.inventory.isEmpty());
	}
	

	@Test
	public void fullInventory() {
		for(int i = 0, s = items.size(); i < s; i++ )
			player.inventory.addItem(items.get(i));
		assertTrue(player.inventory.isFull());
		for(int i = 0; i < 40; i++ )
			assertTrue(player.inventory.contains(items.get(i)));
		for(int i = 40, s = items.size(); i < s; i++ )
			assertFalse(player.inventory.contains(items.get(i)));
	}
	
	@Test
	public void removeFromIntenvory() {
		ItemInstance A = items.get(1);
		ItemInstance B = items.get(2);
		player.inventory.addItem(A);
		assertTrue(player.inventory.contains(A));
		assertFalse(player.inventory.contains(B));
		player.inventory.addItem(B);
		player.inventory.removeItem(A);
		assertFalse(player.inventory.contains(A));
		assertTrue(player.inventory.contains(B));
	}
	
	@Test
	public void removeFirst() {
		ItemInstance A = items.get(1);
		ItemInstance B = items.get(2);
		player.inventory.addItem(A);
		assertTrue(player.inventory.contains(A));
		assertFalse(player.inventory.contains(B));
		player.inventory.removeItem(A);
		player.inventory.addItem(B);
		assertFalse(player.inventory.contains(A));
		assertTrue(player.inventory.contains(B));
	}
	
	@Test
	public void removeAll() {
		for(int i = 0, s = items.size(); i < s; i++ )
			player.inventory.addItem(items.get(i));
		assertTrue(player.inventory.isFull());
		for(int i = 0; i < 40; i++ )
			assertTrue(player.inventory.removeItem(items.get(i)));
		System.out.println(player.inventory);
		assertTrue(player.inventory.isEmpty());
	}

}
