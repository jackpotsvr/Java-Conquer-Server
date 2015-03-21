package net.co.java.packets.serialization;

import net.co.java.packets.GeneralDataPacket;
import net.co.java.packets.IncomingPacket;
import net.co.java.packets.Packet.PacketHeader;
import net.co.java.packets.PacketType;

public enum PacketDeserializerFactory {
	GENERAL_DATA_PACKET_DESERIALIZER(PacketType.GENERAL_DATA_PACKET) {
		public PacketDeserializer<GeneralDataPacket> getInstance(IncomingPacket ip) {
				PacketDeserializer<GeneralDataPacket> deserializer = 
						new PacketDeserializer<GeneralDataPacket>(ip, getPacketHeader(ip)){};
				/* The extra brackets are important for generic type determination. */ 
				return deserializer;
		}
	};
	
	private PacketType type; 
	
	private PacketDeserializerFactory(PacketType type) {
		this.type = type; 
	}
	
	public abstract PacketDeserializer<?> getInstance(IncomingPacket ip);

	/** 
	 * @param type The PacketType of the packet you want to be deserialized. 
	 * @return instance of the factory that can instantiate the deserializer with getInstance(). 
	 */
	public static PacketDeserializerFactory valueOf(PacketType type) {
		for ( PacketDeserializerFactory pt : PacketDeserializerFactory.values() ) {
			if ( pt.type == type )
				return pt;
		}
		return null; 
	}
	
	private static PacketHeader getPacketHeader(IncomingPacket ip) { 
		return PacketDeserializer.deserializeHeader(ip); 
	}
}
