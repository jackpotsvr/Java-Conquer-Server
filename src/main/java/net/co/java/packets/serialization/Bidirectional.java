package net.co.java.packets.serialization;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

import net.co.java.packets.packethandlers.AbstractPacketHandler;
import net.co.java.packets.packethandlers.NoPacketHandler;

@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.TYPE)
public @interface Bidirectional {
	/**
	 * @return The handler for the packet. This is a NoPacketHandler by default, 
	 * if the packet doesn't have a PacketHandler. 
	 */
	Class<? extends AbstractPacketHandler> handler() default NoPacketHandler.class; 
}
