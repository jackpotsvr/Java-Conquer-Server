package net.co.java.packets.serialization;

import java.lang.annotation.ElementType;
import java.lang.annotation.Retention;
import java.lang.annotation.RetentionPolicy;
import java.lang.annotation.Target;

/**
 * @author Thomas Gmelig Meyling
 * Annotation for the length of a packet. This is the length WITHOUT String (PacketValueType.STRING_WITH_LENGTH length EXCEPT 
 * for Strings with a static length (PacketValueType.STRING). 
 * The total string length will be computed and added to this in the serialization process. 
 */
@Retention(RetentionPolicy.RUNTIME)
@Target(ElementType.TYPE)
public @interface PacketLength {
	int length(); 
}
