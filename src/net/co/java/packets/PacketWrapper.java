package net.co.java.packets;

/**
 * A {@code PacketWrapper} wraps an {@code IncomingPacket} and provides
 * additional getters (and optional setters for bi-directional packets)
 * for the packet attributes. 
 * 
 * @author Jan-Willem Gmelig Meyling
 * @author Thomas Gmelig Meyling
 *
 */
public interface PacketWrapper {

	/*
	 * Todo add methods here
	 */
	
	/**
	 * 
	 * @return A PacketWriter based on this PacketWrapper
	 */
	PacketWriter build();
	
}
